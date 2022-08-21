using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptFlipper : MonoBehaviour, IActivatable
{
    [SerializeField]
    private List<TextLabel> textPrompts;
    [SerializeField]
    private float rotationTime;

    private bool rotating;
    private int activePromptIdx;

    public void Activate()
    {
        StartCoroutine(DoRotation());
    }

    public bool IsActive()
    {
        return rotating;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (TextLabel prompt in textPrompts)
        {
            prompt.AddTriggeredObject(gameObject);
            prompt.gameObject.SetActive(false);
        }
        textPrompts[activePromptIdx].gameObject.SetActive(true);
    }

    private IEnumerator DoRotation()
    {
        float theta = 360f / textPrompts.Count;
        float t = 0;
        int prevPromptIdx = activePromptIdx;
        activePromptIdx = activePromptIdx < textPrompts.Count - 1 ? activePromptIdx + 1 : 0;
        textPrompts[activePromptIdx].gameObject.SetActive(true);
        rotating = true;
        while (t < rotationTime)
        {
            transform.Rotate(0, Time.deltaTime / rotationTime * theta, 0);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        rotating = false;
        textPrompts[prevPromptIdx].gameObject.SetActive(false);
        yield return null;
    }
}
