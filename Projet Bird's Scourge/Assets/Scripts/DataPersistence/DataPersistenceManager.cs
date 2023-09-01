using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance;

    [SerializeField] private string fileName;
    
    private GameData gameData;
    private List<IDataPersistence> IDataPersistenceObjects;
    private FileDataHandler fileDataHandler;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        IDataPersistenceObjects = GetIDataPersistenceObjects();
        fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        
        LoadGame();
    }


    
    // --------------------------- MAIN FUNCTIONS --------------------------------
    
    public void NewGame()
    {
        gameData = new GameData();
    }

    
    public void LoadGame()
    {
        gameData = fileDataHandler.LoadData();
        
        if (gameData == null)
        {
            NewGame();
        }

        foreach (var IDataPersistenceObj in IDataPersistenceObjects)
        {
            IDataPersistenceObj.LoadSave(gameData);
        }
    }

    
    public void SaveGame()
    {
        foreach (var IDataPersistenceObj in IDataPersistenceObjects)
        {
            IDataPersistenceObj.SaveData(ref gameData);
        }
        
        fileDataHandler.SaveData(gameData);
    }

    

    // --------------------------- OTHER FUNCTIONS --------------------------------
    
    private List<IDataPersistence> GetIDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> IDataPersistenceObjects =
            FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(IDataPersistenceObjects);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
