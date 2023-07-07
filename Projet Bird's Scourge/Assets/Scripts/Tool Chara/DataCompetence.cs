using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "DataCompetence")]
public class DataCompetence : ScriptableObject
{
    public enum Effets
    {
        soin,
        poussee,
        teleportation,
        inversion,
        benediction
    }

    public enum Alterations
    {
        brulure,
        malediction,
        maladie,
        entrave,
        etourdissement,
        renforcement,
        affaiblissement,
        regeneration,
        vif,
        lenteur
    }

    public enum Paternes
    {
        horizontal,
        vertical,
        lignesDroites,
        diagonales,
    }

    
    //[Header("General")]
    public string competenceName;
   

    //[Header("LevelUp")] 
    public List<CompetenceLevel> levels = new List<CompetenceLevel>(1);
}

[System.Serializable]
public class ListBool
{
    public List<bool> list = new List<bool>();
}


[System.Serializable]
public class CompetenceLevel
{
    public string competenceDescription;
    public int competenceManaCost;
    public float damageMultiplier;
    public int baseHitRate;
    public float criticalMultiplier;
    
    public bool isCustom;
    [SerializeField] public List<ListBool> newPaterne = new List<ListBool>();
    public DataCompetence.Paternes newPaternePrefab;
    public int newPortee;
    
    public DataCompetence.Effets newEffet;
    
    public DataCompetence.Alterations newAlteration;
}
