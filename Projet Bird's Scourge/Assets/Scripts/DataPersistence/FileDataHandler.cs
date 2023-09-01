using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string dirPath;
    private string fileName;

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        dirPath = dataDirPath;
        fileName = dataFileName;
    }
    
    
    public GameData LoadData()
    {
        string fullPath = Path.Combine(dirPath, fileName);
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad;

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from: " + fullPath + "/n" + e);
            }
        }

        return loadedData;
    }

    public void SaveData(GameData gameData)
    {
        string fullPath = Path.Combine(dirPath, fileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(gameData, true);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to: " + fullPath + "/n" + e);
        }
    }
}
