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
    [HideInInspector] public int regionIndex;
    [HideInInspector] public int zoneIndex;
    [HideInInspector] public int maxY;
    [HideInInspector] public int currentY;
    private float currentAvancee;
    public GameObject unit1;
    public GameObject unit2;
    public GameObject unit3;

    [Header("Parameters")] 
    public float aventureCamSize = 10.8f;
    
    [Header("Battles")] 
    public List<ListBattle> possibleBattles;
    
    [Header("Events")] 
    public List<EventData> possibleEvents;
    
    [Header("Blessings")] 
    public List<ShopItemData> possibleBlessings;
    
    [Header("Items")] 
    public List<ShopItemData> possibleItems;

    [Header("Références")]
    [HideInInspector] public AventureCreator scriptCreator;
    [HideInInspector] public AventureController scriptController;
    


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);
        
        scriptCreator = GetComponent<AventureCreator>();
        scriptController = GetComponent<AventureController>();
    }

    private void Start()
    {
        StartCoroutine(AventureEffect.Instance.AppearEffect());
    }

    public void InitialisePossibleNods(AventureData data)
    {
        possibleBattles = data.battleNodes;
        possibleEvents = data.eventNodes;
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
        CalculateAvancee();

        for (int i = 0; i < possibleBattles.Count; i++)
        {
            if (currentAvancee <= ((float)(i + 1) / possibleBattles.Count) * 100f)
            {
                int battleIndex = Random.Range(0, possibleBattles[i].battleObjects.Count);
                
                return possibleBattles[i].battleObjects[battleIndex];
            }
        }

        return possibleBattles[0].battleObjects[0];
    }

    private void CalculateAvancee()
    {
        currentAvancee = ((float)currentY / maxY) * 100f;
    }
}
