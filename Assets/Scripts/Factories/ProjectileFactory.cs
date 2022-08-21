using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFactory : MonoBehaviour
{
    private static ProjectileFactory instance;

    [SerializeField]
    private GameObject projectileTemplate;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    public static ProjectileFactory GetInstance()
    {
        return instance;
    }

    public BasicProjectile FireProjectileAtPlayer(Sprite sprite, Vector3 position, float speed)
    {
        GameObject go = Instantiate(projectileTemplate);
        BasicProjectile proj = go.GetComponent<BasicProjectile>();
        go.transform.position = position;
        Vector3 direction = EntityManager.GetPlayerPosition() - position;
        direction = direction.normalized * speed;
        proj.SetVelocity(direction.x, direction.y);
        go.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        return proj;
    }

    public BasicProjectile SpawnProjectile(Sprite sprite, Vector3 position, Vector2 velocity)
    {
        GameObject go = Instantiate(projectileTemplate);
        BasicProjectile proj = go.GetComponent<BasicProjectile>();
        go.transform.position = position;
        proj.SetVelocity(velocity.x, velocity.y);
        go.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        return proj;
    }
}
