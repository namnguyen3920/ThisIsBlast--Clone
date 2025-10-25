using System.Collections;
using UnityEngine;

public class AudioManager : Singleton_Mono_Method<AudioManager>
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clip")]    
    public AudioClip[] shootSFX;
    public AudioClip applauseSFX;
    public AudioClip pickTurtleSFX;

    [Range(0f, 1f)] public float sfxVolume = 1f;

    private void Start()
    {
        PlayGamePlayBG();
    }
    public void PlayShootSound(Vector3 position)
    {
        PlayRandomAudio(shootSFX, position, sfxVolume);
    }
    public void PlayApplauseSound(Vector3 position)
    {
        PlayAudio(applauseSFX, position, sfxVolume);
    }
    public void PlayPickTurtleSound(Vector3 position)
    {
        PlayAudio(pickTurtleSFX, position, sfxVolume);
    }
    public void PlayGamePlayBG()
    {
        musicSource.Play();
    }
    public void ChangeBGMusic(float endVolume = 0.1f, float duration = 1f)
    {
        StartCoroutine(FadeMusic(endVolume, duration));
    }
    private IEnumerator FadeMusic(float endVolume, float duration)
    {
        float startVolume = musicSource.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, endVolume, t / duration);
            yield return null;
        }
        musicSource.volume = endVolume;
    }
    private void PlayAudio(AudioClip clip, Vector3 position, float volume)
    {
        AudioSource audioSource = Instantiate(sfxSource, position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
    private void PlayRandomAudio(AudioClip[] clip, Vector3 position, float volume)
    {
        int rand = Random.Range(0, clip.Length);
        AudioSource audioSource = Instantiate(sfxSource, position, Quaternion.identity);
        audioSource.clip = clip[rand];
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
}