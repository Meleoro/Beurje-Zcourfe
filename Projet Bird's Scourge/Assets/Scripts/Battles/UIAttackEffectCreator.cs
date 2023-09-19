using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIAttackEffectCreator
{
    public void SpriteEffect1(RectTransform currentRectTransform, RectTransform currentParentRectTransform, float duration, Vector2 newPos, float newSize, float newRot)
    {
        currentRectTransform.DOLocalMoveX(currentRectTransform.localPosition.x + newPos.x, duration).SetEase(Ease.OutQuad);

        currentRectTransform.DOLocalRotate(currentRectTransform.rotation.eulerAngles + new Vector3(0, 0, newRot), duration).SetEase(Ease.OutQuad);

        currentRectTransform.DOScale(Vector3.one * newSize, duration);
    }
    
    
    public IEnumerator SpriteEffect2(RectTransform currentRectTransform, RectTransform currentParentRectTransform, float duration, Vector2 newPos, float newSize, float newRot, float shakeAmplitude, int shakeVibrato,
        UIBattleAttack.CompetenceType currentCompetenceType)
    {
        float duration1 = 0.15f;
        float duration2 = 0.85f;
        
        
        if (currentCompetenceType != UIBattleAttack.CompetenceType.miss)
        {
            currentParentRectTransform.DOShakePosition(duration * duration1, new Vector3(1.2f, 1, 0) * shakeAmplitude, shakeVibrato);
        }
        
        
        currentRectTransform.DOLocalMoveX(currentRectTransform.localPosition.x + newPos.x * duration2, duration * duration1)
            .SetEase(Ease.Linear);
            
        currentRectTransform.DOLocalRotate(currentRectTransform.rotation.eulerAngles + new Vector3(0, 0, newRot), duration).SetEase(Ease.OutQuad);

        currentRectTransform.DOScale(Vector3.one * newSize, duration);

        yield return new WaitForSeconds(duration * duration1);
        
        
        if (currentCompetenceType != UIBattleAttack.CompetenceType.miss)
        {
            currentParentRectTransform.DOShakePosition(duration * duration2, new Vector3(1.2f, 1, 0) * shakeAmplitude, (int)(shakeVibrato * 0.3f));
        }
        
        
        currentRectTransform.DOLocalMoveX(currentRectTransform.localPosition.x + newPos.x * duration1,
            duration * duration2).SetEase(Ease.OutQuad); 
    }
    
    
    public void ChangeColor(Image currentImage, Color newColor, float duration)
    {
        Color colorAttacker = currentImage.material.GetColor("_Color");
        DOTween.To(() => colorAttacker, x => colorAttacker = x, newColor, duration)
            .OnUpdate(() => {
                currentImage.material.SetColor("_Color", colorAttacker);
            });
    }
}
