using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GameController : MonoBehaviour
{
    [SerializeField]
    Board m_gameBoard;
    [SerializeField]
    Spawner m_spawner;

    //shape ativo
    Shape m_activeShape;

    float m_dropInterval = .25f;

    float m_timeToDrop = 0f;



    // Start is called before the first frame update
    void Start()
    {
        if (m_spawner)
        {
            m_spawner.transform.position = Vectorf.Round(m_spawner.transform.position);
            if (m_activeShape == null)
            {
                m_activeShape = m_spawner.SpawnShape();
            }
        }

        if (!m_gameBoard)
            Debug.LogWarning("WARNING! There is no board definied!");

        if (!m_spawner)
            Debug.LogWarning("WARNING! There is no spawner definied!");

    }

    // Update is called once per frame
    void Update()
    {
        if (!m_gameBoard || !m_spawner)
            return;

        if(Time.time > m_timeToDrop)
        {
            m_timeToDrop = Time.time + m_dropInterval;

            if (m_activeShape)
                m_activeShape.MoveDown();

            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.MoveUp();
                m_gameBoard.StoreShapeInGrid(m_activeShape);
                if(m_spawner)
                    m_activeShape = m_spawner.SpawnShape();
            }
        }
    }
}
