using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [Serializable]
    public enum Abilities
    {
        ABILITY_GLIDE = 0
    }

    private static AbilityManager instance;

    private bool[] abilityFlags = new bool[16];

    [SerializeField]
    private Abilities[] debugEnabledAbilities;

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        foreach (Abilities ability in debugEnabledAbilities)
        {
            EnableAbility(ability);
        }
    }

    public static void EnableAbility(Abilities ability)
    {
        instance.abilityFlags[(int)ability] = true;
    }

    public static bool HasAbility(Abilities ability)
    {
        return instance.abilityFlags[(int)ability];
    }

    public static List<Abilities> GetEnabledAbilities()
    {
        List<Abilities> abilities = new List<Abilities>();
        for (int i = 0; i < instance.abilityFlags.Length; i++)
        {
            if (instance.abilityFlags[i])
            {
                abilities.Add((Abilities)i);
            }
        }
        return abilities;
    }
}
