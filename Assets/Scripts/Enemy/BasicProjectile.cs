using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : PhysicsEntity
{
    [SerializeField]
    bool rotateConstantly = true;

    GameObject spriteContainer;

    new private void Start()
    {
        base.Start();
        spriteContainer = GetComponentInChildren<SpriteRenderer>().gameObject;
    }

    private void Update()
    {
        if (rotateConstantly)
        {
            spriteContainer.transform.Rotate(0, 0, -720 * Time.deltaTime);
        }
        else
        {
            spriteContainer.transform.localRotation = Quaternion.Euler(0, 0, Vector3.SignedAngle(Vector3.right, GetSpeed().normalized, Vector3.forward));
        }
    }

    public void SetVelocity(float xs, float ys)
    {
        xspd = xs;
        yspd = ys;
    }

    private void OnTriggerEnter(Collider col)
    {
        GameObject other = col.gameObject;
        if (IsSolid(other))
        {
            ParticleManager.PlayParticleSystemAtPosition("SmokePuff", col.ClosestPointOnBounds(transform.position));
            Destroy(gameObject);
        }
        else if (EntityManager.ObjectIsPlayer(other))
        {
            other.GetComponent<CharacterMovement>().TakeDamage();
            Destroy(gameObject);
        }
    }
}
