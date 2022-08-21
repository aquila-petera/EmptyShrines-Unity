using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFlipper : MonoBehaviour
{
    private static readonly float FLIP_SPEED = 5f;

    [SerializeField]
    private GameObject sprite;
    [SerializeField]
    private bool faceLeft;

    private bool flipping;
    private bool spinning;

    void Start()
    {
        if (faceLeft)
        {
            sprite.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public bool IsFacingLeft()
    {
        return faceLeft;
    }

    public void Flip(bool left)
    {
        if (faceLeft == left || flipping)
            return;

        flipping = true;
        faceLeft = left;
        StartCoroutine(FlipSprite(left, sprite.transform.rotation.eulerAngles.y));
    }

    public void ForceFlip(bool left)
    {
        if (!flipping)
        {
            StartCoroutine(FlipSprite(left, sprite.transform.rotation.eulerAngles.y));
        }
    }

    public void Spin(bool left)
    {
        spinning = true;
        flipping = true;
        StartCoroutine(FlipSprite(left, faceLeft ? 180 : 0));
    }

    public void SetFacing(bool left)
    {
        sprite.transform.rotation = left ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        faceLeft = left;
    }

    private IEnumerator FlipSprite(bool left, float startRotation)
    {
        float targetRotation = left ? startRotation - 180 : startRotation + 180;
        float param = 0;
        while (param < 1)
        {
            param += Time.fixedDeltaTime * FLIP_SPEED;
            sprite.transform.rotation = Quaternion.Euler(0, Mathf.Lerp(startRotation, targetRotation, param), 0);
            yield return new WaitForFixedUpdate();
        }
        if (spinning)
        {
            spinning = false;
            yield return FlipSprite(left, startRotation + 180);
        }
        else
        {
            flipping = false;
        }
        yield return null;
    }
}
