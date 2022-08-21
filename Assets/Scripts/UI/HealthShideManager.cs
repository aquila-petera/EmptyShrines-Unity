using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthShideManager : MonoBehaviour
{
    [SerializeField]
    private Transform shideTransform;
    [SerializeField]
    private Image bentRopeLeft;
    [SerializeField]
    private Image bentRopeRight;

    public void Flip()
    {
        StartCoroutine(DoFlip());
    }

    public void FlipInstant()
    {
        shideTransform.rotation = Quaternion.Euler(0, 180, 0);
    }

    public void SetIsLeftmost(bool leftmost)
    {
        bentRopeLeft.enabled = leftmost;
    }

    public void SetIsRightmost(bool rightmost)
    {
        bentRopeRight.enabled = rightmost;
    }

    private IEnumerator DoFlip()
    {
        float rotation = 0;
        float startRotation = shideTransform.rotation.eulerAngles.y;
        while (rotation < 180)
        {
            rotation += Time.deltaTime * 360;
            if (rotation > 180)
                rotation = 180;
            shideTransform.rotation = Quaternion.Euler(0, startRotation + rotation, 0);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
