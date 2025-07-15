using System;
using UnityEngine;

[System.Serializable]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public enum MusicType
    {
        
    }
    public enum SFXType
    {

    }
    
    public Music[] musicSounds;
    public SFX[] sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //PlayMusic(MusicType.Theme);
    }

    public void PlayMusic(MusicType type)
    {
        Music sound = Array.Find(musicSounds, x => x.type == type);

        if (sound == null)
        {
            Debug.Log("[ Sound Not Found ] or [ the types are different ]");
        }
        else
        {
            musicSource.clip = sound.clip;
            musicSource.Play();
        }
    }
    public void StopMusic(MusicType type)
    {
        Music sound = Array.Find(musicSounds, x => x.type == type);
        if (sound == null)
        {
            Debug.Log("[ Sound Not Found ] or [ the types are different ]");
        }
        else
        {
            musicSource.clip = sound.clip;
            musicSource.Stop();
        }
    }
    public void PlaySFX(SFXType type)
    {
        SFX sound = Array.Find(sfxSounds, x => x.type == type);

        if (sound == null)
        {
            Debug.Log("[ Sound Not Found ] or [ the types are different ]");
        }
        else
        {
            sfxSource.PlayOneShot(sound.clip);
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicSource.mute = false;
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.mute = false;
        sfxSource.volume = volume;
    }
}
