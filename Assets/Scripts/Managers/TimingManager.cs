using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TimingManager : MonoBehaviour
{
    private static TimingManager instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    public static void ExecuteAfterDelay(float delaySeconds, UnityAction callback)
    {
        instance.StartCoroutine(instance.CallbackAfterDelay(delaySeconds, callback));
    }

    private IEnumerator CallbackAfterDelay(float delaySeconds, UnityAction callback)
    {
        float time = 0;
        while (time < delaySeconds)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        callback.Invoke();
        yield return null;
    }
}
