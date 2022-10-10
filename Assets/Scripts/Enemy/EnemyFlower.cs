using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlower : BasicEnemy
{
    [SerializeField]
    private float idleTime = 2;
    [SerializeField]
    private float prepTime = 2;
    [SerializeField]
    private GameObject sprite;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private float shotSpeed = 2;

    private float idleTimer = 0;
    private float prepTimer = 0;
    private Animator animator;

    public override bool OnHit(Vector3 hitOrigin)
    {
        prepTimer = 0;
        idleTimer = idleTime;
        animator.SetTrigger("Hit");
        animator.ResetTrigger("Prep");
        return base.OnHit(hitOrigin);
    }

    new private void Start()
    {
        base.Start();

        idleTimer = idleTime;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (idleTimer > 0)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
            {
                idleTimer = 0;
                prepTimer = prepTime;
                animator.SetTrigger("Prep");
                animator.ResetTrigger("Shoot");
                animator.ResetTrigger("Hit");
            }
        }
        if (prepTimer > 0)
        {
            prepTimer -= Time.deltaTime;
            float param = prepTimer * 2 * Mathf.PI;
            if (prepTimer <= 0)
            {
                prepTimer = 0;
                idleTimer = idleTime;
                animator.SetTrigger("Shoot");
                animator.ResetTrigger("Prep");
            }
        }
    }

    private void Shoot()
    {
        ProjectileFactory.GetInstance().SpawnCustomProjectile(projectilePrefab, transform.position, Vector3.up * shotSpeed);
    }
}
