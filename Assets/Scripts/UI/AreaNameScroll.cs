using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AreaNameScroll : MonoBehaviour
{
    [SerializeField]
    private float lingerTime = 4f;

    private float slideDistance = 500;
    private TMP_Text textLabel;
    private Vector3 startPos;
    private Animator animator;

    public void ShowAreaName(string name)
    {
        if (textLabel.text == name)
        {
            return;
        }

        textLabel.text = name;
        transform.position = startPos;
        animator.ResetTrigger("Expand");
        animator.ResetTrigger("Collapse");
        animator.Play("Closed");
        StopAllCoroutines();
        StartCoroutine(SlideAnimation());
    }

    public void Init()
    {
        startPos = transform.position;
        textLabel = GetComponentInChildren<TMP_Text>();
        slideDistance = ((RectTransform)transform).rect.width * 0.15f;
        animator = GetComponent<Animator>();
    }

    private IEnumerator SlideAnimation()
    {
        float param = 0;
        animator.SetTrigger("Expand");
        while (param < Mathf.PI / 2)
        {
            param += Mathf.PI * Time.deltaTime;
            transform.position = startPos + Vector3.left * slideDistance * Mathf.Sin(param);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(lingerTime);
        animator.SetTrigger("Collapse");
        while (param > 0)
        {
            param -= Mathf.PI * Time.deltaTime;
            transform.position = startPos + Vector3.left * slideDistance * Mathf.Sin(param);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
