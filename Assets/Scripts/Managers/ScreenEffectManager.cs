using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenEffectManager : MonoBehaviour
{
    public enum EffectType
    {
        FADE_IN,
        FADE_OUT,
        FLASH
    }

    private static ScreenEffectManager instance;

    private Image overlay;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
        DontDestroyOnLoad(gameObject);
        overlay = GetComponentInChildren<Image>();
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public static void FadeScreen(Color col, float duration, EffectType type = EffectType.FADE_OUT)
    {
        instance.StartCoroutine(instance.DoFade(col, duration, type));
    }

    public static void FlashScreen(Color col, float duration)
    {
        instance.StartCoroutine(instance.DoFade(col, duration / 2, EffectType.FLASH));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FadeScreen(Color.black, 0.5f, EffectType.FADE_IN);
    }

    private IEnumerator DoFade(Color col, float duration, EffectType type)
    {
        float timer = 0;
        col.a = type == EffectType.FADE_IN ? 1 : 0;
        overlay.color = col;
        while (timer < duration)
        {
            col.a = Mathf.Lerp(0, 1, type == EffectType.FADE_IN ? (1 - timer / duration) : timer / duration);
            timer += Time.deltaTime;
            overlay.color = col;
            yield return new WaitForEndOfFrame();
        }
        col.a = type == EffectType.FADE_IN ? 0 : 1;
        overlay.color = col;
        if (type == EffectType.FLASH)
        {
            yield return DoFade(col, duration, EffectType.FADE_IN);
        }
        yield return null;
    }
}
