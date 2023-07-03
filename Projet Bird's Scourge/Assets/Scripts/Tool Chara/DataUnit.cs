using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataUnit")]
public class DataUnit : ScriptableObject
{
    /*[Header("General")] */
    public string charaName;
    public int moveRange;
    
    /*[Header("Sprites")] */
    public Sprite idleSprite;
    public Sprite attackSprite;
    public Sprite damageSprite;


    /*[Header("Competences")] */
    public DataCompetence attaqueData;
    
    public int levelUnlockCompetence1;
    public DataCompetence competence1Data;
    
    public int levelUnlockCompetence2;
    public DataCompetence competence2Data;
    
    public int levelUnlockPassif;
    public DataCompetence passifData;


    /*[Header("Levels")] */
    public List<UnitLevel> levels = new List<UnitLevel>();
}


[System.Serializable]
public class UnitLevel
{
    //[Header("StatsGeneral")] 
    public int level;
    public int PV;
    public int PM;

    //[Header("Stats")] 
    public int force;
    public int defense;
    public int vitesse;
    public int agilite;
    public int precision;
}
