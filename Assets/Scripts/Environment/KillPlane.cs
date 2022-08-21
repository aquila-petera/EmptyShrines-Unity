using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
    public void OnTriggerEnter(Collider col)
    {
        if (EntityManager.ObjectIsPlayer(col.gameObject))
        {
            EntityManager.PlayerFall();
        }
    }
}
