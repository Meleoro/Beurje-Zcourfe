using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIAttackEffectCreator
{
    public void SpriteEffect1(RectTransform currentRectTransform, float duration, Vector2 newPos, float newSize, float newRot, Ease attackerMovementEase, Ease attackerRotationEase)
    {
        currentRectTransform.DOLocalMoveX(currentRectTransform.localPosition.x + newPos.x, duration).SetEase(attackerMovementEase);

        currentRectTransform.DOLocalRotate(currentRectTransform.rotation.eulerAngles + new Vector3(0, 0, newRot), duration).SetEase(attackerRotationEase);

        currentRectTransform.DOScale(Vector3.one * newSize, duration);
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
