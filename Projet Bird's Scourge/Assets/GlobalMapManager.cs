using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMapManager : MonoBehaviour
{
    public static GlobalMapManager Instance;

    [Header("Parameters")] 
    [SerializeField] private Vector3 aventurePos;

    [Header("References")]
    [SerializeField] private GameObject aventureObject;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        else
            Destroy(gameObject);
    }

    private void Start()
    {
        CameraManager.Instance.EnterGlobal();
    }


    public void OpenZone()
    {
        
    }
    

    public void LaunchAventure(AventureData data)
    {
        AventureCreator currentCreator = Instantiate(aventureObject, aventurePos, Quaternion.identity).GetComponentInChildren<AventureCreator>();

        currentCreator.data = data;
        currentCreator.GenerateMap();
    }
}
