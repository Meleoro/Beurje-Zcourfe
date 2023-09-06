using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenedictionManager : MonoBehaviour
{
    [Header("Général")] 
    public static BenedictionManager instance;
    public List<ShopItemData> possessedBlessings;

    public void Awake()
    {
        if (instance == null) instance = this;
    }

    public void GetNewBlessing(ShopItemData blessingGot)
    {
        possessedBlessings.Add(blessingGot);
    }

    public bool CheckIfBlessingGot(int ID)
    {
        for (int i = 0; i < possessedBlessings.Count; i++)
        {
            if (ID == possessedBlessings[i].ID)
            {
                return true;
            }
        }
        return false;
    }
}
