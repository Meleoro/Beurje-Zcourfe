using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class GlobalMapManager : MonoBehaviour
{
    public static GlobalMapManager Instance;

    [Header("Parameters")] 
    [SerializeField] private Vector3 aventurePos;

    [Header("References")]
    [SerializeField] private GameObject aventureObject;
    [SerializeField] private GameObject continentObject;

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
        List<ListSpots> map = currentCreator.GenerateMap();
        
        AventureManager.Instance.scriptController.Initialise(map);
    }

    public IEnumerator EnterRegion(GameObject regionObject)
    {
        float dissolveValue = 0;
        SpriteRenderer continentSprite = continentObject.GetComponentInChildren<SpriteRenderer>(); 

        DOTween.To(() => dissolveValue, x => dissolveValue = x, 1, 2f).OnUpdate((() =>
            continentSprite.material.SetFloat("_DissolveValue", dissolveValue)));

        yield return new WaitForSeconds(1f);

        regionObject.SetActive(true);
        
        dissolveValue = 1;
        SpriteRenderer regionSprite = regionObject.GetComponentInChildren<SpriteRenderer>(); 

        DOTween.To(() => dissolveValue, x => dissolveValue = x, 0, 2f).OnUpdate((() =>
            regionSprite.material.SetFloat("_DissolveValue", dissolveValue)));
        
        yield return new WaitForSeconds(1f);
        
        continentObject.SetActive(false);
    }
}
