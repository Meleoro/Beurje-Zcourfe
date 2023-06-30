using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataUnit")]
public class DataUnit : ScriptableObject
{
    [Header("General")] 
    public string charaName;
    public int moveRange;
    
    [Header("Sprites")] 
    public Sprite idleSprite;
    public Sprite attackSprite;
    public Sprite damageSprite;


    [Header("Competences")] 
    public DataCompetence attaqueData;
    
    public DataCompetence competence1Data;
    
    public DataCompetence competence2Data;
    
    public DataCompetence passifData;


    [Header("Levels")] 
    public int currentLevel;

}


[System.Serializable]
public class UnitLevel
{
    [Header("StatsGeneral")] 
    public int maxHealth;
    public int moveRange;
    
    [Header("Stats")] 
    public int strength;
    public int defense;
}
