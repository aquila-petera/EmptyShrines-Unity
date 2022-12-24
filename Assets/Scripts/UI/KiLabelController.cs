using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KiLabelController : MonoBehaviour
{
    [SerializeField] private TMP_Text kiText;

    private void OnEnable()
    {
        EventManager.BindEvent(typeof(PlayerKiChangeEvent), OnKiChanged);
        EventManager.BindEvent(typeof(PlayerSpawnedEvent), OnPlayerSpawn);
    }

    private void OnDisable()
    {
        EventManager.UnbindEvent(typeof(PlayerKiChangeEvent), OnKiChanged);
    }

    private void OnKiChanged(BasicEvent e)
    {
        int ki = (e as PlayerKiChangeEvent).newKi;
        int maxKi = (e as PlayerKiChangeEvent).newMaxKi;
        kiText.text = $"{ki}<color=\"grey\">/{maxKi}</color>";
    }

    private void OnPlayerSpawn(BasicEvent e)
    {
        int ki = EntityManager.GetPlayerKi();
        int maxKi = EntityManager.GetPlayerMaxKi();
        kiText.text = $"{ki}<color=\"grey\">/{maxKi}</color>";
    }
}
