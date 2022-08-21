using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;

    private static Vector3 defaultCameraPosition;
    private static Quaternion defaultCameraRotation;

    private List<CameraTrigger> cameraTriggers;

    private void Awake()
    {
        defaultCameraPosition = Camera.main.transform.localPosition;
        defaultCameraRotation = Camera.main.transform.rotation;
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
        cameraTriggers = new List<CameraTrigger>();
    }

    private void Start()
    {
        foreach (var ct in FindObjectsOfType<CameraTrigger>())
        {
            cameraTriggers.Add(ct);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        cameraTriggers.Clear();
        foreach (var ct in FindObjectsOfType<CameraTrigger>())
        {
            cameraTriggers.Add(ct);
        }
    }

    public static void LockCamera(Transform cameraNode, float transitionTime = 0.4f, bool lerpRotation = false, bool lockGlobalPosition = true)
    {
        if (lockGlobalPosition)
            Camera.main.transform.SetParent(instance.transform);
        instance.StartCoroutine(instance.LerpCameraPosition(cameraNode, transitionTime));
        if (lerpRotation)
            instance.StartCoroutine(instance.LerpCameraRotation(cameraNode.rotation, transitionTime));
    }

    public static void UnlockCamera()
    {
        EntityManager.ParentObjectToPlayer(Camera.main.gameObject);
        instance.StartCoroutine(instance.ResetCameraPosition(0.4f));
    }

    public static void OnCameraTriggerActivate(CameraTrigger trigger)
    {
        foreach (var ct in instance.cameraTriggers)
        {
            if (ct != trigger)
            {
                ct.enabled = true;
            }
            trigger.enabled = false;
        }
    }

    public static void OnCameraTriggerDestroyed(CameraTrigger trigger)
    {
        instance.cameraTriggers.Remove(trigger);
    }

    private IEnumerator LerpCameraPosition(Transform newTransform, float time)
    {
        float timer = 0;
        Vector3 startPos = Camera.main.transform.position;
        Vector3 goalPos = newTransform.position;
        while (timer < time)
        {
            if (newTransform.parent != null)
                goalPos = newTransform.parent.position + newTransform.localPosition;
            Camera.main.transform.position = Vector3.Slerp(startPos, goalPos, timer / time);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    private IEnumerator LerpCameraRotation(Quaternion newRotation, float time)
    {
        float timer = 0;
        Quaternion startRot = Camera.main.transform.rotation;
        while (timer < time)
        {
            Camera.main.transform.rotation = Quaternion.Slerp(startRot, newRotation, timer / time);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    private IEnumerator ResetCameraPosition(float time)
    {
        float timer = 0;
        Vector3 startPos = Camera.main.transform.position;
        Quaternion startRot = Camera.main.transform.rotation;
        while (timer < time)
        {
            Vector3 goalPos = EntityManager.GetPlayerPosition() + defaultCameraPosition;
            Camera.main.transform.position = Vector3.Slerp(startPos, goalPos, timer / time);
            Camera.main.transform.rotation = Quaternion.Slerp(startRot, defaultCameraRotation, timer / time);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
