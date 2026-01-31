using UnityEngine;
using System; // Required for Array.Find

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Libraries")]
    public AudioClip[] listMusicClips;
    public AudioClip[] listSfxClips;

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

    void Start()
    {
        this.PlayMusic("Bgm");
    }

    public void PlayMusic(string name)
    {
        // 1. Find the clip in the Music array
        AudioClip clip = Array.Find(listMusicClips, x => x.name == name);

        if (clip == null)
        {
            Debug.LogError("SoundManager: Music not found with name: " + name);
            return;
        }

        // 2. Play the clip (same logic as before)
        if (musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(string name, float volumeScale = 1f)
    {
        AudioClip clip = Array.Find(listSfxClips, x => x.name == name);

        if (clip == null)
        {
            Debug.LogError("SoundManager: SFX not found with name: " + name);
            return;
        }

        // 2. Play one shot
        sfxSource.PlayOneShot(clip, volumeScale);
    }

    public void StopMusic()
    {
        musicSource.Stop();
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