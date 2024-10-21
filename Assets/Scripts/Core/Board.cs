using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Transform m_emptySprite;
    public int m_height = 30;
    public int m_width = 10;
    public int m_header = 8;

    Transform[,] m_grid;

    public int m_completedRows = 0;

    public ParticlePlayer[] m_rowGlowFX;

    void Awake()
    {
        m_grid = new Transform[m_width, m_height];
    }
    // Start is called before the first frame update
    void Start()
    {
        DrawEmptyCells();
    }

    bool IsWithinBoard(int x, int y)
    {
        return (x >= 0 && x < m_width && y >= 0);
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

            if(isOccupied((int)pos.x, (int)pos.y, shape))
            {
                return false;
            }
        }

        return true;
    }

    void DrawEmptyCells()
    {
        if (m_emptySprite != null)
            for (int i = 0; i < m_height - m_header; i++)
                for (int j = 0; j < m_width; j++)
                {
                    Transform clone;
                    clone = Instantiate(m_emptySprite, new Vector3(j, i, 0), Quaternion.identity) as Transform;
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
            m_grid[(int)pos.x, (int)pos.y] = child;
        }
        
    }

    public bool isOccupied(int x, int y,Shape shape)
    {
        return (m_grid[x, y] != null && m_grid[x, y].parent != shape.transform);
    }

    bool IsComplete(int y)
    {
        for (int i = 0; i < m_width; i++)
            if (m_grid[i, y] == null)
                return false;

        return true;
    }

    void ClearRow(int y)
    {
        for (int x = 0; x < m_width; x++)
        { 
            if (m_grid[x, y] != null)
                Destroy(m_grid[x, y].gameObject);

            m_grid[x, y] = null;
        }

    }

    void ShiftOneRowDown(int y)
    {
        for (int x = 0; x < m_width; x++)
            if (m_grid[x, y] != null)
            {       
                m_grid[x, y - 1] = m_grid[x, y];
                m_grid[x, y] = null;
                m_grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
    }

    void ShiftRowsDown(int startY)
    {
        for (int i = startY; i < m_height; i++)
            ShiftOneRowDown(i);
    }

    public IEnumerator ClearAllRows()
    {
        m_completedRows = 0;

        for (int y = 0; y < m_height; y++)
            if (IsComplete(y))
            {
                ClearGlowFX(m_completedRows, y);
                m_completedRows++;
            }

        yield return new WaitForSeconds(.5f);

        for (int y = 0; y < m_height; y++)
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
            if (child.transform.position.y >= (m_height - m_header - 1) )
                return true;

        return false;
    }

    public void ClearGlowFX(int index, int y)
    {
        if (m_rowGlowFX[index])
        {
            m_rowGlowFX[index].transform.position = new Vector3(0, y, -2f);
            m_rowGlowFX[index].Play();
        }
    }
}
