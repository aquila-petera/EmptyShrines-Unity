using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private bool muteBgm;
    [SerializeField]
    private float transitionTime = 0.4f;

    public void OnTriggerEnter(Collider col)
    {
        GameObject other = col.gameObject;
        if (this.enabled && EntityManager.ObjectIsPlayer(other))
        {
            if (muteBgm)
                MusicManager.StopMusic();
            else
                MusicManager.PlayMapBGM(0.4f);

            if (cameraTransform == null)
                CameraManager.UnlockCamera();
            else
                CameraManager.LockCamera(cameraTransform, transitionTime, true, false);

            CameraManager.OnCameraTriggerActivate(this);
        }
    }

    protected void OnDestroy()
    {
        CameraManager.OnCameraTriggerDestroyed(this);
    }
}
