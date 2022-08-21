using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCBase : MonoBehaviour, IInteractable
{
    [SerializeField]
    List<string> dialogue;
    [SerializeField]
    GameObject speechBubble;
    [SerializeField]
    TMP_Text speechText;
    [SerializeField]
    TMP_Text hintText;

    private Animator animator;
    private float startY;
    private int dialogueIdx;
    private CharacterMovement player;
    private SpriteFlipper spriteFlipper;
    private bool transition;

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
        if (player != null && EntityManager.ObjectIsPlayer(other))
        {
            player.SetInteractable(null);
            player = null;
            StopAllCoroutines();
            StartCoroutine(HideText());
            StartCoroutine(HideHint());
            dialogueIdx = 0;
            transition = false;
        }
    }

    private void Start()
    {
        startY = speechBubble.transform.position.y;
        animator = GetComponent<Animator>();
        spriteFlipper = GetComponent<SpriteFlipper>();
    }

    private void FixedUpdate()
    {
        if (spriteFlipper.IsFacingLeft() != (EntityManager.GetPlayerPosition().x < transform.position.x))
        {
            spriteFlipper.Flip(EntityManager.GetPlayerPosition().x < transform.position.x);
        }    
    }

    private IEnumerator ShowText(string message)
    {
        speechBubble.gameObject.SetActive(true);
        speechText.text = message;
        transition = true;
        float param = 0;
        while (param < 1)
        {
            param += Time.deltaTime * 4;
            speechBubble.transform.position = new Vector3(speechBubble.transform.position.x, startY + param / 10f, speechBubble.transform.position.z);
            foreach (SpriteRenderer sprite in speechBubble.GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, param);
            }
            yield return new WaitForEndOfFrame();
        }
        transition = false;
        yield return null;
    }

    private IEnumerator HideText()
    {
        transition = true;
        float param = 1;
        while (param > 0)
        {
            param -= Time.deltaTime * 4;
            speechBubble.transform.position = new Vector3(speechBubble.transform.position.x, startY + param / 10f, speechBubble.transform.position.z);
            foreach (SpriteRenderer sprite in speechBubble.GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, param);
            }
            yield return new WaitForEndOfFrame();
        }
        speechBubble.gameObject.SetActive(false);
        transition = false;
        yield return null;
    }

    public void OnInteract()
    {
        if (dialogueIdx < dialogue.Count && !transition)
        {
            animator.SetTrigger("Talk");
            StartCoroutine(ShowText(dialogue[dialogueIdx]));
            dialogueIdx++;
        }
        else if (speechBubble.activeSelf && !transition)
        {
            StartCoroutine(HideText());
            dialogueIdx = 0;
        }
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
