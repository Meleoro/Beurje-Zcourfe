using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesSaveManager : MonoBehaviour, IDataPersistence
{
    public static ResourcesSaveManager Instance;

    public int woodCounter;
    public int stoneCounter;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);
    }


    public void LoadSave(GameData gameData)
    {
        woodCounter = gameData.woodCounter;
        stoneCounter = gameData.stoneCounter;
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.woodCounter = woodCounter;
        gameData.stoneCounter = stoneCounter;
    }
}
