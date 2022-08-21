using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogWall : MonoBehaviour, IActivatable
{
    private BoxCollider boxCollider;
    private ParticleSystem particles;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        particles = GetComponentInChildren<ParticleSystem>();
        CollisionManager.RegisterCollider(boxCollider);
    }

    private void OnDestroy()
    {
        CollisionManager.UnregisterCollider(boxCollider);
    }

    public void Activate()
    {
        if (IsActive())
        {
            boxCollider.enabled = false;
            particles.Stop();
        }
        else
        {
            boxCollider.enabled = true;
            particles.Play();
        }
    }

    public bool IsActive()
    {
        return boxCollider.enabled;
    }
}
