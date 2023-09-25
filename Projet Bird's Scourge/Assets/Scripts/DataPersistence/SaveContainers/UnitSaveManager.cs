using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSaveManager : MonoBehaviour, IDataPersistence
{
    public static UnitSaveManager Instance;

    public List<int> unitsLevels = new List<int>();
    public List<bool> unitsUnlocked = new List<bool>();
    
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);
    }

    
    
    public void LoadSave(GameData gameData)
    {
        unitsLevels = gameData.unitsLevels;
        unitsUnlocked = gameData.unitsUnlocked;
    }

    
    public void SaveData(ref GameData gameData)
    {
        gameData.unitsLevels = unitsLevels;
        gameData.unitsUnlocked = unitsUnlocked;
    }


    public int GetLevel(int index)
    {
        return unitsLevels[index];
    }
    
    public void GainLevel(int index)
    {
        unitsLevels[index] += 1;
    }

    public void GainUnit(int index)
    {
        unitsUnlocked[index] = true;
    }
}
