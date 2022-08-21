using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : PhysicsObject
{
    GameObject spriteContainer;

    new private void Start()
    {
        delayInit = true;
        spriteContainer = GetComponentInChildren<SpriteRenderer>().gameObject;
    }

    private void Update()
    {
        spriteContainer.transform.Rotate(0, 0, -720 * Time.deltaTime);
    }

    public void SetVelocity(float xs, float ys)
    {
        xspd = xs;
        yspd = ys;
    }

    public override void OnCollision(GameObject other, Vector3 collisionPoint)
    {
        if (IsSolid(other))
        {
            ParticleManager.PlayParticleSystemAtPosition("SmokePuff", collisionPoint);
            Destroy(gameObject);
        }
        else if (EntityManager.ObjectIsPlayer(other))
        {
            other.GetComponent<CharacterMovement>().TakeDamage();
            Destroy(gameObject);
        }
    }
}
