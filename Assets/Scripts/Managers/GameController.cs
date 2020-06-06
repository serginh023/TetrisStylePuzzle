using System;
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
    [SerializeField]
    SoundManager m_soundManager;
    [SerializeField]
    ScoreManager m_scoreManager;
    [SerializeField]
    Ghost m_ghost;
    [SerializeField]
    Holder m_holder;

    //shape ativo
    Shape m_activeShape;

    [SerializeField]
    float m_dropRate = .9f;
    float m_dropRateModded;
    
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

    public IconToggle m_rotIconToggle;

    bool m_clockwise = true;

    public bool m_isPaused = false;

    public GameObject m_pausePanel;


    // Start is called before the first frame update
    void Start()
    {
        m_TimeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
        m_TimeToNextKeyDown = Time.time + m_keyRepeatRateDown;
        m_TimeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

        if (!m_gameBoard)
            Debug.LogWarning("WARNING! There is no board definied!");

        if (!m_soundManager)
            Debug.LogWarning("WARNING! There is no soundManager definied!");

        if (!m_scoreManager)
            Debug.LogWarning("WARNING! There is no scoreManager definied!");

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
        if (!m_gameBoard || !m_spawner || !m_activeShape || m_gameOver || !m_soundManager)
            return;

        PlayerInput();
    }

    private void LateUpdate()
    {
        if (m_ghost)
        {
            m_ghost.DrawGhost(m_activeShape, m_gameBoard);
        }
    }


    void PlayerInput()
    {
        //if(Input.GetKey("right") && Time.time > m_TimeToNextKey || Input.GetKeyDown(KeyCode.RightArrow) ) --> Alternativa, não passa pelo input manager
        if (Input.GetButton("MoveRight") && Time.time > m_TimeToNextKeyLeftRight || Input.GetButtonDown("MoveRight"))// GetButtonDown registra somente o primeiro frame que o botão foi pressionado
        {
            m_TimeToNextKeyLeftRight += Time.time + m_keyRepeatRateLeftRight;
            m_activeShape.MoveRight();
            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.MoveLeft();
                PlaySound(m_soundManager.m_errorSound, .8f);
            }
            else
            {
                PlaySound(m_soundManager.m_moveSound, .8f);
            }

        }
        else if (Input.GetButton("MoveLeft") && Time.time > m_TimeToNextKeyLeftRight || Input.GetButtonDown("MoveLeft"))// GetButtonDown registra somente o primeiro frame que o botão foi pressionado
        {
            m_TimeToNextKeyLeftRight += Time.time + m_keyRepeatRateLeftRight;
            m_activeShape.MoveLeft();
            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.MoveRight();
                PlaySound(m_soundManager.m_errorSound, .8f);
            }
            else
            {
                PlaySound(m_soundManager.m_moveSound, .8f);
            }

        }
        else if (Input.GetButtonDown("Rotate") && Time.time > m_TimeToNextKeyRotate)// GetButtonDown registra somente o primeiro frame que o botão foi pressionado
        {
            m_TimeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;
            m_activeShape.RotateClockwise(m_clockwise);
            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.RotateClockwise(!m_clockwise);
                PlaySound(m_soundManager.m_errorSound, .8f);
            }
            else
            {
                PlaySound(m_soundManager.m_moveSound, .8f);
            }

        }
        else if ( Input.GetButton("MoveDown") && (Time.time > m_TimeToNextKeyDown) || (Time.time > m_timeToDrop) )
        {
            m_timeToDrop = Time.time + m_dropRate;
            m_TimeToNextKeyDown = Time.time + m_keyRepeatRateDown;
            m_activeShape.MoveDown();

            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                if (m_gameBoard.IsOverLimit(m_activeShape))
                {
                    GameOver();
                }
                else
                {
                    LandShape();
                }

            }

        }
        else if (Input.GetButtonDown("ToggleRotation"))
            ToggleRotDirection();
        else if (Input.GetButtonDown("Pause"))
            TogglePause();
        else if (Input.GetButtonDown("Hold"))
        {
            Hold();
        }

    }

    private void GameOver()
    {
        m_activeShape.MoveUp();
        m_gameOver = true;
        Debug.LogWarning(m_activeShape + " Shape is over the limit check");

        m_gameOverPanel.SetActive(true);
        PlaySound(m_soundManager.m_gameOverSound, .9f);
        PlaySound(m_soundManager.m_gameOverVocalClip, .9f);
    }

    void LandShape()
    {
        m_activeShape.MoveUp();
        m_gameBoard.StoreShapeInGrid(m_activeShape);
        m_activeShape = m_spawner.SpawnShape();

        if (m_ghost)
            m_ghost.Reset();

        if (m_holder)
            m_holder.m_canRelease = true;

        m_TimeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
        m_TimeToNextKeyDown = Time.time + m_keyRepeatRateDown;
        m_TimeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

        m_gameBoard.ClearAllRows();

        PlaySound(m_soundManager.m_dropSound, .8f);

        if (m_gameBoard.m_completedRows > 0)
        {
            m_scoreManager.ScoreLines(m_gameBoard.m_completedRows);

            if (m_scoreManager.m_didLevelUp)
            {
                PlaySound(m_soundManager.m_levelUpVocalClip, .75f);
                m_dropRateModded = m_dropRate - Mathf.Clamp( ((float)m_scoreManager.m_level - 1) * 0.05f, 0.1f, 1f );
            }
            else
                if (m_gameBoard.m_completedRows > 1)
                PlaySound(m_soundManager.GetRandomClip(m_soundManager.m_vocalClips), .8f);
            
            PlaySound(m_soundManager.m_clearRowSound, .8f);
        }

    }

    public void Restart()
    {
        Debug.Log("Restarted");
        Time.timeScale = (m_isPaused) ? 0 : 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void PlaySound(AudioClip audioClip, float volmultiplier = .8f)
    {
        if (m_soundManager.m_fxEnabled && audioClip)
            AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position, Mathf.Clamp( m_soundManager.m_fxVolume * volmultiplier, 0.05f, 1f ));
    }


    public void ToggleRotDirection()
    {
        m_clockwise = !m_clockwise;
        if (m_rotIconToggle)
            m_rotIconToggle.ToogleIcon(m_clockwise);
    }

    public void TogglePause()
    {
        if (m_gameOver)
        {
            return;
        }

        m_isPaused = !m_isPaused;

        //if (m_isPaused)
        //{
            m_pausePanel.SetActive(m_isPaused);

            if (m_soundManager)
            {
                m_soundManager.m_musicSource.volume = (m_isPaused) ? m_soundManager.m_musicVolume * .25f : m_soundManager.m_musicVolume;
            }

            Time.timeScale = (m_isPaused) ? 0 : 1;
        //}
    }

    public void Hold()
    {
        if (!m_holder)
        {
            return;
        }

        if (!m_holder.m_heldShape)
        {
            m_holder.Catch(m_activeShape);
            m_activeShape = m_spawner.SpawnShape();
            PlaySound(m_soundManager.m_holdClip);
            if (m_ghost)
                m_ghost.Reset();
        }
        else if (m_holder.m_canRelease)
        {
            Shape temp = m_activeShape;
            m_activeShape = m_holder.Release();
            m_activeShape.transform.position = m_spawner.transform.position;
            m_holder.Catch(temp);
            PlaySound(m_soundManager.m_holdClip);
        }
        else
        {
            Debug.LogWarning("GAMECONTROLLER! Wait for cool down!");
            PlaySound(m_soundManager.m_errorSound);
        }

    }
}
