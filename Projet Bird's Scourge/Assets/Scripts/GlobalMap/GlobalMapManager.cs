using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class GlobalMapManager : MonoBehaviour
{
    public static GlobalMapManager Instance;

    [Header("CurrentInfos")] 
    public RegionContent currentRegion;
    public Vector3 cameraOriginalPos;
    public float cameraOriginalSize;

    [Header("Parameters")] 
    [SerializeField] private Vector3 aventurePos;

    [Header("References")]
    [SerializeField] private GameObject aventureObject;
    [SerializeField] private GameObject continentObject;
    [HideInInspector] public GlobalMapControler scriptController;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        else
            Destroy(gameObject);
    }

    private void Start()
    {
        //GameManager.Instance.EnterGlobalMap();

        scriptController = GetComponent<GlobalMapControler>();
    }

    public void Initialise()
    {
        currentRegion = continentObject.GetComponent<RegionContent>();
    }
    
    

    public void LaunchAventure(AventureData data)
    {
        AventureCreator currentCreator = Instantiate(aventureObject, aventurePos, Quaternion.identity).GetComponentInChildren<AventureCreator>();

        currentCreator.data = data;
        List<ListSpots> map = currentCreator.GenerateMap();
        
        AventureManager.Instance.scriptController.Initialise(map);
    }

    
    
    public IEnumerator EnterRegion(GameObject regionObject, Vector3 posCamOffset, float newSizeCam)
    {
        scriptController.noControl = true;
        
        cameraOriginalPos = CameraManager.Instance.transform.position;
        cameraOriginalSize = CameraManager.Instance._camera.orthographicSize;

        CameraManager.Instance.transform.DOMove(cameraOriginalPos + posCamOffset, 1);
        CameraManager.Instance._camera.DOOrthoSize(newSizeCam, 1);

        yield return new WaitForSeconds(0.2f);

        regionObject.SetActive(true);

        
        StartCoroutine(currentRegion.QuitRegion());
        currentRegion = regionObject.GetComponent<RegionContent>();
        StartCoroutine(currentRegion.EnterRegion());
    }

    public IEnumerator QuitRegion()
    {
        scriptController.noControl = true;

        CameraManager.Instance.transform.DOMove(cameraOriginalPos, 1);
        CameraManager.Instance._camera.DOOrthoSize(cameraOriginalSize, 1);
        
        yield return new WaitForSeconds(0.2f);
        
        StartCoroutine(currentRegion.QuitRegion());
        currentRegion = continentObject.GetComponent<RegionContent>();
        StartCoroutine(currentRegion.EnterRegion());
    }
}
