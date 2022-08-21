using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintNote : MonoBehaviour
{
    [SerializeField]
    private TMP_Text hintText;

    GameObject player;
    float startY;

    public void OnTriggerEnter(Collider col)
    {
        GameObject other = col.gameObject;
        if (EntityManager.ObjectIsPlayer(other))
        {
            StopAllCoroutines();
            StartCoroutine(ShowHint());
        }
    }

    public void OnTriggerExit(Collider col)
    {
        GameObject other = col.gameObject;
        if (EntityManager.ObjectIsPlayer(other))
        {
            StopAllCoroutines();
            StartCoroutine(HideHint());
        }
    }

    private void Start()
    {
        startY = hintText.transform.position.y;
    }

    private IEnumerator ShowHint()
    {
        hintText.gameObject.SetActive(true);
        float param = 0;
        while (param < 1)
        {
            param += Time.deltaTime * 4;
            hintText.transform.position = new Vector3(hintText.transform.position.x, startY + param / 10f, hintText.transform.position.z);
            hintText.alpha = param;
            foreach (SpriteRenderer sprite in hintText.GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, param);
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    private IEnumerator HideHint()
    {
        float param = 1;
        while (param > 0)
        {
            param -= Time.deltaTime * 4;
            hintText.transform.position = new Vector3(hintText.transform.position.x, startY + param / 10f, hintText.transform.position.z);
            hintText.alpha = param;
            foreach (SpriteRenderer sprite in hintText.GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, param);
            }
            yield return new WaitForEndOfFrame();
        }
        hintText.gameObject.SetActive(false);
        yield return null;
    }
}
