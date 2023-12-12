using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSquad : ScriptableObject
{
    public List<GameObject> units = new List<GameObject>();
    public List<int> unitsHealth = new List<int>();

    public List<ShopItemData> items = new List<ShopItemData>();
}
