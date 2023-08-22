using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVfxManager : MonoBehaviour
{
    public static UIVfxManager Instance;

    [Header("Parameters")] 
    [SerializeField] private float attackFrameDuration;
    
    [Header("ReferencesVFX")] 
    [SerializeField] private List<Sprite> VFXSlash;
    [SerializeField] private List<Sprite> VFXBam;
    [SerializeField] private GameObject VFXHeal;

    [Header("ReferencesUI")] 
    [SerializeField] private Image leftAttackVFXImage;
    [SerializeField] private Image rightAttackVFXImage;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);

        leftAttackVFXImage.enabled = false;
        rightAttackVFXImage.enabled = false;
    }


    
    public void DOSlash(Vector2 imagePos, bool leftOrigin)
    {
        if (!leftOrigin)
        {
            leftAttackVFXImage.enabled = true;
            leftAttackVFXImage.rectTransform.position = imagePos;
            
            StartCoroutine(DoVFXSprite(VFXSlash, leftAttackVFXImage, attackFrameDuration));
        }
        else
        {
            rightAttackVFXImage.enabled = true;
            rightAttackVFXImage.rectTransform.position = imagePos;
            
            StartCoroutine(DoVFXSprite(VFXSlash, rightAttackVFXImage, attackFrameDuration));       
        }
    }
    
    
    public void DOBam(Vector2 imagePos, bool leftOrigin)
    {
        if (!leftOrigin)
        {
            leftAttackVFXImage.enabled = true;
            leftAttackVFXImage.rectTransform.position = imagePos;
            
            StartCoroutine(DoVFXSprite(VFXBam, leftAttackVFXImage, attackFrameDuration));
        }
        else
        {
            rightAttackVFXImage.enabled = true;
            rightAttackVFXImage.rectTransform.position = imagePos;
            
            StartCoroutine(DoVFXSprite(VFXBam, rightAttackVFXImage, attackFrameDuration));       
        }
    }

    public void DoHeal(RectTransform parentLeft, RectTransform parentRight, bool leftOrigin)
    {
        if (leftOrigin)
        {
            GameObject currentVFX = Instantiate(VFXHeal, parentRight.position, Quaternion.identity, parentRight);
            Destroy(currentVFX, 2f);
        }

        else
        {
            GameObject currentVFX = Instantiate(VFXHeal, parentLeft.localPosition, Quaternion.identity, parentLeft);
            Destroy(currentVFX, 2f);
        }
    }
    

    private IEnumerator DoVFXSprite(List<Sprite> sprites, Image modifiedImage, float frameDuration)
    {
        for (int i = 0; i < sprites.Count; i++)
        {
            modifiedImage.sprite = sprites[i];
            
            yield return new WaitForSeconds(frameDuration);
        }
        
        leftAttackVFXImage.enabled = false;
        rightAttackVFXImage.enabled = false;
    }
}
