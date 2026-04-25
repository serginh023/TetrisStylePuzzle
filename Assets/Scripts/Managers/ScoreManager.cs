using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Managers
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private Text linesText;
        [SerializeField] private Text levelText;
        [SerializeField] private Text scoreText;
        
        private int score;
        private int lines;
        private int level;
        private int linesPerLevel = 5;
        private const int m_minLines = 1;
        private const int m_maxLines = 4;

        private bool didLevelUp;

        public bool DidLevelUp => didLevelUp;

        [SerializeField] ParticlePlayer m_levelUpFX;

        private void Start()
        {
            Reset();
        }

        public void ScoreLines(int n)
        {
            didLevelUp = false;
            n = Mathf.Clamp(n, m_minLines, m_maxLines);

            switch (n)
            {
                case 1:
                    score += 40 * level;
                    break;
                case 2:
                    score += 100 * level;
                    break;
                case 3:
                    score += 300 * level;
                    break;
                case 4:
                    score += 1200 * level;
                    break;
                default:
                    break;
            }
            lines -= n;
            if(lines <= 0)
                LevelUp();
            UpdateUserInterface();
        }

        public void Reset()
        {
            level = 1;
            lines = linesPerLevel * level;
            UpdateUserInterface();
        }
    
        private void UpdateUserInterface()
        {
            if (linesText)
                linesText.text = lines.ToString();
            if (levelText)
                levelText.text = level.ToString();
            if (scoreText)
                scoreText.text = PadZero(score, 7);
        
            Debug.Log("score: " + score);
        }

        private string PadZero(int n, int padDigits)
        {
            var str = n.ToString();

            while(str.Length < padDigits)
                str = "0" + str;

            return str;
        }

        private void LevelUp()
        {
            level++;
            lines = linesPerLevel * level;
            didLevelUp = true;
            m_levelUpFX.Play();
        }
    }
}
