using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyLeaf : MonoBehaviour
{
    [SerializeField]
    private float bouncePower = 4;

    private GameObject rotationPivot;
    private bool animating;
    private float rotY;

    public void OnCollisionEnter(Collision col)
    {
        GameObject other = col.gameObject;
        if (EntityManager.ObjectIsPlayer(other))
        {
            if (other.GetComponent<CharacterMovement>().GetSpeed().y < 0 && other.transform.position.y > transform.position.y)
            {
                other.GetComponent<CharacterMovement>().SetYSpeed(bouncePower);
                if (!animating)
                {
                    StartCoroutine(BounceAnimation());
                }
            }
            else if (!animating)
            {
                StartCoroutine(BounceAnimation(0.5f));
            }
        }
    }

    private void Start()
    {
        rotationPivot = transform.GetChild(0).gameObject;
        rotY = rotationPivot.transform.rotation.eulerAngles.y;
    }

    private IEnumerator BounceAnimation(float ampStart = 1)
    {
        float amp = ampStart;
        float param = 0;
        animating = true;
        while (amp > 0)
        {
            while (param < Mathf.PI * 2)
            {
                param += Mathf.PI * Time.deltaTime * 4;
                rotationPivot.transform.rotation = Quaternion.Euler(Mathf.Sin(param) * amp * 5, rotY, 0);
                yield return new WaitForEndOfFrame();
            }
            amp -= 0.5f;
            param = 0;
            yield return null;
        }
        animating = false;
        yield return null;
    }
}
