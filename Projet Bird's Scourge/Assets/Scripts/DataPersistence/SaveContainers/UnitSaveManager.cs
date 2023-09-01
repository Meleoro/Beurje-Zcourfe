using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSaveManager : MonoBehaviour, IDataPersistence
{
    public static UnitSaveManager Instance;
    
    public int unit1Level;
    public int unit2Level;
    public int unit3Level;
    public int unit4Level;
    public int unit5Level;
    public int unit6Level;

    
    
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);
    }

    
    
    public void LoadSave(GameData gameData)
    {
        unit1Level = gameData.unit1Level;
        unit2Level = gameData.unit2Level;
        unit3Level = gameData.unit3Level;
        unit4Level = gameData.unit4Level;
        unit5Level = gameData.unit5Level;
        unit6Level = gameData.unit6Level;
    }

    
    public void SaveData(ref GameData gameData)
    {
        gameData.unit1Level = unit1Level;
        gameData.unit2Level = unit2Level;
        gameData.unit3Level = unit3Level;
        gameData.unit4Level = unit4Level;
        gameData.unit5Level = unit5Level;
        gameData.unit6Level = unit6Level;
    }
}
