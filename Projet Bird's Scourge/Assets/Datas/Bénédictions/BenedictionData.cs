using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BlessingData")]

public class BenedictionData : ScriptableObject
{
    public int ID;
    public string name;
    public string description;
    public Sprite icon;
}
