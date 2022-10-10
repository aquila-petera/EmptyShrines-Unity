using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : PhysicsEntity, IActivatable, IHittable
{
    [SerializeField]
    protected List<TextLabel> textPrompts;
    [SerializeField]
    protected bool autoActive;

    new protected void Start()
    {
        base.Start();

        EntityManager.RegisterEnemy(gameObject);
        if (!autoActive)
        {
            gameObject.SetActive(false);
        }
    }

    public virtual bool OnHit(Vector3 hitOrigin)
    {
        EventManager.InvokeEvent(new ObjectHitEvent(gameObject));
        foreach (TextLabel label in textPrompts)
        {
            label.SetEnabled(!label.GetEnabled());
        }
        GetComponent<SpriteFlipper>().ForceFlip(false);
        return true;
    }

    public virtual void Die()
    {
        ParticleManager.PlayParticleSystemAtPosition("SmokePuffBig", transform.position, new Color(0.6f, 0.2f, 0.6f));
        EventManager.InvokeEvent(new EnemyKOEvent(this));
        Destroy(gameObject);
    }

    public void Activate()
    {
        Die();
    }

    public bool IsActive()
    {
        return false;
    }
}
