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
        CameraManager.Instance.EnterGlobal();

        scriptController = GetComponent<GlobalMapControler>();
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
        
        SpriteRenderer[] continentSprites = continentObject.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < continentSprites.Length; i++)
        {
            dissolveValue = 0;
            SpriteRenderer currentSprite = continentSprites[i];
            
            DOTween.To(() => dissolveValue, x => dissolveValue = x, 1, 2f).OnUpdate((() =>
                currentSprite.material.SetFloat("_DissolveValue", dissolveValue)));
        }

        yield return new WaitForSeconds(1f);

        regionObject.SetActive(true);
        
        SpriteRenderer[] regionSprites = regionObject.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < regionSprites.Length; i++)
        {
            dissolveValue = 1;
            SpriteRenderer currentSprite = regionSprites[i];
            
            DOTween.To(() => dissolveValue, x => dissolveValue = x, 0, 2f).OnUpdate((() =>
                currentSprite.material.SetFloat("_DissolveValue", dissolveValue)));
        }

        yield return new WaitForSeconds(1f);
        
        continentObject.SetActive(false);
    }
}
