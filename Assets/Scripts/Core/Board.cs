using System.Collections;
using UnityEngine;
using Utility;

namespace Core
{
    public class Board : MonoBehaviour
    {
        private const int WIDTH = 10;
        private const int HEADER = 8;
        private const int HEIGHT = 30;
    
        private int completedRows;
        private Transform[,] grid;

        [SerializeField] private Transform emptySprite;
        [SerializeField] private ParticlePlayer[] rowGlowFX;

        public int CompletedRows => completedRows;

        private void Awake()
        {
            grid = new Transform[WIDTH, HEIGHT];
        }
    
        private void Start()
        {
            DrawEmptyCells();
        }

        private bool IsWithinBoard(int x, int y)
        {
            return (x >= 0 && x < WIDTH && y >= 0);
        }

        public bool IsValidPosition(Shape shape)
        {
            foreach (Transform child in shape.transform)
            {
                Vector2 pos = Vectorf.Round(child.position);

                if ( !IsWithinBoard((int)pos.x, (int)pos.y) )
                {
                    return false;
                }

                if(IsOccupied((int)pos.x, (int)pos.y, shape))
                {
                    return false;
                }
            }
            return true;
        }

        private void DrawEmptyCells()
        {
            if (emptySprite != null)
                for (var i = 0; i < HEIGHT - HEADER; i++)
                for (var j = 0; j < WIDTH; j++)
                {
                    Transform clone;
                    clone = Instantiate(emptySprite, new Vector3(j, i, 0), Quaternion.identity);
                    clone.name = "Board Space ( x = " + j.ToString() + ", y = " + i.ToString() + ")";
                    clone.transform.parent = transform;
                }
            else
                Debug.LogWarning("WARNING! PLease assign the emptySprite object!");
        }

        public void StoreShapeInGrid(Shape shape)
        {
            if(shape == default)
                return;
        
            foreach (Transform child in shape.transform)
            {
                Vector2 pos = Vectorf.Round(child.position);
                grid[(int)pos.x, (int)pos.y] = child;
            }
        }

        private bool IsOccupied(int x, int y,Shape shape)
        {
            return (grid[x, y] != default && grid[x, y].parent != shape.transform);
        }

        private bool IsComplete(int y)
        {
            for (var i = 0; i < WIDTH; i++)
            {
                if (grid[i, y] == default)
                    return false;
            }
            return true;
        }

        private void ClearRow(int y)
        {
            for (var x = 0; x < WIDTH; x++)
            {
                if (grid[x, y] != default)
                {
                    Destroy(grid[x, y].gameObject);
                }
                grid[x, y] = null;
            }
        }

        private void ShiftOneRowDown(int y)
        {
            for (var x = 0; x < WIDTH; x++)
                if (grid[x, y] != default)
                {       
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;
                    grid[x, y - 1].position += new Vector3(0, -1, 0);
                }
        }

        private void ShiftRowsDown(int startY)
        {
            for (var i = startY; i < HEIGHT; i++)
                ShiftOneRowDown(i);
        }

        public void ClearAllRows()
        {
            StartCoroutine(ClearAllRowsIENumerator());
        }

        private IEnumerator ClearAllRowsIENumerator()
        {
            completedRows = 0;

            for (var y = 0; y < HEIGHT; y++)
                if (IsComplete(y))
                {
                    ClearGlowFX(completedRows, y);
                    completedRows++;
                }

            yield return new WaitForSeconds(.5f);

            for (var y = 0; y < HEIGHT; y++)
                if (IsComplete(y))
                {
                    ClearRow(y);
                    ShiftRowsDown(y + 1);
                    y--;
                    yield return new WaitForSeconds(.2f);
                }
        }

        public bool IsOverLimit(Shape shape)
        {
            foreach(Transform child in shape.transform)
            {
                if (child.position.y >= (HEIGHT - HEADER - 1) )
                    return true;
            }
            return false;
        }

        private void ClearGlowFX(int index, int y)
        {
            if (!rowGlowFX[index]) return;
            rowGlowFX[index].transform.position = new Vector3(0, y, -2f);
            rowGlowFX[index].Play();
        }
    }
}
