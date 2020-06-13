using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    [SerializeField]
    GameObject[] FXObjects;

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

    [SerializeField]
    ParticlePlayer m_gameOverFX;

    enum Direction { none, left, right, up, down };

    Direction m_dragDirection = Direction.none;
    Direction m_swipeDirection = Direction.none;

    float m_timeToNextDrag;
    float m_timeToNextSwipe;

    [SerializeField]
    [Range(.05f, 1f)] 
    float m_minTimeToDrag = .15f;

    [SerializeField]
    [Range(.05f, 1f)]
    float m_minTimeToSwipe = .3f;

    bool m_didTap = false;

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
                m_activeShape = m_spawner.SpawnShape();
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
            m_ghost.DrawGhost(m_activeShape, m_gameBoard);

    }

    void PlayerInput()
    {
        #region PC/MAC
        //if(Input.GetKey("right") && Time.time > m_TimeToNextKey || Input.GetKeyDown(KeyCode.RightArrow) ) --> Alternativa, não passa pelo input manager
        if (Input.GetButton("MoveRight") && Time.time > m_TimeToNextKeyLeftRight || Input.GetButtonDown("MoveRight"))// GetButtonDown registra somente o primeiro frame que o botão foi pressionado
            MoveRight();
        else if (Input.GetButton("MoveLeft") && Time.time > m_TimeToNextKeyLeftRight || Input.GetButtonDown("MoveLeft"))
            MoveLeft();
        else if (Input.GetButtonDown("Rotate") && Time.time > m_TimeToNextKeyRotate)
            Rotate();
        else if (Input.GetButton("MoveDown") && (Time.time > m_TimeToNextKeyDown) || (Time.time > m_timeToDrop)) //segunda verificação é para a mecânica de cair
            MoveDown();
        #endregion

        #region MOBILE
        else if ( (m_dragDirection == Direction.right && Time.time > m_timeToNextDrag)
            ||
                (m_swipeDirection == Direction.right && Time.time > m_timeToNextSwipe) )
        {
            MoveRight();
            m_timeToNextDrag = Time.time + m_minTimeToDrag;
            m_timeToNextSwipe = Time.time + m_minTimeToSwipe;
        }
        else if ( (m_dragDirection == Direction.left && Time.time > m_timeToNextDrag)
            ||
                (m_swipeDirection == Direction.left && Time.time > m_timeToNextSwipe) )
        {
            MoveLeft();
            m_timeToNextDrag = Time.time + m_minTimeToDrag;
            m_timeToNextSwipe = Time.time + m_minTimeToSwipe;
        }
        else if ( m_didTap || (m_swipeDirection == Direction.up && Time.time > m_timeToNextSwipe) )
        {
            Rotate();
            m_timeToNextSwipe = Time.time + m_minTimeToSwipe;
        }
        else if ( (m_dragDirection == Direction.down && Time.time > m_timeToNextDrag)
            ||
                (m_swipeDirection == Direction.down && Time.time > m_timeToNextSwipe) )
        {
            MoveDown();
        }
        #endregion

        else if (Input.GetButtonDown("ToggleRotation"))
            ToggleRotDirection();
        else if (Input.GetButtonDown("Pause"))
            TogglePause();
        else if (Input.GetButtonDown("Hold"))
            Hold();

        m_dragDirection = Direction.none;
        m_swipeDirection = Direction.none;
        m_didTap = false;

    }

    private void MoveDown()
    {
        m_timeToDrop = Time.time + m_dropRate;
        m_TimeToNextKeyDown = Time.time + m_keyRepeatRateDown;
        m_activeShape.MoveDown();

        if (!m_gameBoard.IsValidPosition(m_activeShape))
            if (m_gameBoard.IsOverLimit(m_activeShape))
                GameOver();
            else
                LandShape();
    }

    private void Rotate()
    {
        m_TimeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;
        m_activeShape.RotateClockwise(m_clockwise);
        if (!m_gameBoard.IsValidPosition(m_activeShape))
        {
            m_activeShape.RotateClockwise(!m_clockwise);
            PlaySound(m_soundManager.m_errorSound, .8f);
        }
        else
            PlaySound(m_soundManager.m_moveSound, .8f);
    }

    void MoveRight()
    {
        m_TimeToNextKeyLeftRight += Time.time + m_keyRepeatRateLeftRight;
        m_activeShape.MoveRight();
        if (!m_gameBoard.IsValidPosition(m_activeShape))
        {
            m_activeShape.MoveLeft();
            PlaySound(m_soundManager.m_errorSound, .8f);
        }
        else
            PlaySound(m_soundManager.m_moveSound, .8f);
    }

    private void MoveLeft()
    {
        m_TimeToNextKeyLeftRight += Time.time + m_keyRepeatRateLeftRight;
        m_activeShape.MoveLeft();
        if (!m_gameBoard.IsValidPosition(m_activeShape))
        {
            m_activeShape.MoveRight();
            PlaySound(m_soundManager.m_errorSound, .8f);
        }
        else
            PlaySound(m_soundManager.m_moveSound, .8f);
    }

    private void GameOver()
    {
        m_activeShape.MoveUp();
        m_gameOver = true;
        Debug.LogWarning(m_activeShape + " Shape is over the limit check");
        PlaySound(m_soundManager.m_gameOverSound, .9f);
        PlaySound(m_soundManager.m_gameOverVocalClip, .9f);
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        if (m_gameOverFX)
            m_gameOverFX.Play();

        yield return new WaitForSeconds(.4f);

        if(m_gameOverPanel)
            m_gameOverPanel.SetActive(true);
    }

    void LandShape()
    {
        if (m_activeShape)
        {

            m_activeShape.MoveUp();
            m_gameBoard.StoreShapeInGrid(m_activeShape);
            m_activeShape.LandShapeFX();

            if (m_ghost)
                m_ghost.Reset();

            if (m_holder)
                m_holder.m_canRelease = true;

            m_activeShape = m_spawner.SpawnShape();

            m_TimeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
            m_TimeToNextKeyDown = Time.time + m_keyRepeatRateDown;
            m_TimeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

            m_gameBoard.StartCoroutine("ClearAllRows");

            PlaySound(m_soundManager.m_dropSound, .8f);

            if (m_gameBoard.m_completedRows > 0)
            {
                m_scoreManager.ScoreLines(m_gameBoard.m_completedRows);

                if (m_scoreManager.m_didLevelUp)
                {
                    PlaySound(m_soundManager.m_levelUpVocalClip, .75f);
                    m_dropRateModded = m_dropRate - Mathf.Clamp(((float)m_scoreManager.m_level - 1) * 0.05f, 0.1f, 1f);
                }
                else
                    if (m_gameBoard.m_completedRows > 1)
                    PlaySound(m_soundManager.GetRandomClip(m_soundManager.m_vocalClips), .8f);

                PlaySound(m_soundManager.m_clearRowSound, .8f);
            }

        }

    }

    public void Restart()
    {
        Debug.Log("Restarted");
        TogglePause();
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index);
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
            return;

        m_isPaused = !m_isPaused;

        m_pausePanel.SetActive(m_isPaused);

        if (m_soundManager)
            m_soundManager.m_musicSource.volume = (m_isPaused) ? m_soundManager.m_musicVolume * .25f : m_soundManager.m_musicVolume;

        Time.timeScale = (m_isPaused) ? 0 : 1;
        Debug.Log("Time.timeScale " + Time.timeScale);

    }

    public void Hold()
    {
        if (!m_holder)
            return;

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
            if (m_ghost)
                m_ghost.Reset();
        }
        else
        {
            Debug.LogWarning("GAMECONTROLLER! Wait for cool down!");
            PlaySound(m_soundManager.m_errorSound);
        }

    }

    void TapHandler(Vector2 swipeMovement)
    {
        m_didTap = true;
    }

    void DragHandler(Vector2 dragMovement)
    {
        m_dragDirection = GetDirection(dragMovement);
    }

    void SwipeHandler(Vector2 swipeMovement)
    {
        m_swipeDirection = GetDirection(swipeMovement);
    }

    Direction GetDirection(Vector2 swipeMovement)
    {
        Direction swipeDir = Direction.none;

        //horizontal
        if (Mathf.Abs(swipeMovement.x) > Mathf.Abs(swipeMovement.y))
            swipeDir = (swipeMovement.x >= 0) ? Direction.right : Direction.left;
        //vertical
        else
            swipeDir = (swipeMovement.y >= 0) ? Direction.up : Direction.down;

        return swipeDir;
    }

    private void OnEnable()
    {
        TouchManager.DragEvent += DragHandler;
        TouchManager.SwipeEvent += SwipeHandler;
        TouchManager.TapEvent += TapHandler;
    }

    private void OnDisable()
    {
        TouchManager.DragEvent -= DragHandler;
        TouchManager.SwipeEvent -= SwipeHandler;
        TouchManager.TapEvent -= TapHandler;
    }

}
