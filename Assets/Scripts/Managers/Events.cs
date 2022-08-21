using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicEvent { };

public class ObjectHitEvent : BasicEvent
{
    public GameObject hitObj;

    public ObjectHitEvent(GameObject obj)
    {
        hitObj = obj;
    }
}

public class EnemyKOEvent : BasicEvent
{
    public BasicEnemy enemy;

    public EnemyKOEvent(BasicEnemy e)
    {
        enemy = e;
    }
}

public class PlayerHealthChangeEvent : BasicEvent
{
    public int damage;

    public PlayerHealthChangeEvent(int dmg)
    {
        damage = dmg;
    }
}

public class PlayerSpawnedEvent : BasicEvent
{
    public PlayerSpawnedEvent() { }
}
