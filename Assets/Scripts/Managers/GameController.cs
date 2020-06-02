using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class GameController : MonoBehaviour
{
    [SerializeField]
    Board m_gameBoard;
    [SerializeField]
    Spawner m_spawner;

    //shape ativo
    Shape m_activeShape;

    [SerializeField]
    float m_dropRate = .9f;

    float m_timeToDrop = 0f;

    float m_TimeToNextKeyLeftRight;

    [SerializeField][Range(0.02f, 1f)]
    float m_keyRepeatRateLeftRight = 0.065f;

    float m_TimeToNextKeyDown;

    [SerializeField]
    [Range(0.01f, 1f)]
    float m_keyRepeatRateDown = 0.05f;

    float m_TimeToNextKeyRotate;

    [SerializeField]
    [Range(0.02f, 1f)]
    float m_keyRepeatRateRotate = 0.065f;

    bool m_gameOver = false;

    public GameObject m_gameOverPanel;

    // Start is called before the first frame update
    void Start()
    {
        m_TimeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
        m_TimeToNextKeyDown = Time.time + m_keyRepeatRateDown;
        m_TimeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

        if (!m_gameBoard)
            Debug.LogWarning("WARNING! There is no board definied!");

        if (!m_spawner)
            Debug.LogWarning("WARNING! There is no spawner definied!");
        else
        {
            m_spawner.transform.position = Vectorf.Round(m_spawner.transform.position);
            if (!m_activeShape)
            {
                m_activeShape = m_spawner.SpawnShape();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_gameBoard || !m_spawner || !m_activeShape || m_gameOver)
            return;

        PlayerInput();
    }


    void PlayerInput()
    {
        //if(Input.GetKey("right") && Time.time > m_TimeToNextKey || Input.GetKeyDown(KeyCode.RightArrow) ) --> Alternativa, não passa pelo input manager
        if (Input.GetButton("MoveRight") && Time.time > m_TimeToNextKeyLeftRight || Input.GetButtonDown("MoveRight"))// GetButtonDown registra somente o primeiro frame que o botão foi pressionado
        {
            m_TimeToNextKeyLeftRight += Time.time + m_keyRepeatRateLeftRight;
            m_activeShape.MoveRight();
            if (!m_gameBoard.IsValidPosition(m_activeShape))
                m_activeShape.MoveLeft();

        }
        else if (Input.GetButton("MoveLeft") && Time.time > m_TimeToNextKeyLeftRight || Input.GetButtonDown("MoveLeft"))// GetButtonDown registra somente o primeiro frame que o botão foi pressionado
        {
            m_TimeToNextKeyLeftRight += Time.time + m_keyRepeatRateLeftRight;
            m_activeShape.MoveLeft();
            if (!m_gameBoard.IsValidPosition(m_activeShape))
                m_activeShape.MoveRight();

        }
        else if (Input.GetButtonDown("Rotate") && Time.time > m_TimeToNextKeyRotate)// GetButtonDown registra somente o primeiro frame que o botão foi pressionado
        {
            m_TimeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;
            m_activeShape.RotateRight();
            if (!m_gameBoard.IsValidPosition(m_activeShape))
                m_activeShape.RotateLeft();

        }
        else if ( Input.GetButton("MoveDown") && (Time.time > m_TimeToNextKeyDown) || (Time.time > m_timeToDrop) )
        {
            m_timeToDrop = Time.time + m_dropRate;
            m_TimeToNextKeyDown = Time.time + m_keyRepeatRateDown;
            m_activeShape.MoveDown();

            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                if(m_gameBoard.IsOverLimit(m_activeShape))
                {
                    GameOver();
                }
                else
                {
                    LandShape();
                }
                    
            }
                
        }

        //Drop
        if (Time.time > m_timeToDrop)
        {
            m_timeToDrop = Time.time + m_dropRate;

            if (m_activeShape)
                m_activeShape.MoveDown();

            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.MoveUp();
                m_gameBoard.StoreShapeInGrid(m_activeShape);
                if (m_spawner)
                    m_activeShape = m_spawner.SpawnShape();
            }
        }
    }

    private void GameOver()
    {
        m_activeShape.MoveUp();
        m_gameOver = true;
        Debug.LogWarning(m_activeShape + " Shape is over the limit check");

        m_gameOverPanel.SetActive(true);
    }

    void LandShape()
    {
        m_activeShape.MoveUp();
        m_gameBoard.StoreShapeInGrid(m_activeShape);
        m_activeShape = m_spawner.SpawnShape();

        m_TimeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
        m_TimeToNextKeyDown = Time.time + m_keyRepeatRateDown;
        m_TimeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

        m_gameBoard.ClearAllRows();


    }

    public void Restart()
    {
        Debug.Log("Restarted");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



}
