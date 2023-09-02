using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneMap : MonoBehaviour
{
    public AventureData zoneData;

    private void Start()
    {
        GetComponent<SpriteRenderer>().color = GlobalMapManager.Instance.scriptController.baseColor;
    }
}
