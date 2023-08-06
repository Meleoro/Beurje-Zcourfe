using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AventureManager : MonoBehaviour
{
    [SerializeField] private AventureCreator scriptCreator;
    
    void Start()
    {
        scriptCreator = GetComponent<AventureCreator>();
        scriptCreator.GenerateMap();
    }
    
}
