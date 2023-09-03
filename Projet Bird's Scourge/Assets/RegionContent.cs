using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegionContent : MonoBehaviour
{
    public bool isGlobal;

    public List<SpriteRenderer> regionSpriteRenderers = new List<SpriteRenderer>();
    public List<PolygonCollider2D> regionColliders = new List<PolygonCollider2D>();
    public List<TextMeshProUGUI> regionTexts = new List<TextMeshProUGUI>();
    public List<Image> regionImages = new List<Image>();


    public IEnumerator EnterRegion()
    {
        if (!isGlobal)
        {
            for (int i = 0; i < regionSpriteRenderers.Count; i++)
            {
                float dissolveValue = 1;
                Material currentSprite = regionSpriteRenderers[i].material;
            
                DOTween.To(() => dissolveValue, x => dissolveValue = x, 0, 2f).OnUpdate((() =>
                    currentSprite.SetFloat("_DissolveValue", dissolveValue)));
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

            float dissolveValue = 1;
            Material currentSprite = regionTexts[i].material;
            
            DOTween.To(() => dissolveValue, x => dissolveValue = x, 0, 2f).OnUpdate((() =>
                currentSprite.SetFloat("_DissolveValue", dissolveValue)));
        } 
        
        for (int i = 0; i < regionImages.Count; i++)
        {
            regionImages[i].enabled = true;

            float dissolveValue = 1;
            Material currentSprite = regionImages[i].material;
            
            DOTween.To(() => dissolveValue, x => dissolveValue = x, 0, 2f).OnUpdate((() =>
                currentSprite.SetFloat("_DissolveValue", dissolveValue)));
        }

        yield return new WaitForSeconds(2);
        
        
    }

    
    public IEnumerator QuitRegion()
    {
        if (!isGlobal)
        {
            for (int i = 0; i < regionSpriteRenderers.Count; i++)
            {
                float dissolveValue = 0;
                Material currentSprite = regionSpriteRenderers[i].material;
            
                DOTween.To(() => dissolveValue, x => dissolveValue = x, 1, 2f).OnUpdate((() =>
                    currentSprite.SetFloat("_DissolveValue", dissolveValue)));
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
            regionTexts[i].enabled = false;

            float dissolveValue = 0;
            Material currentSprite = regionTexts[i].material;
            
            DOTween.To(() => dissolveValue, x => dissolveValue = x, 1, 2f).OnUpdate((() =>
                currentSprite.SetFloat("_DissolveValue", dissolveValue)));
        } 
        
        for (int i = 0; i < regionImages.Count; i++)
        {
            regionImages[i].enabled = false;

            float dissolveValue = 0;
            Material currentSprite = regionImages[i].material;

            DOTween.To(() => dissolveValue, x => dissolveValue = x, 1, 2f).OnUpdate((() =>
                currentSprite.SetFloat("_DissolveValue", dissolveValue)));
        } 
        
        yield return new WaitForSeconds(2);

        if(!isGlobal)
            gameObject.SetActive(false);
    }
}
