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
    [SerializeField] private Image bande1Image;
    [SerializeField] private Image bande2Image;

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
        Debug.Log(CameraManager.Instance.screenHeight);
        
        flashImage.DOFade(1, flashDuration * 0.3f).OnComplete((() => flashImage.DOFade(0, flashDuration * 0.7f)));
        
        yield return new WaitForSeconds(flashDuration * 2);

        bande1Image.rectTransform.DOScaleY(280, 1);
        bande1Image.rectTransform.DOLocalMoveY(bande1Image.rectTransform.localPosition.y + 280 * 0.5f, 1);
        
        bande2Image.rectTransform.DOScaleY(280, 1);
        bande2Image.rectTransform.DOLocalMoveY(bande2Image.rectTransform.localPosition.y - 280 * 0.5f, 1);
        
        yield return new WaitForSeconds(1);
        
        bande1Image.rectTransform.DOScaleY(0, 1);
        bande1Image.rectTransform.DOLocalMoveY(bande1Image.rectTransform.localPosition.y + 280 * 0.5f, 1);
        
        bande2Image.rectTransform.DOScaleY(0, 1);
        bande2Image.rectTransform.DOLocalMoveY(bande2Image.rectTransform.localPosition.y - 280 * 0.5f, 1);
    }
}
