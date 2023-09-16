using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIBattleAttack : MonoBehaviour
{
    [Header("Start")] 
    public float apparitionFadeDuration;

    [Header("Shake")] 
    public bool unifiedShake;   // If true, the parent transform also shakes
    [Range(0f, 5f)] public float attackerShakeDuration;
    [Range(0f, 15f)] public float attackerShakeAmplitude;
    [Range(0, 25)] public int attackerShakeVibrato;
    [Range(0f, 5f)] public float attackedShakeDuration;
    [Range(0f, 15f)] public float attackedShakeAmplitude;
    [Range(0, 25)] public int attackedShakeVibrato;

    [Header("Movement")]
    [Range(-150f, 150f)] public float attackerMovement = 0;
    [Range(0f, 2f)] public float attackerMovementDuration = 0;
    public Ease attackerMovementEase;
    [Range(-150f, 150f)] public float attackedMovement = 0;
    [Range(0f, 2f)] public float attackedMovementDuration = 0;
    public Ease attackedMovementEase;
    private float originalXLeft;
    private float originalXRight;
    private float originalYLeft;
    private float originalYRight;

    [Header("Rotation")]
    [Range(-20f, 20f)] public float attackerRotation = 0;
    [Range(0f, 2f)] public float attackerRotationDuration = 0;
    public Ease attackerRotationEase;
    [Range(-20f, 20f)] public float attackedRotation = 0;
    [Range(0f, 2f)] public float attackedRotationDuration = 0;
    public Ease attackedRotationEase;

    [Header("Scale")]
    [Range(0f, 2f)] public float attackerScale = 1;
    [Range(0f, 2f)] public float attackerScaleDuration = 0;
    [Range(0f, 2f)] public float attackedScale = 1;
    [Range(0f, 2f)] public float attackedScaleDuration = 0;

    [Header("Sprites Colors")]
    public Color colorStart;
    public Color colorStandard;
    public Color colorEnd;
    public Color colorDamage;
    public Color colorHeal;
    public Color colorSummon;
    [Range(0f, 2f)] public float fadeColorStartDuration = 0.2f;
    [Range(0f, 2f)] public float fadeColorEndDuration = 0.1f;
    [Range(0f, 2f)] public float flickerColorDuration = 0.3f;


    [Header("Text Colors")] 
    public Color colorNormalAttack;
    public Color colorMissAttack;
    public Color colorCritAttack;
    public Color colorNormalHealText;
    public Color colorCritHealText;
    public Color colorSummonText;

    [Header("Text Feel")]
    [Range(0, 800)] public float textXOrigin;
    [Range(0, 800)] public float textXEnd;
    [Range(0, 300)] public float textYOrigin;
    [Range(0, 300)] public float textYEnd;
    [Range(0f, 3f)] public float textOriginalSize;
    [Range(0f, 3f)] public float textEndSize;
    [Range(0, 360)] public float textOriginalRot;
    [Range(0, 360)] public float textEndRot;
    [Range(0f, 1f)] public float textMoveDuration;
    [Range(0f, 1f)] public float textFadeStart;    // Time before the fade out start
    [Range(0f, 1f)] public float textFadeDuration;
    public Ease textMoveEase;
    public Ease textRotateEase;


    [Header("Other")]
    /*float currentWidthRatio;
    float currentHeightRatio;*/
    float currentWidthOffset;
    float currentHeightOffset;

    public enum CompetenceType
    {
        attack,
        attackCrit,
        miss,
        heal,
        healCrit,
        summon,
        buff
    }

    
    [Header("Prefabs")] 
    public GameObject imageParentObject;
    public GameObject textObject;

    [Header("References")] 
    public TextMeshProUGUI damageNumber;
    public RectTransform attackUIParent;
    public RectTransform attackUI;
    public RectTransform leftCharaParent;
    public RectTransform rightCharaParent;
    public Image leftChara;
    public Image rightChara;
    public Image attackFond;
    public GameObject ghostPrefab;
    public Transform ghostParentLeft;
    public Transform ghostParentRight;
    private UIAttackEffectCreator effectCreator;
    
    private RectTransform currentImageParent;
    private Image currentImage;
    private TextMeshProUGUI currentTMPRO;


    public void Start()
    {
        attackUI.gameObject.SetActive(false);

        originalXLeft = leftCharaParent.localPosition.x;
        originalXRight = rightCharaParent.localPosition.x;

        originalYLeft = leftCharaParent.localPosition.y;
        originalYRight = rightCharaParent.localPosition.y;
        
        leftCharaParent.gameObject.SetActive(false);
        rightChara.gameObject.SetActive(false);

        effectCreator = new UIAttackEffectCreator();
    }


    // SETUP EVERY ALPHAS WHEN THE SCENE START
    public void AttackUISetup()
    {
        attackFond.DOFade(0, 0);

        leftChara.material.SetFloat("_Alpha", 0);
        rightChara.material.SetFloat("_Alpha", 0);

        leftChara.material.SetColor("_Color", colorStart);
        rightChara.material.SetColor("_Color", colorStart);

        attackUI.DORotate(Vector3.zero, 0);
        leftCharaParent.DORotate(Vector3.zero, 0);
        rightCharaParent.DORotate(Vector3.zero, 0);

        leftCharaParent.DOScale(Vector3.one, 0);
        rightCharaParent.DOScale(Vector3.one, 0);

        attackUI.localScale = Vector3.one;


        damageNumber.rectTransform.localScale = Vector3.one * textOriginalRot;
    }

    
    public void LaunchAttack(List<DataUnit> leftDatas, List<DataUnit> rightDatas, CompetenceType currentCompetenceType, bool leftOrigin, bool deadEnnemy, int damage, DataCompetence.VFXTypes currentVFXType, Buff createdBuff)
    {
        List<Vector2> leftPos = new List<Vector2>();
        List<Vector2> rightPos = new List<Vector2>();

        float offset = 50;

        for (int i = 0; i < leftDatas.Count; i++)
        {
            leftPos.Add(new Vector2(originalXLeft + i * offset, originalYLeft));
        }
        
        for (int i = 0; i < rightDatas.Count; i++)
        {
            rightPos.Add(new Vector2(originalXRight - i * offset, originalYRight));
        }
        

        for (int i = 0; i < leftDatas.Count; i++)
        {
            StartCoroutine(CharacterFeel(leftDatas[i], leftPos[i], true, leftOrigin, deadEnnemy,
                currentCompetenceType, currentVFXType));
        }
        
        for (int i = 0; i < rightDatas.Count; i++)
        {
            StartCoroutine(CharacterFeel(rightDatas[i], rightPos[i], false, leftOrigin, deadEnnemy,
                currentCompetenceType, currentVFXType));
        }
        
        StartCoroutine(TextFeel(leftOrigin, damage, currentCompetenceType, createdBuff));
    }
    
    
    // WHEN THE ATTACK UI HAS TO APPEAR
    public IEnumerator CharacterFeel(DataUnit currentData, Vector2 currentPos, bool isLeft, bool leftOrigin, bool deadEnnemy, CompetenceType currentCompetenceType, DataCompetence.VFXTypes VFXType)
    {
        if ((leftOrigin && isLeft) || (!leftOrigin && !isLeft))
        {
            SetupFeel(currentData.attackSprite, currentData, currentPos);
            StartCoroutine(GhostTrail(10, 0.04f, 0.1f, currentCompetenceType, true));
        }
        else
        {
            SetupFeel(currentData.damageSprite, currentData, currentPos);
            StartCoroutine(GhostTrail(10, 0.04f, 0.1f, currentCompetenceType, false));
        }
        
        StartCoroutine(CharacterFeel(leftOrigin, isLeft, currentData, currentCompetenceType, deadEnnemy));
        
        LaunchVFX(leftOrigin, VFXType);

        yield return new WaitForSeconds(1.3f);

        StartCoroutine(EndFeel(isLeft));
    }
    
    
    // SETUP THE DIFFERENT ELEMENTS
    public void SetupFeel(Sprite currentSprite, DataUnit currentData, Vector2 wantedPos)
    {
        CameraManager.Instance.canMove = false;
        UIBattleManager.Instance.buttonScript.SwitchButtonInteractible(false);
        attackUI.gameObject.SetActive(true);

        leftChara.gameObject.SetActive(true);
        rightChara.gameObject.SetActive(true);

        currentImageParent = Instantiate(imageParentObject, wantedPos, Quaternion.identity, attackUI).GetComponent<RectTransform>();
        currentImage = currentImageParent.GetComponentInChildren<Image>();
        
        currentImage.material = Instantiate(currentImage.material);
        
        Destroy(currentImageParent.gameObject, 3);

        
        currentImage.gameObject.SetActive(true);
        currentImage.sprite = currentSprite;
        
        currentWidthOffset = 800 * currentData.XPosModificator;
        currentHeightOffset = 300 * currentData.YPosModificator;
        
        currentImageParent.localScale = Vector3.one * currentData.attackSpriteSize;
        currentImageParent.localPosition = new Vector3(wantedPos.x + currentWidthOffset, wantedPos.y + currentHeightOffset, currentImageParent.position.z);

        
        attackFond.DOFade(0.8f, apparitionFadeDuration);

        float fade = currentImage.material.GetFloat("_Alpha");
        Image imageToModify = currentImage;
        
        DOTween.To(() => fade, x => fade = x, 1, fadeColorStartDuration * 0.25f)
            .OnUpdate(() => {
                imageToModify.material.SetFloat("_Alpha", fade);
            });

        imageToModify.material.SetFloat("_DissolveValue", 0);
    }


    // GENERATE THE MOVEMENT OF THE CHARACTERS
    public IEnumerator CharacterFeel(bool leftOrigin, bool isLeft, DataUnit currentData, CompetenceType currentCompetenceType, bool deathBlow)
    {
        float rightModificator = -1;

        if (isLeft)
            rightModificator = 1;
            
            
        float shakeDuration = attackedShakeDuration;
        float shakeAmplitude = attackedShakeAmplitude;
        int shakeVibrato = attackedShakeVibrato;

        float movementValue = attackedMovement;
        float scaleValue = attackedScale;
        float rotValue = attackedRotation;

        Ease currentMovementEase = attackedMovementEase;
        Ease currentRotEase = attackedRotationEase;

        bool isAttacker = false;

        if ((isLeft && leftOrigin) || (!isLeft && !leftOrigin))
        {
            shakeDuration = attackerShakeDuration;
            shakeAmplitude = attackerShakeAmplitude;
            shakeVibrato = attackerShakeVibrato;
            
            movementValue = attackerMovement;
            scaleValue = attackerScale;
            rotValue = attackerRotation;
            
            currentMovementEase = attackerMovementEase;
            currentRotEase = attackerRotationEase;

            isAttacker = true;
        }
        
            
        if (CompetenceType.miss != currentCompetenceType)
        {
            if(CompetenceType.attackCrit == currentCompetenceType)
            {
                currentImageParent.DOShakePosition(shakeDuration, new Vector3(1.2f, 1, 0) * (shakeAmplitude), shakeVibrato);
            }
            else
            {
                currentImageParent.DOShakePosition(shakeDuration, new Vector3(1, 1, 0) * (shakeAmplitude), shakeVibrato);
            }
        }

        Vector2 newPos = new Vector2(movementValue * rightModificator + currentWidthOffset, 0);
        float newSize = scaleValue * currentData.attackSpriteSize;
        float newRot = rotValue * rightModificator;

        effectCreator.SpriteEffect1(currentImageParent, attackerMovementDuration, newPos, newSize, newRot, currentMovementEase, currentRotEase);


        Image imageToModify = currentImage;
        if (isAttacker)
        {
            effectCreator.ChangeColor(imageToModify, colorStandard, fadeColorStartDuration);
        }
        else
        {
            if (!deathBlow)
            {
                Color wantedColor = colorDamage;

                if (CompetenceType.miss == currentCompetenceType)
                    wantedColor = colorMissAttack;

                else if (CompetenceType.attackCrit == currentCompetenceType)
                    wantedColor = colorCritAttack;

                else if (CompetenceType.heal == currentCompetenceType)
                    wantedColor = colorHeal;

            
                effectCreator.ChangeColor(imageToModify, wantedColor, flickerColorDuration);

                yield return new WaitForSeconds(flickerColorDuration);
            
                effectCreator.ChangeColor(imageToModify, colorStandard, flickerColorDuration);
            }

            else
            {
                Color wantedColor = colorDamage;

                effectCreator.ChangeColor(imageToModify, wantedColor, flickerColorDuration);

                yield return new WaitForSeconds(flickerColorDuration);
            
                float dissoveValue = imageToModify.material.GetFloat("_DissolveValue");
                DOTween.To(() => dissoveValue, x => dissoveValue = x, 1, flickerColorDuration * 3)
                    .OnUpdate(() => {
                        imageToModify.material.SetFloat("_DissolveValue", dissoveValue);
                    });
            
                effectCreator.ChangeColor(imageToModify, colorStandard, flickerColorDuration);
            }
        }
    }
    
    
    // MANAGE THE DAMAGE TEXT
    public IEnumerator TextFeel(bool leftOrigin, int damage, CompetenceType currentCompetenceType, Buff currentBuff)
    {
        Vector3 posLeftBottomCorner = new Vector3(-attackUIParent.rect.width * 0.5f, -attackUIParent.rect.height * 0.5f, 0);
        float healModificator = 1;

        if (currentCompetenceType == CompetenceType.heal || currentCompetenceType == CompetenceType.summon || currentCompetenceType == CompetenceType.buff)
        {
            healModificator = 0.8f;
        }

        if (leftOrigin)
        {
            Vector3 pos1 = new Vector3(Mathf.Lerp(posLeftBottomCorner.x, -posLeftBottomCorner.x, textXOrigin / 800), Mathf.Lerp(posLeftBottomCorner.y, -posLeftBottomCorner.y, textYOrigin / 300), 0);
            Vector3 pos2 = new Vector3(Mathf.Lerp(posLeftBottomCorner.x, -posLeftBottomCorner.x, textXEnd * healModificator / 800), Mathf.Lerp(posLeftBottomCorner.y, -posLeftBottomCorner.y, textYEnd / 300), 0);
            
            damageNumber.rectTransform.localPosition = pos1;
            damageNumber.rectTransform.rotation = Quaternion.Euler(0, 0, -textOriginalRot);
            damageNumber.rectTransform.localScale = Vector3.one * textOriginalSize;

            damageNumber.rectTransform.DOLocalMoveX(pos2.x, textMoveDuration).SetEase(textMoveEase);
            damageNumber.rectTransform.DOLocalMoveY(pos2.y, textMoveDuration).SetEase(textMoveEase);

            damageNumber.rectTransform.DOScale(Vector3.one * textEndSize, textMoveDuration);

            damageNumber.rectTransform.DORotate(new Vector3(0, 0, -textEndRot * healModificator), textMoveDuration).SetEase(textRotateEase);
        }
        else
        {
            Vector3 pos1 = new Vector3(Mathf.Lerp(-posLeftBottomCorner.x, posLeftBottomCorner.x, textXOrigin / 800), Mathf.Lerp(posLeftBottomCorner.y, -posLeftBottomCorner.y, textYOrigin / 300), 0);
            Vector3 pos2 = new Vector3(Mathf.Lerp(-posLeftBottomCorner.x, posLeftBottomCorner.x, textXEnd * healModificator / 800), Mathf.Lerp(posLeftBottomCorner.y, -posLeftBottomCorner.y, textYEnd / 300), 0);
            
            damageNumber.rectTransform.localPosition = pos1;
            damageNumber.rectTransform.rotation = Quaternion.Euler(0, 0, textOriginalRot);
            damageNumber.rectTransform.localScale = Vector3.one * textOriginalSize;

            damageNumber.rectTransform.DOLocalMoveX(pos2.x, textMoveDuration).SetEase(textMoveEase);
            damageNumber.rectTransform.DOLocalMoveY(pos2.y, textMoveDuration).SetEase(textMoveEase);

            damageNumber.rectTransform.DOScale(Vector3.one * textEndSize, textMoveDuration);

            damageNumber.rectTransform.DORotate(new Vector3(0, 0, textEndRot * healModificator), textMoveDuration).SetEase(textRotateEase);
        }


        switch (currentCompetenceType)
        {
            case CompetenceType.summon : 
                damageNumber.text = "Summoned";
                damageNumber.color = colorSummonText;
                break;
            
            case CompetenceType.heal :
                damageNumber.text = damage.ToString();
                damageNumber.color = colorNormalHealText;
                break;
            
            case CompetenceType.healCrit :
                damageNumber.text = "CRIT " + damage.ToString();
                damageNumber.color = colorCritHealText;
                break;
            
            case CompetenceType.miss : 
                damageNumber.text = "Miss";
                damageNumber.color = colorMissAttack;
                break;
            
            case CompetenceType.attack :
                damageNumber.text = damage.ToString();
                damageNumber.color = colorNormalAttack;
                break;
            
            case CompetenceType.attackCrit :
                damageNumber.text = "CRIT " + damage.ToString();
                damageNumber.color = colorCritAttack;
                break;
            
            case CompetenceType.buff :
                switch (currentBuff.buffType)
                {
                    case (BuffManager.BuffType.damage) :
                        damageNumber.text = "Attack + " + currentBuff.buffValue.ToString() + "%";
                        break;
                
                    case (BuffManager.BuffType.accuracy) :
                        damageNumber.text = "Accuracy + " + currentBuff.buffValue.ToString() + "%";
                        break;
                
                    case (BuffManager.BuffType.crit) :
                        damageNumber.text = "Crit + " + currentBuff.buffValue.ToString() + "%";
                        break;
                
                    case (BuffManager.BuffType.defense) :
                        damageNumber.text = "Defense + " + currentBuff.buffValue.ToString() + "%";
                        break;
                }
                damageNumber.color = colorCritAttack;
                break;
        }

        yield return new WaitForSeconds(textFadeStart);

        damageNumber.DOFade(0, textFadeDuration);
    }


    // FADES OUT OF THE ATTACK UI
    public IEnumerator EndFeel(bool isLeft)
    {
        Image imageToModify = currentImage;
        
        float fadeLeft = imageToModify.material.GetFloat("_Alpha");
        DOTween.To(() => fadeLeft, x => fadeLeft = x, 0.5f, fadeColorEndDuration)
            .OnUpdate(() => {
                imageToModify.material.SetFloat("_Alpha", fadeLeft);
            });
        
        Color colorLeft = imageToModify.material.GetColor("_Color");
        DOTween.To(() => colorLeft, x => colorLeft = x, colorEnd, fadeColorEndDuration)
            .OnUpdate(() => {
                imageToModify.material.SetColor("_Color", colorLeft);
            });


        if (isLeft)
        {
            currentImageParent.DOLocalMoveX(currentImageParent.localPosition.x + (-50), fadeColorEndDuration).SetEase(attackerMovementEase);
            currentImageParent.DOScale(currentImageParent.localScale * 0.8f, fadeColorEndDuration).SetEase(attackerMovementEase);
        }
        
        else
        {
            currentImageParent.DOLocalMoveX(currentImageParent.localPosition.x + (50), fadeColorEndDuration).SetEase(attackerMovementEase);
            currentImageParent.DOScale(currentImageParent.localScale * 0.8f, fadeColorEndDuration).SetEase(attackerMovementEase);
        }
        
        attackFond.DOFade(0f, fadeColorEndDuration + 0.01f);
        
        
        yield return new WaitForSeconds(fadeColorEndDuration * 0.5f);

        CameraManager.Instance.ExitCameraBattle();
        
        
        yield return new WaitForSeconds(fadeColorEndDuration * 0.5f + 0.01f);

        
        AttackUISetup();
        attackUI.gameObject.SetActive(false);
        MouseManager.Instance.noControl = false;

        if (BattleManager.Instance.order[0].CompareTag("Unit"))
        {
            CameraManager.Instance.canMove = true;
            UIBattleManager.Instance.buttonScript.SwitchButtonInteractible(true);
        }
        else if (BattleManager.Instance.order[0].CompareTag("Ennemy"))
        {
            UIBattleManager.Instance.buttonScript.SwitchButtonInteractible(false);
        }
    }


    // CREATES THE GHOST TRAIL ON THE ATTACKED UNIT
    public IEnumerator GhostTrail(int iterations, float durationBetween, float ghostDuration, CompetenceType currentCompetenceType, bool isKind)
    {
        Image imageToModify = currentImage;
        
        while (iterations > 0)
        {
            iterations -= 1;

            GameObject currentPrefab = null;
            Image currentGhost = null;
            
            currentPrefab = Instantiate(ghostPrefab, imageToModify.rectTransform.position, Quaternion.identity, ghostParentRight);

            currentGhost = currentPrefab.GetComponent<Image>();
            currentGhost.material = Instantiate(currentGhost.material);
            
            currentGhost.sprite = imageToModify.sprite;

            if (!isKind)
            {
                Color wantedColor = colorDamage;

                if (currentCompetenceType == CompetenceType.miss)
                    wantedColor = colorMissAttack;

                if (currentCompetenceType == CompetenceType.attackCrit)
                    wantedColor = colorCritAttack;
                
                currentGhost.material.SetColor("_Color", wantedColor);
            }
            else
            {
                currentGhost.material.SetColor("_Color", Color.white);
            }

            currentGhost.DOFade(1, ghostDuration).OnComplete(() => { currentGhost.DOFade(0, 0.2f); });

            yield return new WaitForSeconds(durationBetween);
        }
    }
    
    
    // MANAGES THE VFX TO LAUNCH
    private void LaunchVFX(bool leftOrigin, DataCompetence.VFXTypes currentVFXType)
    {
        Vector2 wantedPos = leftCharaParent.position;
        if (leftOrigin)
            wantedPos = rightCharaParent.position;

        if (currentVFXType == DataCompetence.VFXTypes.bam)
        {
            UIVfxManager.Instance.DOBam(wantedPos, leftOrigin);
        }
        
        else if (currentVFXType == DataCompetence.VFXTypes.slash)
        {
            UIVfxManager.Instance.DOSlash(wantedPos, leftOrigin);
        }
        
        else if (currentVFXType == DataCompetence.VFXTypes.heal)
        {
            UIVfxManager.Instance.DoHeal(ghostParentLeft.GetComponent<RectTransform>(), ghostParentRight.GetComponent<RectTransform>(), leftOrigin);
        }
    }
}
