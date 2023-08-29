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
    public int minBattlePerPath = 2;
    

    [Header("Nod Content")] 
    public List<GameObject> battleNodes = new List<GameObject>();
}
