using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Debug")] 
    public bool startInMap;
    public bool startInAventure;
    public AventureData startAventureData;
    
    
    [Header("GlobalMap")] 
    public GameObject globalMapObject;
    public Vector3 globalMapPos;
    private GameObject currentGlobalMap;
    private bool isInGlobalMap;

    [Header("Aventure")] 
    public GameObject aventureObject;
    public Vector3 aventurePos;
    private GameObject currentAventure;
    private bool isInAventure;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);
    }


    private void Start()
    {
        if (startInMap)
            EnterGlobalMap();

        else
            StartCoroutine(EnterAventure(startAventureData, -1, -1));
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(ExitAventure());
        }
    }


    public void EnterGlobalMap()
    {
        isInGlobalMap = true;
        isInAventure = false;
        
        currentGlobalMap = Instantiate(globalMapObject, globalMapPos, Quaternion.identity);

        CameraManager.Instance.EnterGlobal(globalMapPos + new Vector3(0, 0, -10), GlobalMapManager.Instance.cameraOriginalSize);
    }

    
    
    
    public IEnumerator EnterAventure(AventureData data, int regionIndex, int zoneIndex)
    {
        StartCoroutine(CameraManager.Instance.TransitionAventure(aventurePos + new Vector3(2.8f, 0, -10), CameraManager.Instance.isInGlobal));

        yield return new WaitForSeconds(2.2f);
        
        if (currentGlobalMap != null)
        {
            Destroy(currentGlobalMap);
        }
        
        AventureCreator currentCreator = Instantiate(aventureObject, aventurePos, Quaternion.identity).GetComponentInChildren<AventureCreator>();
        currentAventure = currentCreator.gameObject;

        currentCreator.data = data;
        List<ListSpots> map = currentCreator.GenerateMap();
        
        AventureManager.Instance.scriptController.Initialise(map);
        AventureManager.Instance.regionIndex = regionIndex;
        AventureManager.Instance.zoneIndex = zoneIndex;

        isInGlobalMap = false;
        isInAventure = true;
    }

    public IEnumerator ExitAventure()
    {
        StartCoroutine(CameraManager.Instance.TransitionAventure(globalMapPos + new Vector3(0, 0, -10), true));
        
        ProgressionSaveManager.Instance.ActualiseSaves(AventureManager.Instance.regionIndex, AventureManager.Instance.zoneIndex, (int)(((float)AventureManager.Instance.currentY / (float)AventureManager.Instance.maxY) * 100f));

        yield return new WaitForSeconds(2.2f);
        
        Destroy(currentAventure);
        
        currentGlobalMap = Instantiate(globalMapObject, globalMapPos, Quaternion.identity);
        
        isInGlobalMap = true;
        isInAventure = false;
    }
}
