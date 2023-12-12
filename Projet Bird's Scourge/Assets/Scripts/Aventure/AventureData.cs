using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataAventure")]
public class AventureData : ScriptableObject
{
    [Header("Gen Pro")] 
    public int wantedMapLength;
    public int rawsNbr;
    [Range(1, 20)] public int distanceBetweenColumns;
    public int stepsBetweenCamp;
    [Range(0, 100)] public int probaSpotSpawn;
    public int maxElementsPerColumn;

    [Header("IconPart")]
    public List<NodeTypeClass> nodeTypes = new List<NodeTypeClass>();

    [Header("Nod Content")] 
    public List<ListBattle> battleNodes = new List<ListBattle>();
    public List<EventData> eventNodes = new List<EventData>();
}



[Serializable]
public class ListBattle
{
    public List<GameObject> battleObjects = new List<GameObject>();
}
