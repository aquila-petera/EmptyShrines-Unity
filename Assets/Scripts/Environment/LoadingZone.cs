using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingZone : MonoBehaviour
{
    [SerializeField]
    private string destination;
    [SerializeField]
    private int entranceIndex;

    private bool loading = false;

    public void OnTriggerEnter(Collider col)
    {
        GameObject other = col.gameObject;
        if (!EntityManager.ObjectIsPlayer(other) || loading)
            return;

        var player = other.GetComponent<CharacterMovement>();
        if (!player.HasControl())
            return;

        loading = true;
        player.AutoMove(transform.parent.rotation.y == 0);
        EntityManager.SetSceneEntrance(entranceIndex);
        ScreenEffectManager.FadeScreen(Color.black, 0.5f);
        TimingManager.ExecuteAfterDelay(0.5f, LoadScene);
        EntityManager.RecordPlayerHp();
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(destination);
    }
}
