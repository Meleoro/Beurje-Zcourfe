using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataChara")]
public class DataChara : ScriptableObject
{
    [Header("General")] 
    public string charaName;
    public int startHealth;
    public int maxLevel;


    [Header("Stats")] 
    public int PM;
    

    [Header("Competences")] 
    public DataCompetence attaqueData;

    public int levelCompetence1;
    public DataCompetence competenceData1;

    public int levelCompetence2;
    public DataCompetence competenceData2;

    public int levelPassif;
    public DataCompetence passifData;


    [Header("Levels")] 
    public int currentLevel;

}
