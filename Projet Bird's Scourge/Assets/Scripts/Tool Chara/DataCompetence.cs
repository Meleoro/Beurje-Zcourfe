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
    public int degatsMin;
    public int degatsMax;
    public int cooldown;
    
    //[Header("Paterne")]
    public bool hasCustomPaterne;
    [SerializeField] public List<ListBool> customPaterne = new List<ListBool>();
    public Paternes paternePrefab;
    public int portee;
    
    //[Header("Effets")]
    public bool doEffet;
    public Effets effet;
    public bool doAlteration;
    public Alterations alteration;

    //[Header("LevelUp")] 
    public List<CompetenceLevel> levels = new List<CompetenceLevel>(1);


    /*public void InitialiseList()
    {
        for (int i = 0; i < 7; i++)
        {
            paterne.Add(new List<bool>());
            
            for (int j = 0; j < 7; j++)
            {
                paterne[i].Add(false);
            }
        }
    }

    public void ChangeValue(int x, int y, bool newValue)
    {
        paterne[x][y] = newValue;
    }*/
}

[System.Serializable]
public class ListBool
{
    public List<bool> list = new List<bool>();
}


[System.Serializable]
public class CompetenceLevel
{
    public bool upgradeDegats;
    public int newDegatsMin;
    public int newDegatsMax;
    
    public bool upgradePaterne;
    public bool isCustom;
    [SerializeField] public List<ListBool> newPaterne = new List<ListBool>();
    public DataCompetence.Paternes newPaternePrefab;
    public int newPortee;

    public bool upgradeEffet;
    public DataCompetence.Effets newEffet;

    public bool upgradeAlteration;
    public DataCompetence.Alterations newAlteration;
}
