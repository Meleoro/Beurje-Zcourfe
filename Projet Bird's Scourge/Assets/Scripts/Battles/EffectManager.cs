using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EffectMaker 
{
    public IEnumerator SquishTransform(Transform currentTransform, float squishStrength, float squishDuration)
    {
        Vector3 originalScale = currentTransform.localScale;
        
        currentTransform.DOScaleX(currentTransform.localScale.x * squishStrength, squishDuration);
        currentTransform.DOScaleY(currentTransform.localScale.x * squishStrength * 1.1f, squishDuration);
        
        yield return new WaitForSeconds(squishDuration);

        currentTransform.DOScale(originalScale, squishDuration * 0.8f);
    }
}
