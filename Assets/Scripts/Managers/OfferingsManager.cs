using System;
using System.Collections.Generic;
using UnityEngine;

public class OfferingsManager : MonoBehaviour
{
    [Serializable]
    public enum Offerings
    {
        OFFERING_RICE = 0
    }

    private static OfferingsManager instance;

    private bool[] offeringFlags = new bool[16];

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public static List<Offerings> GetCollectedOfferings()
    {
        List<Offerings> abilities = new List<Offerings>();
        for (int i = 0; i < instance.offeringFlags.Length; i++)
        {
            if (instance.offeringFlags[i])
            {
                abilities.Add((Offerings)i);
            }
        }
        return abilities;
    }
}
