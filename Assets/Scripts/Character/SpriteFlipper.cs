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

    public void Spin(int flips = 2, float speed = 0)
    {
        flipping = true;
        StartCoroutine(SpinSprite(flips, speed));
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
        flipping = false;
    }

    private IEnumerator SpinSprite(int flips = 2, float speed = 0)
    {
        float param = 0, rStart = faceLeft ? 180 : 0, rEnd = rStart + 180;
        int flipsLeft = flips;
        while (flipsLeft > 0)
        {
            while (param < 1)
            {
                param += Time.fixedDeltaTime * (speed != 0 ? speed : FLIP_SPEED);
                sprite.transform.rotation = Quaternion.Euler(0, Mathf.Lerp(rStart, rEnd, param), 0);
                yield return new WaitForFixedUpdate();
            }

            param = 0;
            flipsLeft--;
            rStart = rStart == 0 ? 180 : 0;
            rEnd = rEnd == 180 ? 360 : 180;

            yield return null;
        }
        flipping = false;
    }
}
