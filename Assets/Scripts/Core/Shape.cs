using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    bool m_canRotate = true;
    public Vector3 m_QueueOffSet;
    public GameObject[] m_glowSquareFX;

    private void Awake()
    {
        m_glowSquareFX = GameObject.FindGameObjectsWithTag("LandShapeFX");
    }

    void Move(Vector3 moveDirection)
    {
        transform.position += moveDirection;
    }

    public void MoveLeft()
    {
        Move(new Vector3(-1, 0, 0));
    }

    public void MoveRight()
    {
        Move(new Vector3(1, 0, 0));
    }

    public void MoveDown()
    {
        Move(new Vector3(0, -1, 0));
    }
    
    public void MoveUp()
    {
        Move(new Vector3(0, 1, 0));
    }

    void RotateLeft()
    {
        if(m_canRotate)
            transform.Rotate(0, 0, 90);
    }

    void RotateRight()
    {
        if(m_canRotate)
            transform.Rotate(0, 0, -90);
    }

    public void RotateClockwise(bool clockwise)
    {
        if (clockwise)
            RotateRight();
        else
            RotateLeft();
    }

    public void LandShapeFX()
    {
        int i = 0;
        //Transform[] transforms = GetComponentsInChildren<Transform>();
        //for(int i = 0; i < m_glowSquareFX.Length; i++)
        foreach(Transform child in transform)//subistitui as duas linhas acima
        {
            if (m_glowSquareFX[i])
            {
                m_glowSquareFX[i].transform.position = new Vector3(child.position.x, child.position.y, -2);
                ParticlePlayer pp = m_glowSquareFX[i].GetComponent<ParticlePlayer>();
                if(pp)
                    pp.Play();
            }
            i++;
        }
    }
}
