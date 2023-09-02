using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneMap : MonoBehaviour
{
    public AventureData zoneData;

    public void ChangeColor(Color newColor)
    {
        GetComponent<SpriteRenderer>().color = newColor;
    }
}
