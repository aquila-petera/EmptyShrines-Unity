using System;
using UnityEngine;

public class RNGManager : MonoBehaviour
{
    private static RNGManager instance;

    private void Awake()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    public static float RandomRange(float a, float b)
    {
        return UnityEngine.Random.Range(a, b);
    }
}
