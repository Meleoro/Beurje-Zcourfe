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
    float currentWidthRatio;
    float currentHeightRatio;
    float attackerWidthOffset;
    float attackerHeightOffset;
    float attackedWidthOffset;
    float attackedHeightOffset;

    public enum CompetenceType
    {
        attack,
        attackCrit,
        miss,
        heal,
        summon,
        buff
    }


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


    public void Start()
    {
        attackUI.gameObject.SetActive(false);

        originalXLeft = leftCharaParent.localPosition.x;
        originalXRight = rightCharaParent.localPosition.x;

        originalYLeft = leftCharaParent.localPosition.y;
        originalYRight = rightCharaParent.localPosition.y;
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
    
    
    // WHEN THE ATTACK UI HAS TO APPEAR
    public IEnumerator AttackUIFeel(DataUnit leftData, DataUnit rightData, bool leftOrigin, int damage, bool miss, bool crit, bool deadEnnemy)
    {
        CompetenceType currentCompetenceType = CompetenceType.attack;

        if (miss) currentCompetenceType = CompetenceType.miss;

        else if (crit) currentCompetenceType = CompetenceType.attackCrit;


        if (leftOrigin)
            SetupFeel(leftData.attackSprite, rightData.damageSprite, leftData, rightData, leftOrigin);
        
        else
            SetupFeel(leftData.damageSprite, rightData.attackSprite, leftData, rightData, leftOrigin);
        
        StartCoroutine(CharacterFeel(leftOrigin, leftData, rightData, currentCompetenceType, deadEnnemy));

        StartCoroutine(TextFeel(leftOrigin, miss, crit, damage, false, false));

        StartCoroutine(GhostTrail(10, 0.04f, 0.1f, leftOrigin, currentCompetenceType));
        
        LaunchVFX(leftOrigin, currentCompetenceType);

        yield return new WaitForSeconds(1.3f);

        StartCoroutine(EndFeel());
    }
    
    
    // WHEN THE SUMMON UI HAS TO APPEAR
    public IEnumerator SummonUIFeel(DataUnit leftData, DataUnit rightData, bool leftOrigin)
    {
        CompetenceType currentCompetenceType = CompetenceType.summon;

        SetupFeel(leftData.attackSprite, rightData.attackSprite, leftData, rightData, leftOrigin);

        StartCoroutine(CharacterFeelHeal(leftOrigin, leftData, rightData, currentCompetenceType));

        StartCoroutine(TextFeel(false, false, false, 0, true, false));
        
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(EndFeel());
    }
    
    
    // WHEN THE BUFF / HEAL UI HAS TO APPEAR
    public IEnumerator HealUIFeel(DataUnit leftData, DataUnit rightData, bool leftOrigin, int healValue, bool miss, bool crit)
    {
        CompetenceType currentCompetenceType = CompetenceType.heal;

        if (leftOrigin)
            SetupFeel(leftData.attackSprite, rightData.damageSprite, leftData, rightData, leftOrigin);

        else
            SetupFeel(leftData.damageSprite, rightData.attackSprite, leftData, rightData, leftOrigin);

        StartCoroutine(CharacterFeelHeal(leftOrigin, leftData, rightData, currentCompetenceType));

        StartCoroutine(TextFeel(leftOrigin, miss, crit, healValue, false, true));
        
        LaunchVFX(leftOrigin, currentCompetenceType);

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(EndFeel());
    }
    
    
    
    // SETUP THE DIFFERENT ELEMENTS
    public void SetupFeel(Sprite leftSprite, Sprite rightSprite, DataUnit leftData, DataUnit rightData, bool leftOrigin)
    {
        currentWidthRatio = CameraManager.Instance.screenWidth / 800;
        currentHeightRatio = CameraManager.Instance.screenHeight / 300;

        CameraManager.Instance.canMove = false;
        UIBattleManager.Instance.buttonScript.SwitchButtonInteractible(false);
        attackUI.gameObject.SetActive(true);

        leftChara.gameObject.SetActive(true);
        rightChara.gameObject.SetActive(true);

        leftChara.sprite = leftSprite;
        rightChara.sprite = rightSprite;

        leftCharaParent.localScale = Vector3.one * leftData.attackSpriteSize;
        rightCharaParent.localScale = Vector3.one * rightData.attackSpriteSize;

        if (leftOrigin)
        {
            attackerWidthOffset = 800 * leftData.XPosModificator;
            attackerHeightOffset = 300 * leftData.YPosModificator;

            attackedWidthOffset = 800 * rightData.XPosModificator;
            attackedHeightOffset = 300 * rightData.YPosModificator;

            leftCharaParent.localPosition = new Vector3(originalXLeft + attackerWidthOffset, originalYLeft + attackerHeightOffset, leftCharaParent.position.z);
            rightCharaParent.localPosition = new Vector3(originalXRight + attackedWidthOffset, originalYRight + attackedHeightOffset, rightCharaParent.position.z);
        }
        else
        {
            attackerWidthOffset = 800 * rightData.XPosModificator;
            attackerHeightOffset = 300 * rightData.YPosModificator;

            attackedWidthOffset = 800 * leftData.XPosModificator;
            attackedHeightOffset = 300 * leftData.YPosModificator;

            leftCharaParent.localPosition = new Vector3(originalXLeft + attackedWidthOffset, originalYLeft + attackedHeightOffset, leftCharaParent.position.z);
            rightCharaParent.localPosition = new Vector3(originalXRight + attackerWidthOffset, originalYRight + attackerHeightOffset, rightCharaParent.position.z);
        }

        attackFond.DOFade(0.8f, apparitionFadeDuration);

        float fadeLeft = leftChara.material.GetFloat("_Alpha");
        DOTween.To(() => fadeLeft, x => fadeLeft = x, 1, fadeColorStartDuration * 0.25f)
            .OnUpdate(() => {
                leftChara.material.SetFloat("_Alpha", fadeLeft);
            });

        float fadeRight = rightChara.material.GetFloat("_Alpha");
        DOTween.To(() => fadeRight, x => fadeRight = x, 1, fadeColorStartDuration * 0.25f)
            .OnUpdate(() => {
                rightChara.material.SetFloat("_Alpha", fadeRight);
            });
        
        leftChara.material.SetFloat("_DissolveValue", 0);
        rightChara.material.SetFloat("_DissolveValue", 0);
    }


    // GENERATE THE MOVEMENT OF THE CHARACTERS
    public IEnumerator CharacterFeel(bool leftOrigin, DataUnit leftData, DataUnit rightData, CompetenceType currentCompetenceType, bool deathBlow)
    {
        RectTransform attackerParent = rightCharaParent;
        Image attackerImage = rightChara;
        DataUnit attackerData = rightData;

        RectTransform attackedParent = leftCharaParent;
        Image attackedImage = leftChara;
        DataUnit attackedData = leftData;

        int rightModificator = -1;


        if (leftOrigin)
        {
            attackerParent = leftCharaParent;
            attackerImage = leftChara;
            attackerData = leftData;

            attackedParent = rightCharaParent;
            attackedImage = rightChara;
            attackedData = rightData;

            rightModificator = 1;
        }
        
        

        if (CompetenceType.miss != currentCompetenceType)
        {
            if(CompetenceType.attackCrit == currentCompetenceType)
            {
                attackerImage.rectTransform.DOShakePosition(attackerShakeDuration, new Vector3(1.2f, 1, 0) * (attackedShakeAmplitude * currentWidthRatio), attackerShakeVibrato);
                attackedImage.rectTransform.DOShakePosition(attackedShakeDuration, new Vector3(1.2f, 1, 0) * (attackedShakeAmplitude * currentWidthRatio), attackedShakeVibrato);
            }
            else
            {
                attackerImage.rectTransform.DOShakePosition(attackerShakeDuration, new Vector3(1, 1, 0) * (attackedShakeAmplitude * currentWidthRatio), attackerShakeVibrato);
                attackedImage.rectTransform.DOShakePosition(attackedShakeDuration, new Vector3(1, 1, 0) * (attackedShakeAmplitude * currentWidthRatio), attackedShakeVibrato);
            }
        }

        attackerParent.DOLocalMoveX(attackerParent.localPosition.x + (attackerMovement * rightModificator * currentWidthRatio) + attackerWidthOffset, attackerMovementDuration).SetEase(attackerMovementEase);
        attackedParent.DOLocalMoveX(attackedParent.localPosition.x + (attackedMovement * rightModificator * currentWidthRatio) + attackedWidthOffset, attackedMovementDuration).SetEase(attackedMovementEase);

        attackerParent.DOLocalRotate(attackerParent.rotation.eulerAngles + new Vector3(0, 0, attackerRotation * rightModificator), attackerRotationDuration).SetEase(attackerRotationEase);
        attackedParent.DOLocalRotate(attackedParent.rotation.eulerAngles + new Vector3(0, 0, attackedRotation * rightModificator), attackedRotationDuration).SetEase(attackedRotationEase);

        attackerParent.DOScale(Vector3.one * (attackerScale * attackerData.attackSpriteSize), attackerScaleDuration);
        attackedParent.DOScale(Vector3.one * (attackedScale * attackedData.attackSpriteSize), attackedScaleDuration);

        Color colorAttacker = attackerImage.material.GetColor("_Color");
        DOTween.To(() => colorAttacker, x => colorAttacker = x, colorStandard, fadeColorStartDuration)
            .OnUpdate(() => {
                attackerImage.material.SetColor("_Color", colorAttacker);
            });


        if (!deathBlow)
        {
            Color wantedColor = colorDamage;

            if (CompetenceType.miss == currentCompetenceType)
                wantedColor = colorMissAttack;

            else if (CompetenceType.attackCrit == currentCompetenceType)
                wantedColor = colorCritAttack;

            else if (CompetenceType.heal == currentCompetenceType)
                wantedColor = colorHeal;


            attackedImage.material.SetColor("_Color", colorStandard);
            Color colorAttacked = attackedImage.material.GetColor("_Color");
            DOTween.To(() => colorAttacked, x => colorAttacked = x, wantedColor, flickerColorDuration)
                .OnUpdate(() => {
                    attackedImage.material.SetColor("_Color", colorAttacked);
                });

            yield return new WaitForSeconds(flickerColorDuration);

            colorAttacked = attackedImage.material.GetColor("_Color");
            DOTween.To(() => colorAttacked, x => colorAttacked = x, colorStandard, flickerColorDuration)
                .OnUpdate(() => {
                    attackedImage.material.SetColor("_Color", colorAttacked);
                });
        }

        else
        {
            Color wantedColor = colorDamage;

            attackedImage.material.SetColor("_Color", colorStandard);
            Color colorAttacked = attackedImage.material.GetColor("_Color");
            DOTween.To(() => colorAttacked, x => colorAttacked = x, wantedColor, flickerColorDuration)
                .OnUpdate(() => {
                    attackedImage.material.SetColor("_Color", colorAttacked);
                });

            yield return new WaitForSeconds(flickerColorDuration);
            
            float dissoveValue = attackedImage.material.GetFloat("_DissolveValue");
            DOTween.To(() => dissoveValue, x => dissoveValue = x, 1, flickerColorDuration * 3)
                .OnUpdate(() => {
                    attackedImage.material.SetFloat("_DissolveValue", dissoveValue);
                });


            colorAttacked = attackedImage.material.GetColor("_Color");
            DOTween.To(() => colorAttacked, x => colorAttacked = x, colorEnd, flickerColorDuration)
                .OnUpdate(() => {
                    attackedImage.material.SetColor("_Color", colorAttacked);
                });
        }
    }

    public IEnumerator CharacterFeelHeal(bool leftOrigin, DataUnit leftData, DataUnit rightData, CompetenceType currentCompetenceType)
    {
        RectTransform attackerParent = rightCharaParent;
        Image attackerImage = rightChara;
        DataUnit attackerData = rightData;

        RectTransform attackedParent = leftCharaParent;
        Image attackedImage = leftChara;
        DataUnit attackedData = leftData;

        int rightModificator = -1;


        if (leftOrigin)
        {
            attackerParent = leftCharaParent;
            attackerImage = leftChara;
            attackerData = leftData;

            attackedParent = rightCharaParent;
            attackedImage = rightChara;
            attackedData = rightData;

            rightModificator = 1;
        }

        attackerParent.DOLocalMoveX(attackerParent.localPosition.x + (attackerMovement * rightModificator * currentWidthRatio) + attackerWidthOffset, attackerMovementDuration).SetEase(attackerMovementEase);
        //attackedParent.DOLocalMoveX(attackedParent.localPosition.x + (-attackedMovement * rightModificator) + attackedWidthOffset * currentWidthRatio, attackedMovementDuration).SetEase(attackedMovementEase);

        attackerParent.DORotate(attackerParent.rotation.eulerAngles + new Vector3(0, 0, attackerRotation * rightModificator), attackerRotationDuration).SetEase(attackerRotationEase);
        attackedParent.DORotate(attackedParent.rotation.eulerAngles + new Vector3(0, 0, attackedRotation * rightModificator), attackedRotationDuration).SetEase(attackedRotationEase);

        attackerParent.DOScale(Vector3.one * (attackerScale * attackerData.attackSpriteSize), attackerScaleDuration);
        attackedParent.DOScale(Vector3.one * (attackedScale * attackedData.attackSpriteSize), attackedScaleDuration * 3f).SetEase(Ease.OutElastic);

        Color colorAttacker = attackerImage.material.GetColor("_Color");
        DOTween.To(() => colorAttacker, x => colorAttacker = x, colorStandard, fadeColorStartDuration)
            .OnUpdate(() => {
                attackerImage.material.SetColor("_Color", colorAttacker);
            });

        
        Color wantedColor = colorHeal;

        if (currentCompetenceType == CompetenceType.summon)
            wantedColor = colorSummon;

        
        attackedImage.material.SetColor("_Color", colorStandard);
        Color colorAttacked = attackedImage.material.GetColor("_Color");
        DOTween.To(() => colorAttacked, x => colorAttacked = x, wantedColor, flickerColorDuration)
            .OnUpdate(() => {
                attackedImage.material.SetColor("_Color", colorAttacked);
            });

        yield return new WaitForSeconds(flickerColorDuration);

        colorAttacked = attackedImage.material.GetColor("_Color");
        DOTween.To(() => colorAttacked, x => colorAttacked = x, colorStandard, flickerColorDuration)
            .OnUpdate(() => {
                attackedImage.material.SetColor("_Color", colorAttacked);
            });
    }
    
    
    // MANAGE THE DAMAGE TEXT
    public IEnumerator TextFeel(bool leftOrigin, bool miss, bool crit, int damage, bool isSummon, bool isHeal)
    {
        Vector3 posLeftBottomCorner = new Vector3(-attackUIParent.rect.width * 0.5f, -attackUIParent.rect.height * 0.5f, 0);
        float healModificator = 1;

        if (isHeal || isSummon)
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


        if (isSummon)
        {
            damageNumber.text = "Summoned";
            damageNumber.color = colorSummonText;
        }
        else
        {
            if (!miss)
            {
                if (crit)
                {
                    damageNumber.text = "CRIT " + damage.ToString();

                    if (isHeal)
                        damageNumber.color = colorCritHealText;

                    else
                        damageNumber.color = colorCritAttack;
                }
                else
                {
                    damageNumber.text = damage.ToString();

                    if (isHeal)
                        damageNumber.color = colorNormalHealText;

                    else
                        damageNumber.color = colorNormalAttack;
                }
            }
            else
            {
                damageNumber.text = "Miss";
                damageNumber.color = colorMissAttack;
            }
        }

        yield return new WaitForSeconds(textFadeStart);

        damageNumber.DOFade(0, textFadeDuration);
    }


    // FADES OUT OF THE ATTACK UI
    public IEnumerator EndFeel()
    {
        float fadeLeft = leftChara.material.GetFloat("_Alpha");
        DOTween.To(() => fadeLeft, x => fadeLeft = x, 0.5f, fadeColorEndDuration)
            .OnUpdate(() => {
                leftChara.material.SetFloat("_Alpha", fadeLeft);
            });

        float fadeRight = rightChara.material.GetFloat("_Alpha");
        DOTween.To(() => fadeRight, x => fadeRight = x, 0.5f, fadeColorEndDuration)
            .OnUpdate(() => {
                rightChara.material.SetFloat("_Alpha", fadeRight);
            });
        
        
        Color colorLeft = leftChara.material.GetColor("_Color");
        DOTween.To(() => colorLeft, x => colorLeft = x, colorEnd, fadeColorEndDuration)
            .OnUpdate(() => {
                leftChara.material.SetColor("_Color", colorLeft);
            });
        
        Color colorRight = rightChara.material.GetColor("_Color");
        DOTween.To(() => colorRight, x => colorRight = x, colorEnd, fadeColorEndDuration)
            .OnUpdate(() => {
                rightChara.material.SetColor("_Color", colorRight);
            });
        
        
        leftCharaParent.DOLocalMoveX(leftCharaParent.localPosition.x + (-50 * currentWidthRatio), fadeColorEndDuration).SetEase(attackerMovementEase);
        rightCharaParent.DOLocalMoveX(rightCharaParent.localPosition.x + (50 * currentWidthRatio), fadeColorEndDuration).SetEase(attackerMovementEase);
        
        leftCharaParent.DOScale(leftCharaParent.localScale * 0.8f, fadeColorEndDuration).SetEase(attackerMovementEase);
        rightCharaParent.DOScale(rightCharaParent.localScale * 0.8f, fadeColorEndDuration).SetEase(attackerMovementEase);
        
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
    public IEnumerator GhostTrail(int iterations, float durationBetween, float ghostDuration, bool leftOrigin, CompetenceType currentCompetenceType)
    {
        while (iterations > 0)
        {
            iterations -= 1;

            GameObject currentPrefab = null;
            Image currentGhost = null;

            if (leftOrigin)
            {
                currentPrefab = Instantiate(ghostPrefab, rightChara.rectTransform.position, Quaternion.identity, ghostParentRight);

                currentGhost = currentPrefab.GetComponent<Image>();
                currentGhost.sprite = rightChara.sprite;
            }
            else
            {
                currentPrefab = Instantiate(ghostPrefab, leftChara.rectTransform.position, Quaternion.identity, ghostParentLeft);

                currentGhost = currentPrefab.GetComponent<Image>();
                currentGhost.sprite = leftChara.sprite;
            }

            Color wantedColor = colorDamage;

            if (currentCompetenceType == CompetenceType.miss)
                wantedColor = colorMissAttack;

            if (currentCompetenceType == CompetenceType.attackCrit)
                wantedColor = colorCritAttack;

            currentGhost.DOFade(1, ghostDuration).OnComplete(() => { currentGhost.DOFade(0, 0.2f); });
            currentGhost.material.SetColor("_Color", wantedColor);

            yield return new WaitForSeconds(durationBetween);
        }
    }
    
    
    // MANAGES THE VFX TO LAUNCH
    private void LaunchVFX(bool leftOrigin, CompetenceType currentCompetenceType)
    {
        Vector2 wantedPos = leftCharaParent.position;
        if (leftOrigin)
            wantedPos = rightCharaParent.position;

        if (currentCompetenceType == CompetenceType.attack)
        {
            UIVfxManager.Instance.DOSlash(wantedPos, leftOrigin);
        }
        
        else if (currentCompetenceType == CompetenceType.heal)
        {
            UIVfxManager.Instance.DoHeal(ghostParentLeft.GetComponent<RectTransform>(), ghostParentRight.GetComponent<RectTransform>(), leftOrigin);
        }
    }
}
