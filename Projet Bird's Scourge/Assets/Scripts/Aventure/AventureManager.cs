using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Searcher;
using UnityEngine;
using Random = UnityEngine.Random;

public class AventureManager : MonoBehaviour
{
    public static AventureManager Instance;
    
    [Header("Datas")] 
    [HideInInspector] public List<ListSpots> map;
    public GameObject unit1;
    public GameObject unit2;
    public GameObject unit3;
    
    [Header("Battles")] 
    public List<GameObject> possibleBattles;

    [Header("Références")]
    [HideInInspector] public AventureCreator scriptCreator;
    [HideInInspector] public AventureController scriptController;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        scriptCreator = GetComponent<AventureCreator>();
        scriptController = GetComponent<AventureController>();
        
        map = scriptCreator.GenerateMap();
        scriptController.Initialise(map);

        CameraManager.Instance.isInAdventure = true;
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


    // CHOSES WHICH BATTLE PREFAB IS ASSIGNED TO A NODE
    public GameObject ChoseBattle()
    {
        int battleIndex = Random.Range(0, possibleBattles.Count);

        return possibleBattles[battleIndex];
    }
}
