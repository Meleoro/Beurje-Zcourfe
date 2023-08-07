using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AventureManager : MonoBehaviour
{
    [Header("Datas")] 
    public List<ListSpots> map;
    
    [Header("Références")]
    private AventureCreator scriptCreator;
    private AventureController scriptController;
    
    void Start()
    {
        scriptCreator = GetComponent<AventureCreator>();
        scriptController = GetComponent<AventureController>();
        
        map = scriptCreator.GenerateMap();
        scriptController.Initialise(map);
    }
    
}
