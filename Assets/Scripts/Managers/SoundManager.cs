using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public bool m_musicEnabled = true;
    public bool m_fxEnabled = true;
    [Range(0,1)]
    public float m_musicVolume = .5f;
    [Range(0, 1)]
    public float m_fxVolume = .5f;

    public AudioClip m_clearRowSound;

    public AudioClip m_moveSound;

    public AudioClip m_dropSound;

    public AudioClip m_gameOverSound;

    public AudioClip m_errorSound;

    public AudioSource m_musicSource;

    public AudioClip[] m_musiClips;

    AudioClip m_randomAudioClip;

    public AudioClip[] m_vocalClips;

    public AudioClip m_gameOverVocalClip;

    public AudioClip m_levelUpVocalClip;

    public IconToggle m_fxIconToogle;

    public IconToggle m_musicIconToogle;


    // Start is called before the first frame update
    void Start()
    {
        UpdateMusic();
    }

    public AudioClip GetRandomClip(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length)];
    }

    public void ToogleMusic()
    {
        m_musicEnabled = !m_musicEnabled;
        UpdateMusic();

        if (m_musicIconToogle)
        {
            m_musicIconToogle.ToogleIcon(m_musicEnabled);
        }
    }

    public void ToogleFX()
    {
        m_fxEnabled = !m_fxEnabled;
        if (m_fxIconToogle)
        {
            m_fxIconToogle.ToogleIcon(m_fxEnabled);
        }
    }

    void PlayBackGroundMusic(AudioClip musicCLip)
    {
        if(!m_musicEnabled || !musicCLip || !m_musicSource)
            return;

        m_musicSource.Stop();

        m_musicSource.clip = musicCLip;

        m_musicSource.volume = m_musicVolume;

        m_musicSource.loop = true;

        m_musicSource.Play();
    }

    void UpdateMusic()
    {
        if(m_musicSource.isPlaying != m_musicEnabled)
        {
            if (m_musicEnabled)
            {
                m_randomAudioClip = GetRandomClip(m_musiClips);
                PlayBackGroundMusic(m_randomAudioClip);
            }
            else
                m_musicSource.Stop();
        }
    }
}
