using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenedictionManager : MonoBehaviour
{
    [Header("Général")] 
    public static BenedictionManager instance;
    public List<BenedictionData> possessedBlessings;

    public void Awake()
    {
        if (instance == null) instance = this;
    }

    public void GetNewBlessing(BenedictionData blessingGot)
    {
        possessedBlessings.Add(blessingGot);
        
        switch (blessingGot.ID)
        {
            case 0 :
                
                break;
        }
    }

    public bool VerifyAdditionalPM()
    {
        return false;
    }
}
