using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public enum SoundType
    {
        MOVE,
        DROP,
        ERROR,
        VOCAL,
        HOLD,
        CLEAR_ROW,
        GAME_OVER,
        GAME_OVER_VOCAL,
        LEVEL_UP
    }
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private bool musicEnabled = true;
        [SerializeField] private bool fxEnabled = true;
        
        [Header("Volume")]
        [Range(0, 1)] [SerializeField] private float fxVolume = .5f;
        [Range(0,1)] [SerializeField] private float musicVolume = .5f;
        
        [Header("Audio Clips")]
        [SerializeField] private AudioClip moveSound;
        [SerializeField] private AudioClip dropSound;
        [SerializeField] private AudioClip holdSound;
        [SerializeField] private AudioClip errorSound;
        [SerializeField] private AudioClip gameOverSound;
        [SerializeField] private AudioClip clearRowSound;
        [SerializeField] private AudioClip[] musicSounds;
        [SerializeField] private AudioClip[] vocalSounds;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioClip levelUpVocalSound;
        [SerializeField] private AudioClip gameOverVocalSound;
        
        [Header("Toggles")]
        [SerializeField] private IconToggle fxIconToggle;
        [SerializeField] private IconToggle musicIconToggle;
        private AudioClip m_randomAudioClip;
        
        public AudioSource MusicSource => musicSource;
        public float MusicVolume => musicVolume;

        private void Start()
        {
            UpdateMusic();
        }

        private AudioClip GetRandomClip(AudioClip[] clips)
        {
            return clips[Random.Range(0, clips.Length)];
        }
        
        private AudioClip GetRandomVocalSound()
        {
            return vocalSounds[Random.Range(0, vocalSounds.Length)];
        }

        public void ToggleMusic()
        {
            musicEnabled = !musicEnabled;
            UpdateMusic();

            if (musicIconToggle)
            {
                musicIconToggle.ToogleIcon(musicEnabled);
            }
        }

        public void ToggleFX()
        {
            fxEnabled = !fxEnabled;
            if (fxIconToggle)
            {
                fxIconToggle.ToogleIcon(fxEnabled);
            }
        }

        private void PlayBackGroundMusic(AudioClip musicCLip)
        {
            if(!musicEnabled || !musicCLip || !musicSource)
                return;

            musicSource.Stop();
            musicSource.clip = musicCLip;
            musicSource.volume = musicVolume;
            musicSource.loop = true;
            musicSource.Play();
        }

        private void UpdateMusic()
        {
            if (musicSource.isPlaying == musicEnabled) 
                return;
            if (musicEnabled)
            {
                m_randomAudioClip = GetRandomClip(musicSounds);
                PlayBackGroundMusic(m_randomAudioClip);
            }
            else
                musicSource.Stop();
        }
        
        public void PlaySound(SoundType type, float volMultiplier = .8f)
        {
            if (!fxEnabled)
                return;
            
            switch (type)
            {
                case SoundType.MOVE: AudioSource.PlayClipAtPoint(moveSound, transform.position, Mathf.Clamp( fxVolume * volMultiplier, 0.05f, 1f ));
                    break;
                case SoundType.DROP: AudioSource.PlayClipAtPoint(dropSound, transform.position, Mathf.Clamp( fxVolume * volMultiplier, 0.05f, 1f ));
                    break;
                case SoundType.ERROR: AudioSource.PlayClipAtPoint(errorSound, transform.position, Mathf.Clamp( fxVolume * volMultiplier, 0.05f, 1f ));
                    break;
                case SoundType.VOCAL: AudioSource.PlayClipAtPoint(GetRandomVocalSound(), transform.position, Mathf.Clamp( fxVolume * volMultiplier, 0.05f, 1f ));
                    break;
                case SoundType.CLEAR_ROW: AudioSource.PlayClipAtPoint(clearRowSound, transform.position, Mathf.Clamp( fxVolume * volMultiplier, 0.05f, 1f ));
                    break;
                case SoundType.GAME_OVER: AudioSource.PlayClipAtPoint(gameOverSound, transform.position, Mathf.Clamp( fxVolume * volMultiplier, 0.05f, 1f ));
                    break;
                case SoundType.LEVEL_UP: AudioSource.PlayClipAtPoint(levelUpVocalSound, transform.position, Mathf.Clamp( fxVolume * volMultiplier, 0.05f, 1f ));
                    break;
                case SoundType.HOLD: AudioSource.PlayClipAtPoint(holdSound, transform.position, Mathf.Clamp( fxVolume * volMultiplier, 0.05f, 1f ));
                    break;
                case SoundType.GAME_OVER_VOCAL: AudioSource.PlayClipAtPoint(gameOverVocalSound, transform.position, Mathf.Clamp( fxVolume * volMultiplier, 0.05f, 1f ));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
