using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour, IActivatable
{
    public void Activate()
    {
        Destroy(gameObject);
    }

    public bool IsActive()
    {
        return false;
    }
}
