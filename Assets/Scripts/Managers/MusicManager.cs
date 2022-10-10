using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private static AudioClip mapBGM;

    [SerializeField]
    private AudioClip musicClip;

    private AudioSource musicSource;

    private void Awake()
    {
        mapBGM = musicClip;
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        musicSource = GetComponent<AudioSource>();
        PlayMapBGM();
    }

    private IEnumerator FadeMusicIn(AudioClip music, float time = 0f)
    {
        musicSource.clip = music;
        musicSource.Play();

        if (time == 0)
        {
            musicSource.volume = 1;
            yield break;
        }

        musicSource.volume = 0;
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Clamp(t / time, 0, 1);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    private IEnumerator FadeMusicOut(float time = 0.5f)
    {
        if (time == 0)
        {
            musicSource.Stop();
            yield break;
        }

        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime;
            musicSource.volume = Mathf.Clamp(t / time, 0, 1);
            yield return new WaitForEndOfFrame();
        }
        musicSource.Stop();
        yield return null;
    }

    private IEnumerator SwitchMusicTrack(AudioClip music, float fadeInTime = 0.4f, float fadeOutTime = 0.4f)
    {
        if (musicSource.isPlaying)
        {
            if (musicSource.clip == music)
                yield break;

            yield return FadeMusicOut(fadeOutTime);
        }

        yield return FadeMusicIn(music, fadeInTime);
    }

    public static void PlayCustomBGM(AudioClip music, float fadeInTime = 0, float fadeOutTime = 0, bool loop = true)
    {
        instance.musicSource.loop = loop;
        instance.StartCoroutine(instance.SwitchMusicTrack(music, fadeInTime, fadeOutTime));
    }

    public static void PlayMapBGM(float fadeInTime = 0, float fadeOutTime = 0)
    {
        instance.musicSource.loop = true;
        instance.StartCoroutine(instance.SwitchMusicTrack(mapBGM, fadeInTime, fadeOutTime));
    }

    public static void StopMusic()
    {
        instance.StartCoroutine(instance.FadeMusicOut());
    }
}
