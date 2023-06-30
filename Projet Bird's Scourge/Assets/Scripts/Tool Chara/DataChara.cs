using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataChara")]
public class DataChara : ScriptableObject
{
    [Header("General")] 
    public string charaName;
    public int startHealth;
    public int moveRange;


    [Header("Stats")] 
    public int PM;
    

    [Header("Competences")] 
    public DataCompetence attaqueData;
    
    public DataCompetence competence1Data;
    
    public DataCompetence competence2Data;
    
    public DataCompetence passifData;


    [Header("Levels")] 
    public int currentLevel;

}
