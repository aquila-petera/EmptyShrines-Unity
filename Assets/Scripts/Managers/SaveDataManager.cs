using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveDataManager : MonoBehaviour
{
    private static SaveDataManager instance;

    private string SaveData_CurrentLevelName;
    private int SaveData_CurrentEntranceIndex;
    private List<AbilityManager.Abilities> SaveData_Abilities;
    private List<OfferingsManager.Offerings> SaveData_Offerings;

    private string saveDataPath = Application.persistentDataPath + "/data.dat";

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public static void SaveGame()
    {
        instance.SaveData_CurrentLevelName = EntityManager.GetRespawnScene();
        instance.SaveData_CurrentEntranceIndex = EntityManager.GetRespawnEntrance();
        instance.SaveData_Abilities = AbilityManager.GetEnabledAbilities();
        instance.SaveData_Offerings = OfferingsManager.GetCollectedOfferings();

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(instance.saveDataPath);
        formatter.Serialize(file, instance);
        file.Close();
    }

    public static void LoadGame()
    {
        if (File.Exists(instance.saveDataPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(instance.saveDataPath, FileMode.Open);
            SaveDataManager saveData = (SaveDataManager)formatter.Deserialize(file);
            file.Close();

            instance.SaveData_Abilities = saveData.SaveData_Abilities;
            instance.SaveData_CurrentEntranceIndex = saveData.SaveData_CurrentEntranceIndex;
            instance.SaveData_CurrentLevelName = saveData.SaveData_CurrentLevelName;
            instance.SaveData_Offerings = saveData.SaveData_Offerings;
        }
    }

    public static void ResetData()
    {
        if (File.Exists(instance.saveDataPath))
        {
            File.Delete(instance.saveDataPath);
            instance.SaveData_Abilities = new List<AbilityManager.Abilities>();
            instance.SaveData_CurrentEntranceIndex = 0;
            instance.SaveData_CurrentLevelName = "Temple1";
            instance.SaveData_Offerings = new List<OfferingsManager.Offerings>();
        }
    }
}
