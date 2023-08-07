using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AventureManager : MonoBehaviour
{
    [Header("Datas")] 
    public List<ListSpots> map;

    [Header("Références")]
    [HideInInspector] public AventureCreator scriptCreator;
    [HideInInspector] public AventureController scriptController;
    
    private void Start()
    {
        scriptCreator = GetComponent<AventureCreator>();
        scriptController = GetComponent<AventureController>();
        
        map = scriptCreator.GenerateMap();
        scriptController.Initialise(map);
    }


    private void Update()
    {
        if (!scriptController.noControl)
        {
            scriptController.ManageOverlayedElement();

            if (Input.GetKeyDown(KeyCode.Mouse0))
            { 
                scriptController.ManageClickedElement();
            }
        }
    }
}
