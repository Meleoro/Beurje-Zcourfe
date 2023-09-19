using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIAttackEffectCreator
{
    public UIBattleAttack mainScript;
    
    
    public void SpriteEffect1(RectTransform currentRectTransform, RectTransform currentParentRectTransform, float duration, Vector2 newPos, float newSize, float newRot)
    {
        currentRectTransform.DOLocalMoveX(currentRectTransform.localPosition.x + newPos.x, duration).SetEase(Ease.OutQuad);

        currentRectTransform.DOLocalRotate(currentRectTransform.rotation.eulerAngles + new Vector3(0, 0, newRot), duration).SetEase(Ease.OutQuad);

        currentRectTransform.DOScale(Vector3.one * newSize, duration);
    }
    
    
    public IEnumerator SpriteEffect2(RectTransform currentRectTransform, RectTransform currentParentRectTransform, float duration, Vector2 newPos, float newSize, float newRot, float shakeAmplitude, int shakeVibrato,
        UIBattleAttack.CompetenceType currentCompetenceType, UIBattleAttack currentMainScript)
    {
        mainScript = currentMainScript;
        
        float duration1 = 0.18f;
        float duration2 = 0.82f;
        
        
        if (currentCompetenceType != UIBattleAttack.CompetenceType.miss)
        {
            mainScript.attackUI.DOShakePosition(duration * duration2 * 0.5f, new Vector3(1, 1, 0) * (shakeAmplitude * 2f));
        }
        
        
        currentRectTransform.DOLocalMoveX(currentRectTransform.localPosition.x + newPos.x * duration2, duration * duration1)
            .SetEase(Ease.Linear);
            
        currentRectTransform.DOLocalRotate(currentRectTransform.rotation.eulerAngles + new Vector3(0, 0, newRot), duration).SetEase(Ease.OutQuad);

        currentRectTransform.DOScale(Vector3.one * newSize, duration);

        yield return new WaitForSeconds(duration * duration1);
        
        
        if (currentCompetenceType != UIBattleAttack.CompetenceType.miss)
        {
            currentParentRectTransform.DOShakePosition(duration * duration2 * 0.5f, new Vector3(1.2f, 1, 0) * (shakeAmplitude * 1.5f), (int)(shakeVibrato * 1.5f));
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
