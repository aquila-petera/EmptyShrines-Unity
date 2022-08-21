using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingMovement : MonoBehaviour
{
    [SerializeField]
    private float frequency = 1;
    [SerializeField]
    private float amplitude = 0.025f;

    private float startY;

    void Start()
    {
        startY = transform.localPosition.y;
        StartCoroutine(DoFloat());
    }

    private IEnumerator DoFloat()
    {
        float param = 0;
        float animTime = 0;
        while (true)
        {
            animTime += Time.deltaTime;
            if (animTime > frequency)
            {
                animTime = 0;
            }
            param = (animTime / frequency) * Mathf.PI * 2;
            var pos = transform.localPosition;
            pos.y = startY + Mathf.Sin(param) * amplitude;
            transform.localPosition = pos;
            yield return new WaitForEndOfFrame();
        }
    }
}
