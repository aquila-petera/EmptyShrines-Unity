using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHead : BasicEnemy
{
    [SerializeField]
    private float shootDelay = 3;
    [SerializeField]
    private float moveTime = 1;
    [SerializeField]
    private float baseXSpd = 1;
    [SerializeField]
    private float baseYSpd = 0.25f;
    [SerializeField]
    private Sprite projectileSprite;
    [SerializeField]
    private float projectileSpeed = 1;
    [SerializeField]
    private Transform projectileOrigin;

    bool moving = false;
    float shootTimer, waitTime;
    float flipChance = 0.5f;
    float kbPower = 1.5f;
    float kbDuration = 0.5f;
    Animator animator;

    new private void Start()
    {
        base.Start();

        waitTime = (shootDelay - moveTime) / 2;
        shootTimer = shootDelay;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer < shootDelay - waitTime && shootTimer >= waitTime)
            {
                if (!moving)
                {
                    moving = true;
                    baseYSpd = -baseYSpd;
                }
                float param = (shootTimer - waitTime) / moveTime;
                xspd = Mathf.Lerp(0, baseXSpd, param);
                yspd = Mathf.Lerp(0, baseYSpd, param);
            }
            else if (shootTimer <= 0)
            {
                StartShot();
            }
        }
    }

    public override bool OnHit(Vector3 hitOrigin)
    {
        base.OnHit(hitOrigin);

        moving = false;
        shootTimer = 0;
        xspd = 0;
        yspd = 0;
        if (hitOrigin.x < transform.position.x)
        {
            StartCoroutine(DoKnockback(kbPower, 0, kbDuration));
        }
        else
        {
            StartCoroutine(DoKnockback(-kbPower, 0, kbDuration));
        }
        return true;
    }

    private void StartShot()
    {
        moving = false;
        xspd = 0;
        yspd = 0;
        animator.SetBool("Shooting", true);
    }

    private void Shoot()
    {
        ProjectileFactory.GetInstance().FireProjectileAtPlayer(projectileSprite, projectileOrigin.position, projectileSpeed);
    }

    private void EndShot()
    {
        animator.SetBool("Shooting", false);
        shootTimer = shootDelay;
        if (Random.Range(0, 2) > flipChance)
        {
            baseXSpd = -baseXSpd;
            flipChance = 0.5f;
        }
        else
        {
            flipChance /= 2;
        }
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
        shootTimer = shootDelay;
        yield return null;
    }
}
