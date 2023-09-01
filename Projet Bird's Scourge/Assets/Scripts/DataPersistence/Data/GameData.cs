using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    [Header("Units")]
    public int unit1Level;
    public int unit2Level;
    public int unit3Level;
    public int unit4Level;
    public int unit5Level;
    public int unit6Level;

    [Header("Resources")] 
    public int woodCounter;
    public int stoneCounter;
    
    
    public GameData()
    {
        unit1Level = 1;
        unit2Level = 1;
        unit3Level = 1;
        unit4Level = 1;
        unit5Level = 1;
        unit6Level = 1;

        woodCounter = 0;
        stoneCounter = 0;
    }
}
