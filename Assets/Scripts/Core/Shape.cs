using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    bool m_canRotate = true;

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

    public void RotateLeft()
    {
        if(m_canRotate)
            transform.Rotate(0, 0, 90);
    }

    public void RotateRight()
    {
        if(m_canRotate)
            transform.Rotate(0, 0, -90);
    }


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("MoveDown", 0f, 0.5f);
        InvokeRepeating("RotateLeft", 0f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
