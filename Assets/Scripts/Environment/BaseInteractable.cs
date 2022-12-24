using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BaseInteractable : MonoBehaviour, IInteractable
{
    [SerializeField]
    protected TMP_Text hintText;

    protected CharacterMovement player;

    public void OnTriggerEnter(Collider col)
    {
        GameObject other = col.gameObject;
        if (EntityManager.ObjectIsPlayer(other))
        {
            player = other.GetComponent<CharacterMovement>();
            player.SetInteractable(this);
            StopAllCoroutines();
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
            StopAllCoroutines();
            StartCoroutine(HideHint());
        }
    }

    protected IEnumerator ShowHint()
    {
        hintText.gameObject.SetActive(true);
        while (hintText.alpha < 1)
        {
            hintText.alpha += Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    protected IEnumerator HideHint()
    {
        while (hintText.alpha > 0)
        {
            hintText.alpha -= Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }
        hintText.gameObject.SetActive(false);
        yield return null;
    }

    public virtual void OnInteract()
    {
        throw new System.NotImplementedException();
    }
}
