using System;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [Serializable]
    public enum Abilities
    {
        ABILITY_DOUBLE_JUMP = 0
    }

    private static AbilityManager instance;

    private bool[] abilityFlags = new bool[16];

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
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
