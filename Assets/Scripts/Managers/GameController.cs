using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public enum Direction { none, left, right, up, down };

    public class GameController : MonoBehaviour
    {
        [Header("Game Objects")]
        [SerializeField] Ghost Ghost;
        [SerializeField] Board GameBoard;
        [SerializeField] Holder Holder;
        [SerializeField] Spawner Spawner;
        [SerializeField] GameObject[] FXObjects;
        [Header("Managers")]
        [SerializeField] SoundManager SoundManager;
        [SerializeField] ScoreManager ScoreManager;
        [SerializeField] ParticlePlayer GameOverFX;
        [Header("Movement")]
        [SerializeField] float DropRate = .9f;
        [SerializeField] [Range(0.02f, 1f)] float KeyRepeatRateLeftRight = 0.065f;
        [SerializeField] [Range(0.01f, 1f)] float KeyRepeatRateDown = 0.05f;
        [SerializeField] [Range(0.02f, 1f)] float KeyRepeatRateRotate = 0.065f;
        [Header("Time")]
        [SerializeField] [Range(.05f, 1f)] private float MinTimeToDrag = .15f;
        [SerializeField] [Range(.05f, 1f)] private float MinTimeToSwipe = .3f;

        private bool gameOver = false;
        private bool clockwise = true;
        private bool isPaused = false;
        private bool m_didTap = false;
        private float dropRateModded;
        private float timeToDrop = 0f;
        private float timeToNextKeyLeftRight;
        private float timeToNextKeyDown;
        private float timeToNextKeyRotate;
        private float timeToNextDrag;
        private float timeToNextSwipe;
        private Shape activeShape;
        private GameObject gameOverPanel;
        private GameObject pausePanel;
        private IconToggle rotIconToggle;
        private Direction dragDirection = Direction.none;
        private Direction swipeDirection = Direction.none;

        #region Monobehaviour
        private void Start()
        {
            timeToNextKeyLeftRight = Time.time + KeyRepeatRateLeftRight;
            timeToNextKeyDown = Time.time + KeyRepeatRateDown;
            timeToNextKeyRotate = Time.time + KeyRepeatRateRotate;

            if (!GameBoard)
                Debug.LogWarning("WARNING! There is no board definied!");

            if (!SoundManager)
                Debug.LogWarning("WARNING! There is no soundManager definied!");

            if (!ScoreManager)
                Debug.LogWarning("WARNING! There is no scoreManager definied!");

            if (!Spawner)
                Debug.LogWarning("WARNING! There is no spawner definied!");
            else
            {
                Spawner.transform.position = Vectorf.Round(Spawner.transform.position);
                if (!activeShape)
                    activeShape = Spawner.SpawnShape();
            }

        }

        // Update is called once per frame
        private void Update()
        {
            if (!GameBoard || !Spawner || !activeShape || gameOver || !SoundManager)
                return;

            PlayerInput();
        }

        private void LateUpdate()
        {
            if (Ghost && activeShape)
                Ghost.DrawGhost(activeShape, GameBoard);
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
        #endregion

        #region Inputs
        private void PlayerInput()
        {
            #region PC/MAC
            //if(Input.GetKey("right") && Time.time > m_TimeToNextKey || Input.GetKeyDown(KeyCode.RightArrow) ) --> Alternativa, não passa pelo input manager
            if (Input.GetButton("MoveRight") && Time.time > timeToNextKeyLeftRight || Input.GetButtonDown("MoveRight"))// GetButtonDown registra somente o primeiro frame que o botão foi pressionado
                MoveRight();
            else if (Input.GetButton("MoveLeft") && Time.time > timeToNextKeyLeftRight || Input.GetButtonDown("MoveLeft"))
                MoveLeft();
            else if (Input.GetButtonDown("Rotate") && Time.time > timeToNextKeyRotate)
                Rotate();
            else if (Input.GetButton("MoveDown") && (Time.time > timeToNextKeyDown) || (Time.time > timeToDrop)) //segunda verificação é para a mecânica de cair
                MoveDown();
            #endregion

            #region MOBILE
            else if ( (dragDirection == Direction.right && Time.time > timeToNextDrag)
                      ||
                      (swipeDirection == Direction.right && Time.time > timeToNextSwipe) )
            {
                MoveRight();
                timeToNextDrag = Time.time + MinTimeToDrag;
                timeToNextSwipe = Time.time + MinTimeToSwipe;
            }
            else if ( (dragDirection == Direction.left && Time.time > timeToNextDrag)
                      ||
                      (swipeDirection == Direction.left && Time.time > timeToNextSwipe) )
            {
                MoveLeft();
                timeToNextDrag = Time.time + MinTimeToDrag;
                timeToNextSwipe = Time.time + MinTimeToSwipe;
            }
            else if ( m_didTap || (swipeDirection == Direction.up && Time.time > timeToNextSwipe) )
            {
                Rotate();
                timeToNextSwipe = Time.time + MinTimeToSwipe;
            }
            else if ( (dragDirection == Direction.down && Time.time > timeToNextDrag)
                      ||
                      (swipeDirection == Direction.down && Time.time > timeToNextSwipe) )
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

            dragDirection = Direction.none;
            swipeDirection = Direction.none;
            m_didTap = false;

        }
    
        private void TapHandler(Vector2 swipeMovement)
        {
            m_didTap = true;
        }

        private void DragHandler(Vector2 dragMovement)
        {
            dragDirection = GetDirection(dragMovement);
        }

        private void SwipeHandler(Vector2 swipeMovement)
        {
            swipeDirection = GetDirection(swipeMovement);
        }
        #endregion

        #region Movement
        private void MoveDown()
        {
            timeToDrop = Time.time + DropRate;
            timeToNextKeyDown = Time.time + KeyRepeatRateDown;
            activeShape.MoveDown();

            if (!GameBoard.IsValidPosition(activeShape))
                if (GameBoard.IsOverLimit(activeShape))
                    GameOver();
                else
                {
                    LandShape();
                    //TODO Destroy gameObject
                    // DestroyShape(m_activeShape.gameObject);
                }
        }

        private void Rotate()
        {
            timeToNextKeyRotate = Time.time + KeyRepeatRateRotate;
            activeShape.RotateClockwise(clockwise);
            if (!GameBoard.IsValidPosition(activeShape))
            {
                activeShape.RotateClockwise(!clockwise);
                PlaySound(SoundManager.m_errorSound, .8f);
            }
            else
                PlaySound(SoundManager.m_moveSound, .8f);
        }

        private void MoveRight()
        {
            timeToNextKeyLeftRight += Time.time + KeyRepeatRateLeftRight;
            activeShape.MoveRight();
            if (!GameBoard.IsValidPosition(activeShape))
            {
                activeShape.MoveLeft();
                PlaySound(SoundManager.m_errorSound, .8f);
            }
            else
                PlaySound(SoundManager.m_moveSound, .8f);
        }

        private void MoveLeft()
        {
            timeToNextKeyLeftRight += Time.time + KeyRepeatRateLeftRight;
            activeShape.MoveLeft();
            if (!GameBoard.IsValidPosition(activeShape))
            {
                activeShape.MoveRight();
                PlaySound(SoundManager.m_errorSound, .8f);
            }
            else
                PlaySound(SoundManager.m_moveSound, .8f);
        }
    
        //Refacture this method
        //1 - land shape
        //2 - move shape
        private void LandShape()
        {
            if (activeShape)
            {

                activeShape.MoveUp();
                GameBoard.StoreShapeInGrid(activeShape);
                activeShape.LandShapeFX();

                if (Ghost)
                    Ghost.Reset();

                if (Holder)
                    Holder.m_canRelease = true;

                activeShape = Spawner.SpawnShape();

                timeToNextKeyLeftRight = Time.time + KeyRepeatRateLeftRight;
                timeToNextKeyDown = Time.time + KeyRepeatRateDown;
                timeToNextKeyRotate = Time.time + KeyRepeatRateRotate;

                GameBoard.StartCoroutine("ClearAllRows");

                PlaySound(SoundManager.m_dropSound, .8f);

                if (GameBoard.m_completedRows > 0)
                {
                    ScoreManager.ScoreLines(GameBoard.m_completedRows);

                    if (ScoreManager.m_didLevelUp)
                    {
                        PlaySound(SoundManager.m_levelUpVocalClip, .75f);
                        dropRateModded = DropRate - Mathf.Clamp(((float)ScoreManager.m_level - 1) * 0.05f, 0.1f, 1f);
                    }
                    else
                    if (GameBoard.m_completedRows > 1)
                        PlaySound(SoundManager.GetRandomClip(SoundManager.m_vocalClips), .8f);

                    PlaySound(SoundManager.m_clearRowSound, .8f);
                }

            }

        }

        public void ToggleRotDirection()
        {
            clockwise = !clockwise;
            if (rotIconToggle)
                rotIconToggle.ToogleIcon(clockwise);
        }
        #endregion

        #region Game Control
    
        private void GameOver()
        {
            activeShape.MoveUp();
            gameOver = true;
            Debug.LogWarning(activeShape + " Shape is over the limit check");
            PlaySound(SoundManager.m_gameOverSound, .9f);
            PlaySound(SoundManager.m_gameOverVocalClip, .9f);
            StartCoroutine(GameOverRoutine());
        }

        private IEnumerator GameOverRoutine()
        {
            if (GameOverFX)
                GameOverFX.Play();

            yield return new WaitForSeconds(.4f);

            if(gameOverPanel)
                gameOverPanel.SetActive(true);
        }

        public void Restart()
        {
            Debug.Log("Restarted");
            TogglePause();
            int index = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(index);
        }

        public void TogglePause()
        {
            if (gameOver)
                return;

            isPaused = !isPaused;

            pausePanel.SetActive(isPaused);

            if (SoundManager)
                SoundManager.m_musicSource.volume = (isPaused) ? SoundManager.m_musicVolume * .25f : SoundManager.m_musicVolume;

            Time.timeScale = (isPaused) ? 0 : 1;
            Debug.Log("Time.timeScale " + Time.timeScale);

        }

        public void Hold()
        {
            if (!Holder)
                return;

            if (!Holder.m_heldShape)
            {
                Holder.Catch(activeShape);
                activeShape = Spawner.SpawnShape();
                PlaySound(SoundManager.m_holdClip);
                if (Ghost)
                    Ghost.Reset();
            }
            else if (Holder.m_canRelease)
            {
                Shape temp = activeShape;
                activeShape = Holder.Release();
                activeShape.transform.position = Spawner.transform.position;
                Holder.Catch(temp);
                PlaySound(SoundManager.m_holdClip);
                if (Ghost)
                    Ghost.Reset();
            }
            else
            {
                Debug.LogWarning("GAMECONTROLLER! Wait for cool down!");
                PlaySound(SoundManager.m_errorSound);
            }

        }

        private void PlaySound(AudioClip audioClip, float volmultiplier = .8f)
        {
            if (SoundManager.m_fxEnabled && audioClip)
                AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position, Mathf.Clamp( SoundManager.m_fxVolume * volmultiplier, 0.05f, 1f ));
        }

        private Direction GetDirection(Vector2 swipeMovement)
        {
            var swipeDir = Direction.none;

            //horizontal
            if (Mathf.Abs(swipeMovement.x) > Mathf.Abs(swipeMovement.y))
                swipeDir = (swipeMovement.x >= 0) ? Direction.right : Direction.left;
            //vertical
            else
                swipeDir = (swipeMovement.y >= 0) ? Direction.up : Direction.down;

            return swipeDir;
        }

        private void DestroyShape(GameObject go)
        {
            Destroy(go);
        }
    
        #endregion

    }
}