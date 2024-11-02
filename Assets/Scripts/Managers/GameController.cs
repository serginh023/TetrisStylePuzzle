using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace Managers
{
    public enum Direction { None, Left, Right, Up, Down };

    public class GameController : MonoBehaviour
    {
        #region fields
        [Header("Game Objects")]
        [SerializeField] private Ghost ghost;
        [SerializeField] private Holder holder;
        [SerializeField] private Board gameBoard;
        [SerializeField] private Spawner spawner;
        
        [Header("Panels")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject gameOverPanel;

        [Header("Managers")]
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private ParticlePlayer gameOverFX;
        
        [Header("Movement")]
        [SerializeField] float dropRate = .9f;
        [SerializeField] [Range(0.01f, 1f)] float keyRepeatRateDown = 0.05f;
        [SerializeField] [Range(0.02f, 1f)] float keyRepeatRateRotate = 0.065f;
        [SerializeField] [Range(0.02f, 1f)] float keyRepeatRateLeftRight = 0.065f;

        [Header("Time")]
        [SerializeField] [Range(.05f, 1f)] private float minTimeToDrag = .15f;
        [SerializeField] [Range(.05f, 1f)] private float minTimeToSwipe = .3f;

        //Game Control
        private bool gameOver;
        private bool clockwise = true;
        private bool isPaused;
        private bool didTap;
        
        // private float dropRateModded;
        //Times
        private float timeToDrop;
        private float timeToNextKeyDown;
        private float timeToNextKeyRotate;
        private float timeToNextKeyLeftRight;
        private float timeToNextDrag;
        private float timeToNextSwipe;
        
        //Objects
        private Shape activeShape;
        private IconToggle rotIconToggle;
        private Camera mainCamera;
        
        //Direction
        private Direction dragDirection = Direction.None;
        private Direction swipeDirection = Direction.None;
        #endregion

        #region Monobehaviour
        private void Start()
        {
            mainCamera = Camera.main;
            timeToNextKeyLeftRight = Time.time + keyRepeatRateLeftRight;
            timeToNextKeyDown = Time.time + keyRepeatRateDown;
            timeToNextKeyRotate = Time.time + keyRepeatRateRotate;

            if (!gameBoard)
                Debug.LogWarning("WARNING! There is no board definied!");

            if (!soundManager)
                Debug.LogWarning("WARNING! There is no soundManager definied!");

            if (!scoreManager)
                Debug.LogWarning("WARNING! There is no scoreManager definied!");

            if (!spawner)
                Debug.LogWarning("WARNING! There is no spawner definied!");
            else
            {
                spawner.transform.position = Vectorf.Round(spawner.transform.position);
                if (!activeShape)
                    activeShape = spawner.SpawnShape();
            }

        }

        // Update is called once per frame
        private void Update()
        {
            if (!gameBoard || !spawner || !activeShape || gameOver || !soundManager)
                return;

            PlayerInput();
        }

        private void LateUpdate()
        {
            if (ghost && activeShape)
                ghost.DrawGhost(activeShape, gameBoard);
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
            else if ( (dragDirection == Direction.Right && Time.time > timeToNextDrag)
                      ||
                      (swipeDirection == Direction.Right && Time.time > timeToNextSwipe) )
            {
                MoveRight();
                timeToNextDrag = Time.time + minTimeToDrag;
                timeToNextSwipe = Time.time + minTimeToSwipe;
            }
            else if ( (dragDirection == Direction.Left && Time.time > timeToNextDrag)
                      ||
                      (swipeDirection == Direction.Left && Time.time > timeToNextSwipe) )
            {
                MoveLeft();
                timeToNextDrag = Time.time + minTimeToDrag;
                timeToNextSwipe = Time.time + minTimeToSwipe;
            }
            else if ( didTap || (swipeDirection == Direction.Up && Time.time > timeToNextSwipe) )
            {
                Rotate();
                timeToNextSwipe = Time.time + minTimeToSwipe;
            }
            else if ( (dragDirection == Direction.Down && Time.time > timeToNextDrag)
                      ||
                      (swipeDirection == Direction.Down && Time.time > timeToNextSwipe) )
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

            dragDirection = Direction.None;
            swipeDirection = Direction.None;
            didTap = false;

        }
    
        private void TapHandler(Vector2 swipeMovement)
        {
            didTap = true;
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
            timeToDrop = Time.time + dropRate;
            timeToNextKeyDown = Time.time + keyRepeatRateDown;
            activeShape.MoveDown();

            if (gameBoard.IsValidPosition(activeShape)) return;
            if (gameBoard.IsOverLimit(activeShape))
            {
                GameOver();
            }
            else
            {
                LandShape();
                //TODO Destroy gameObject
                // DestroyShape(m_activeShape.gameObject);
            }
        }

        private void Rotate()
        {
            timeToNextKeyRotate = Time.time + keyRepeatRateRotate;
            activeShape.RotateClockwise(clockwise);
            if (!gameBoard.IsValidPosition(activeShape))
            {
                activeShape.RotateClockwise(!clockwise);
                PlaySound(soundManager.m_errorSound, .8f);
            }
            else
                PlaySound(soundManager.m_moveSound, .8f);
        }

        private void MoveRight()
        {
            timeToNextKeyLeftRight += Time.time + keyRepeatRateLeftRight;
            activeShape.MoveRight();
            if (!gameBoard.IsValidPosition(activeShape))
            {
                activeShape.MoveLeft();
                PlaySound(soundManager.m_errorSound, .8f);
            }
            else
                PlaySound(soundManager.m_moveSound, .8f);
        }

        private void MoveLeft()
        {
            timeToNextKeyLeftRight += Time.time + keyRepeatRateLeftRight;
            activeShape.MoveLeft();
            if (!gameBoard.IsValidPosition(activeShape))
            {
                activeShape.MoveRight();
                PlaySound(soundManager.m_errorSound, .8f);
            }
            else
                PlaySound(soundManager.m_moveSound, .8f);
        }
    
        //Refactor this method
        //1 - land shape
        //2 - move shape
        private void LandShape()
        {
            if (activeShape)
            {
                activeShape.MoveUp();
                gameBoard.StoreShapeInGrid(activeShape);
                activeShape.LandShapeFX();

                if (ghost)
                    ghost.Reset();

                if (holder)
                    holder.CanRelease = true;

                activeShape = spawner.SpawnShape();

                timeToNextKeyLeftRight = Time.time + keyRepeatRateLeftRight;
                timeToNextKeyDown = Time.time + keyRepeatRateDown;
                timeToNextKeyRotate = Time.time + keyRepeatRateRotate;

                gameBoard.ClearAllRows();

                PlaySound(soundManager.m_dropSound, .8f);

                if (gameBoard.CompletedRows > 0)
                {
                    scoreManager.ScoreLines(gameBoard.CompletedRows);

                    if (scoreManager.DidLevelUp)
                    {
                        PlaySound(soundManager.m_levelUpVocalClip, .75f);
                        // dropRateModded = DropRate - Mathf.Clamp(((float)ScoreManager.m_level - 1) * 0.05f, 0.1f, 1f);
                    }
                    else
                    if (gameBoard.CompletedRows > 1)
                        PlaySound(soundManager.GetRandomClip(soundManager.m_vocalClips), .8f);

                    PlaySound(soundManager.m_clearRowSound, .8f);
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
            PlaySound(soundManager.m_gameOverSound, .9f);
            PlaySound(soundManager.m_gameOverVocalClip, .9f);
            StartCoroutine(GameOverRoutine());
        }

        private IEnumerator GameOverRoutine()
        {
            if (gameOverFX)
                gameOverFX.Play();

            yield return new WaitForSeconds(.4f);

            if(gameOverPanel)
                gameOverPanel.SetActive(true);
        }

        public void Restart()
        {
            Debug.Log("Restarted");
            TogglePause();
            var index = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(index);
        }

        public void TogglePause()
        {
            if (gameOver)
                return;

            isPaused = !isPaused;

            pausePanel.SetActive(isPaused);

            if (soundManager)
                soundManager.m_musicSource.volume = (isPaused) ? soundManager.m_musicVolume * .25f : soundManager.m_musicVolume;

            Time.timeScale = (isPaused) ? 0 : 1;
            Debug.Log("Time.timeScale " + Time.timeScale);
        }

        public void Hold()
        {
            if (!holder)
                return;

            if (!holder.HeldShape)
            {
                holder.Catch(activeShape);
                activeShape = spawner.SpawnShape();
                PlaySound(soundManager.m_holdClip);
                if (ghost)
                    ghost.Reset();
            }
            else if (holder.CanRelease)
            {
                var temp = activeShape;
                activeShape = holder.Release();
                activeShape.transform.position = spawner.transform.position;
                holder.Catch(temp);
                PlaySound(soundManager.m_holdClip);
                if (ghost)
                    ghost.Reset();
            }
            else
            {
                Debug.LogWarning("GAMECONTROLLER! Wait for cool down!");
                PlaySound(soundManager.m_errorSound);
            }
        }

        private void PlaySound(AudioClip audioClip, float volmultiplier = .8f)
        {
            if (soundManager.m_fxEnabled && audioClip)
                AudioSource.PlayClipAtPoint(audioClip, mainCamera.transform.position, Mathf.Clamp( soundManager.m_fxVolume * volmultiplier, 0.05f, 1f ));
        }

        private Direction GetDirection(Vector2 swipeMovement)
        {
            var swipeDir = Direction.None;

            //horizontal
            if (Mathf.Abs(swipeMovement.x) > Mathf.Abs(swipeMovement.y))
            {
                swipeDir = (swipeMovement.x >= 0) ? Direction.Right : Direction.Left;
            }
            //vertical
            else
            {
                swipeDir = (swipeMovement.y >= 0) ? Direction.Up : Direction.Down;
            }

            return swipeDir;
        }

        private void DestroyShape(GameObject go)
        {
            Destroy(go);
        }
    
        #endregion

    }
}