using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionManager : MonoBehaviour
{
    private static CollisionManager instance;

    private List<Collider> colliders;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
        colliders = new List<Collider>();
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    public static void RegisterCollider(Collider col)
    {
        if (!instance.colliders.Contains(col))
            instance.colliders.Add(col);
    }

    public static void UnregisterCollider(Collider col)
    {
        instance.colliders.Remove(col);
    }

    public static void OnSceneUnloaded(Scene scene)
    {
        instance.colliders.Clear();
    }

    public static bool AreObjectsColliding(GameObject objA, GameObject objB)
    {
        Collider colA = objA.GetComponent<Collider>();
        Collider colB = objB.GetComponent<Collider>();
        foreach (Collider col in Physics.OverlapBox(colA.bounds.center, colA.bounds.extents, colA.transform.rotation))
        {
            if (col.Equals(colB))
                return true;
        }
        return false;
    }

    private void FixedUpdate()
    {
        foreach (Collider colA in colliders)
        {
            foreach (Collider colB in Physics.OverlapBox(colA.bounds.center, colA.bounds.extents, colA.transform.rotation))
            {
                if (colA.Equals(colB))
                    continue;

                if (colliders.Contains(colB))
                {
                    colA.GetComponent<PhysicsObject>()?.OnCollision(colB.gameObject, colB.ClosestPoint(colA.transform.position));
                }
            }
        }
    }
}
