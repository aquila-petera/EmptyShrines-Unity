using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRonin : BasicEnemy
{
    [SerializeField]
    private float dashDelay = 3;
    [SerializeField]
    private float moveTime = 0.5f;
    [SerializeField]
    private float walkSpd = 1;
    [SerializeField]
    private float dashSpd = 3;
    [SerializeField]
    private GameObject hat;

    bool moving, dashing, dying;
    float shootTimer, waitTime;
    Animator animator;
    SpriteFlipper spriteFlipper;

    new private void Start()
    {
        base.Start();

        waitTime = (dashDelay - moveTime) / 2;
        shootTimer = dashDelay;
        animator = GetComponent<Animator>();
        spriteFlipper = GetComponent<SpriteFlipper>();
    }

    private void Update()
    {
        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer < dashDelay - waitTime && shootTimer >= waitTime)
            {
                if (!moving)
                {
                    moving = true;
                }
                xspd = spriteFlipper.IsFacingLeft() ? walkSpd : -walkSpd;
                animator.SetBool("Moving", true);
            }
            else if (shootTimer <= 0)
            {
                StartShot();
            }
            else
            {
                xspd = 0;
                animator.SetBool("Moving", false);
            }
        }

        if (!dashing && !dying && spriteFlipper.IsFacingLeft() != (EntityManager.GetPlayerPosition().x < transform.position.x))
        {
            spriteFlipper.Flip(EntityManager.GetPlayerPosition().x < transform.position.x);
        }
    }

    public override bool OnHit(Vector3 hitOrigin)
    {
        if (hat != null || dying)
        {
            return false;
        }

        xspd = 0;
        yspd = 0;
        shootTimer = 0;
        animator.SetBool("Moving", false);
        animator.SetBool("Dashing", false);
        animator.SetBool("Hurt", true);
        moving = false;
        dashing = false;
        dying = true;
        spriteFlipper.Spin();
        TimingManager.ExecuteAfterDelay(1, Die);
        return true;
    }

    private void OnTriggerEnter(Collider col)
    {
        GameObject other = col.gameObject;
        if (EntityManager.ObjectIsPlayer(other) && dashing)
        {
            other.GetComponent<CharacterMovement>().TakeDamage();
        }
    }

    private void StartShot()
    {
        moving = false;
        yspd = 0;
        animator.SetBool("Moving", false);
        animator.SetBool("Dashing", true);
    }

    private void Shoot()
    {
        xspd = spriteFlipper.IsFacingLeft() ? -dashSpd : dashSpd;
        dashing = true;
    }

    private void EndShot()
    {
        animator.SetBool("Dashing", false);
        shootTimer = dashDelay;
        xspd = 0;
        dashing = false;
    }

    private IEnumerator DoKnockback(float startXSpd, float startYSpd, float duration)
    {
        float xkb = startXSpd;
        float ykb = startYSpd;
        float param = 1;
        while (param > 0)
        {
            xspd = Mathf.Lerp(0, startXSpd, param);
            yspd = Mathf.Lerp(0, startYSpd, param);
            param -= Time.deltaTime / duration;
            yield return new WaitForEndOfFrame();
        }
        xspd = 0;
        yspd = 0;
        shootTimer = dashDelay;
        yield return null;
    }
}
