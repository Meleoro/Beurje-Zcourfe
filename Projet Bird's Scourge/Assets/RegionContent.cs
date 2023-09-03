using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegionContent : MonoBehaviour
{
    public bool isGlobal;
    public int regionIndex;

    public List<SpriteRenderer> regionSpriteRenderers = new List<SpriteRenderer>();
    public List<PolygonCollider2D> regionColliders = new List<PolygonCollider2D>();
    public List<TextMeshProUGUI> regionTexts = new List<TextMeshProUGUI>();
    public List<Image> regionImages = new List<Image>();

    public List<TextMeshProUGUI> progressionTexts = new List<TextMeshProUGUI>();
    public List<Image> progressionsImages = new List<Image>();


    public IEnumerator EnterRegion()
    {
        GlobalMapManager.Instance.scriptController.noControl = true;
        LoadProgressions(progressionTexts, progressionsImages, regionIndex);

        if (!isGlobal)
        {
            for (int i = 0; i < regionSpriteRenderers.Count; i++)
            {
                float dissolveValue = 1;
                SpriteRenderer currentSpriteRenderer = regionSpriteRenderers[i];

                DOTween.To(() => dissolveValue, x => dissolveValue = x, 0, 2f).OnUpdate((() =>
                    currentSpriteRenderer.material.SetFloat("_DissolveValue", dissolveValue)));
            } 
        }
        else
        {
            for (int i = 0; i < regionSpriteRenderers.Count; i++)
            {
                regionSpriteRenderers[i].DOFade(1f, 1);
            } 
        }

        for (int i = 0; i < regionColliders.Count; i++)
        {
            regionColliders[i].enabled = true;
        } 
        
        for (int i = 0; i < regionTexts.Count; i++)
        {
            regionTexts[i].enabled = true;

            regionTexts[i].DOFade(0, 0);
        } 
        
        for (int i = 0; i < regionImages.Count; i++)
        {
            regionImages[i].enabled = true;

            regionImages[i].DOFade(0, 0);
        }
        
        yield return new WaitForSeconds(0.8f);
        
        for (int i = 0; i < regionTexts.Count; i++)
        {
            regionTexts[i].DOFade(1, 1);
        } 
        
        for (int i = 0; i < regionImages.Count; i++)
        {
            regionImages[i].DOFade(1, 1);
        }

        yield return new WaitForSeconds(1);
        
        for (int i = 0; i < regionColliders.Count; i++)
        {
            regionColliders[i].enabled = true;
        } 
        
        yield return new WaitForSeconds(0.21f);
        
        GlobalMapManager.Instance.scriptController.noControl = false;
    }

    
    public IEnumerator QuitRegion()
    {
        GlobalMapManager.Instance.scriptController.noControl = true;
        
        if (!isGlobal)
        {
            for (int i = 0; i < regionSpriteRenderers.Count; i++)
            {
                float dissolveValue = 0;
                SpriteRenderer currentSpriteRenderer = regionSpriteRenderers[i];
            
                DOTween.To(() => dissolveValue, x => dissolveValue = x, 1, 2f).OnUpdate((() =>
                    currentSpriteRenderer.material.SetFloat("_DissolveValue", dissolveValue)));
            } 
        }
        else
        {
            for (int i = 0; i < regionSpriteRenderers.Count; i++)
            {
                regionSpriteRenderers[i].DOFade(0.5f, 1);
            } 
        }
        
        for (int i = 0; i < regionColliders.Count; i++)
        {
            regionColliders[i].enabled = false;
        } 
        
        for (int i = 0; i < regionTexts.Count; i++)
        {
            TextMeshProUGUI currentText = regionTexts[i];

            currentText.DOFade(0, 1).OnComplete(() => currentText.enabled = false);
        } 
        
        for (int i = 0; i < regionImages.Count; i++)
        {
            Image currentImage = regionImages[i];

            currentImage.DOFade(0, 1).OnComplete(() => currentImage.enabled = false);
        } 
        
        yield return new WaitForSeconds(2);

        if(!isGlobal)
            gameObject.SetActive(false);
        
        GlobalMapManager.Instance.scriptController.noControl = false;
    }
    
    


    private void LoadProgressions(List<TextMeshProUGUI> texts, List<Image> fillImages, int regionIndex)
    {
        for (int i = 0; i < texts.Count; i++)
        {
            texts[i].text = ProgressionSaveManager.Instance.zonesProgressions[regionIndex][i] + "%";
            fillImages[i].fillAmount = ProgressionSaveManager.Instance.zonesProgressions[regionIndex][i] / 100;
        }
    }
}
