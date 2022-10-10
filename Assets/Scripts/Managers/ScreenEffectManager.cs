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
        FLASH,
        TUTORIAL_IN,
        TUTORIAL_OUT
    }

    private static ScreenEffectManager instance;

    [SerializeField]
    private Image overlay;
    [SerializeField]
    private CanvasGroup tutorialText;

    private bool showingTutorial;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (showingTutorial && Input.GetKeyDown(KeyCode.E))
        {
            HideTutorial();
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public static void ShowTutorial()
    {
        instance.StartCoroutine(instance.DoFade(Color.black, 1, EffectType.TUTORIAL_IN));
        TimingManager.ExecuteAfterDelay(3, instance.AllowTutorialDismiss);
        EntityManager.SetPlayerControlEnabled(false);
    }

    public static void HideTutorial()
    {
        instance.StartCoroutine(instance.DoFade(Color.black, 1, EffectType.TUTORIAL_OUT));
        instance.showingTutorial = false;
        EntityManager.SetPlayerControlEnabled(true);
    }

    private void AllowTutorialDismiss()
    {
        showingTutorial = true;
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
        float timer = 0, targetAlpha = 1;
        col.a = type == EffectType.FADE_IN || type == EffectType.TUTORIAL_OUT ? 1 : 0;
        if (type == EffectType.TUTORIAL_IN | type == EffectType.TUTORIAL_OUT)
        {
            targetAlpha = 0.5f;
        }
        overlay.color = col;
        while (timer < duration)
        {
            col.a = Mathf.Lerp(0, targetAlpha, type == EffectType.FADE_IN || type == EffectType.TUTORIAL_OUT ? (1 - timer / duration) : timer / duration);
            timer += Time.deltaTime;
            overlay.color = col;

            if (type == EffectType.TUTORIAL_IN)
            {
                tutorialText.alpha = Mathf.Lerp(0, 1, timer / duration);
            }
            else if (type == EffectType.TUTORIAL_OUT)
            {

                tutorialText.alpha = Mathf.Lerp(1, 0, timer / duration);
            }

            yield return new WaitForEndOfFrame();
        }
        col.a = type == EffectType.FADE_IN || type == EffectType.TUTORIAL_OUT ? 0 : targetAlpha;
        overlay.color = col;
        if (type == EffectType.FLASH)
        {
            yield return DoFade(col, duration, EffectType.FADE_IN);
        }
        yield return null;
    }
}
