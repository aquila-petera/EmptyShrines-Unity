using System;
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
            Destroy(gameObject);
        else
            instance = this;
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
}
