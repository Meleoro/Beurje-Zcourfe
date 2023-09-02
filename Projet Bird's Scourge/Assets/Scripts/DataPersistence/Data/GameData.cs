using System;
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
    public int wood;
    public int stone;
    public int iron;
    public int gold;
    public int food;
    
    
    public GameData()
    {
        unit1Level = 1;
        unit2Level = 1;
        unit3Level = 1;
        unit4Level = 1;
        unit5Level = 1;
        unit6Level = 1;

        wood = 0;
        stone = 0;
        iron = 0;
        gold = 0;
        food = 0;
    }
}
