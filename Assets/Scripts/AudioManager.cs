using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clipss")]
    public AudioClip backgroundMusic;
    public AudioClip dartThrowClip;
    public AudioClip dartHitClip;
    public AudioClip monsterDeathClip;
    public AudioClip bullseyeHitClip;
    public AudioClip gameOverClip;

    private Dictionary<string, AudioClip> sfxClips;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxClips = new Dictionary<string, AudioClip>
        {
            { "Throw", dartThrowClip },
            { "Hit", dartHitClip },
            { "MonsterDeath", monsterDeathClip },
            { "BullseyeHit", bullseyeHitClip },
            { "GameOver", gameOverClip }
        };
    }

    void Start()
    {
        PlayMusic(backgroundMusic);
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicSource != null && musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        if (sfxClips.ContainsKey(name) && sfxClips[name] != null)
        {
            sfxSource.PlayOneShot(sfxClips[name]);
        }
        else
        {
            Debug.LogWarning($"SFX '{name}' not found in AudioManager."); 
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume; 
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume; 
    }
}
