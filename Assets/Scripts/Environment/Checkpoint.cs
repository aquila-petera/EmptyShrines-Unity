using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    protected TMP_Text hintText;

    private CharacterMovement player;
    private int spawnPointIndex;

    public void OnTriggerEnter(Collider col)
    {
        GameObject other = col.gameObject;
        if (EntityManager.ObjectIsPlayer(other))
        {
            player = other.GetComponent<CharacterMovement>();
            player.SetInteractable(this);
            StartCoroutine(ShowHint());
        }
    }

    public void OnTriggerExit(Collider col)
    {
        GameObject other = col.gameObject;
        if (EntityManager.ObjectIsPlayer(other))
        {
            player.SetInteractable(null);
            player = null;
            StartCoroutine(HideHint());
        }
    }

    private void Start()
    {
        EntranceManager em = FindObjectOfType<EntranceManager>();
        spawnPointIndex = em.entrances.Count;
        em.entrances.Add(spawnPoint);
    }

    public void OnInteract()
    {
        EntityManager.SetRespawnData(SceneManager.GetActiveScene().name, spawnPointIndex);
        player.AutoChannel(1);
        player.FullHeal();
        ParticleManager.PlayParticleSystemAtPosition("Dots", transform.position, Color.yellow);
        ScreenEffectManager.FlashScreen(new Color32(0xAD, 0xAA, 0x0F, 0xFF), 1);
    }

    private IEnumerator ShowHint()
    {
        hintText.gameObject.SetActive(true);
        while (hintText.alpha < 1)
        {
            hintText.alpha += Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    private IEnumerator HideHint()
    {
        while (hintText.alpha > 0)
        {
            hintText.alpha -= Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }
        hintText.gameObject.SetActive(false);
        yield return null;
    }
}
