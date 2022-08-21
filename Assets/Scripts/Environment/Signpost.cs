using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signpost : MonoBehaviour, IHittable
{
    [SerializeField]
    private List<TextLabel> textPrompts;

    private bool wasHit;

    public bool OnHit(Vector3 hitOrigin)
    {
        if (wasHit)
            return false;

        wasHit = true;
        EventManager.InvokeEvent(new ObjectHitEvent(gameObject));
        foreach (TextLabel label in textPrompts)
        {
            if (label != null)
            {
                label.ForceDeactivate();
                label.gameObject.SetActive(!label.gameObject.activeSelf);
            }
        }
        GetComponent<SpriteFlipper>().ForceFlip(false);
        return true;
    }
}
