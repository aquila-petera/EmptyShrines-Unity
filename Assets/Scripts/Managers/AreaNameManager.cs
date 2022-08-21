using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaNameManager : MonoBehaviour
{
    private static AreaNameManager instance;
    private static string currentAreaName;

    [SerializeField]
    private string areaName;

    private AreaNameScroll areaNameScroll;

    public void SetAreaName()
    {
        areaNameScroll.ShowAreaName(currentAreaName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (areaNameScroll == null)
        {
            areaNameScroll = GetComponentInChildren<AreaNameScroll>();
            areaNameScroll.Init();
        }
        SetAreaName();
    }

    private void Awake()
    {
        currentAreaName = areaName;
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
