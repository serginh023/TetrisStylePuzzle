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

    void Awake()
    {
        m_grid = new Transform[m_width, m_height];
    }
    // Start is called before the first frame update
    void Start()
    {
        DrawEmptyCells();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        if(shape == null)
        { 
            return;
        }
        else
        {
            foreach (Transform child in shape.transform)
            {
                Vector2 pos = child.position;
                m_grid[(int)pos.x, (int)pos.y] = child;
            }
        }
    }

    public bool isOccupied(int x, int y,Shape shape)
    {
        return (m_grid[x, y] != null && m_grid[x, y].parent != shape.transform);
    }
}
