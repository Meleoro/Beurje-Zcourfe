using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionSaveManager : MonoBehaviour, IDataPersistence
{
    public static ProgressionSaveManager Instance;
    
    /*public int r1z1Progression;
    public int r1z2Progression;
    public int r1z3Progression;
    
    public int r2z1Progression;
    public int r2z2Progression;
    public int r2z3Progression;
    
    public int r3z1Progression;
    public int r3z2Progression;
    public int r3z3Progression;*/

    [SerializeField] public List<ListFloat> zonesProgressions = new List<ListFloat>();


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);
    }



    public void ActualiseSaves(int regionIndex, int zoneIndex, int newProgression)
    {
        if (regionIndex >= 0 && regionIndex < zonesProgressions.Count)
        {
            if (zoneIndex >= 0 && zoneIndex < zonesProgressions[regionIndex].list.Count)
            {
                if (newProgression > zonesProgressions[regionIndex].list[zoneIndex])
                {
                    zonesProgressions[regionIndex].list[zoneIndex] = newProgression;
                }
            }
        }
    }
    
    


    public void LoadSave(GameData gameData)
    {
        /*r1z1Progression = gameData.r1z1Progression;
        r1z2Progression = gameData.r1z2Progression;
        r1z3Progression = gameData.r1z3Progression;
        
        r2z1Progression = gameData.r2z1Progression;
        r2z2Progression = gameData.r2z2Progression;
        r2z3Progression = gameData.r2z3Progression;
        
        r3z1Progression = gameData.r3z1Progression;
        r3z2Progression = gameData.r3z2Progression;
        r3z3Progression = gameData.r3z3Progression;*/

        zonesProgressions = gameData.zonesProgressions;
    }

    public void SaveData(ref GameData gameData)
    {
        /*gameData.r1z1Progression = r1z1Progression;
        gameData.r1z2Progression = r1z2Progression;
        gameData.r1z3Progression = r1z3Progression;
        
        gameData.r2z1Progression = r2z1Progression;
        gameData.r2z2Progression = r2z2Progression;
        gameData.r2z3Progression = r2z3Progression;
        
        gameData.r3z1Progression = r3z1Progression;
        gameData.r3z2Progression = r3z2Progression;
        gameData.r3z3Progression = r3z3Progression;*/

        gameData.zonesProgressions = zonesProgressions;
    }
}


[Serializable]
public class ListFloat
{
    public List<float> list;
}
