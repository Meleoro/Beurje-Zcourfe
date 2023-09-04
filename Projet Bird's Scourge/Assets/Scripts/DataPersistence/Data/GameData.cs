using System;
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
    public int wood;
    public int stone;
    public int iron;
    public int gold;
    public int food;
    
    [Header("Progression")]
    public int r1z1Progression;
    public int r1z2Progression;
    public int r1z3Progression;
    public int r2z1Progression;
    public int r2z2Progression;
    public int r2z3Progression;
    public int r3z1Progression;
    public int r3z2Progression;
    public int r3z3Progression;
    public List<ListFloat> zonesProgressions = new List<ListFloat>();
    
    
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

        r1z1Progression = 0;
        r1z2Progression = 0;
        r1z3Progression = 0;
        
        r2z1Progression = 0;
        r2z2Progression = 0;
        r2z3Progression = 0;
        
        r3z1Progression = 0;
        r3z2Progression = 0;
        r3z3Progression = 0;

        zonesProgressions = new List<ListFloat>();
        
        for (int i = 0; i < 3; i++)
        {
            zonesProgressions.Add(new ListFloat());
            
            /*for (int j = 0; j < 3; j++)
            {
                zonesProgressions[i].list.Add(0);
            }*/
        }
    }
}
