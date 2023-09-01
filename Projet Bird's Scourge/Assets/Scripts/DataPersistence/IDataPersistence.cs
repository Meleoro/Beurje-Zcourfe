using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    void LoadSave(GameData gameData);

    void SaveData(ref GameData gameData);
}
