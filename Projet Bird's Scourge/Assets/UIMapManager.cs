using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIMapManager : MonoBehaviour
{
    public static UIMapManager Instance;

    [Header("StartBattleTransitionReferences")]
    [SerializeField] private Image flashImage;

    [Header("StartBattleTransitionParameters")]
    [SerializeField] private float flashDuration;
    
    

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);
    }


    public IEnumerator StartBattleEffect()
    {
        flashImage.DOFade(1, flashDuration * 0.3f).OnComplete((() => flashImage.DOFade(0, flashDuration * 0.7f)));
        
        yield return new WaitForSeconds(flashDuration);
    }
}
