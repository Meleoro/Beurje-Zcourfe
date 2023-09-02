using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesSaveManager : MonoBehaviour, IDataPersistence
{
    public static ResourcesSaveManager Instance;

    public int wood;
    public int stone;
    public int iron;
    public int gold;
    public int food;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);
    }


    public void SaveData(ref GameData gameData)
    {
        wood = gameData.wood;
        stone = gameData.stone;
        iron = gameData.iron;
        gold = gameData.gold;
        food = gameData.food;
    }
    public void LoadSave(GameData gameData)
    {
        gameData.wood = wood ;
        gameData.stone = stone;
        gameData.iron = iron;
        gameData.gold = gold;
        gameData.food = food;
    }
}
