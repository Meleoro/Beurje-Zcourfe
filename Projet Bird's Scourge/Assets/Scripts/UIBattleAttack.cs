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
    float leftWidthOffset;
    float leftHeightOffset;
    float rightWidthOffset;
    float rightHeightOffset;

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

        originalXLeft = leftCharaParent.transform.position.x;
        originalXRight = rightCharaParent.transform.position.x;

        originalYLeft = leftCharaParent.transform.position.y;
        originalYRight = rightCharaParent.transform.position.y;
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
    public IEnumerator AttackUIFeel(DataUnit leftData, DataUnit rightData, bool leftOrigin, int damage, bool miss, bool crit)
    {
        CompetenceType currentCompetenceType = CompetenceType.attack;

        if (miss) currentCompetenceType = CompetenceType.miss;

        else if (crit) currentCompetenceType = CompetenceType.attackCrit;


        if (leftOrigin)
            SetupFeel(leftData.attackSprite, rightData.damageSprite, leftData, rightData);
        
        else
            SetupFeel(leftData.damageSprite, rightData.attackSprite, leftData, rightData);
        
        StartCoroutine(CharacterFeel(leftOrigin, leftData, rightData, currentCompetenceType));

        StartCoroutine(TextFeel(leftOrigin, miss, crit, damage, false, false));

        StartCoroutine(GhostTrail(10, 0.04f, 0.1f, leftOrigin, currentCompetenceType));

        yield return new WaitForSeconds(1.3f);

        StartCoroutine(EndFeel());
    }
    
    
    // WHEN THE SUMMON UI HAS TO APPEAR
    public IEnumerator SummonUIFeel(DataUnit leftData, DataUnit rightData, bool leftOrigin)
    {
        CompetenceType currentCompetenceType = CompetenceType.summon;

        SetupFeel(leftData.attackSprite, rightData.attackSprite, leftData, rightData);

        StartCoroutine(CharacterFeel(false, leftData, rightData, currentCompetenceType));

        StartCoroutine(TextFeel(false, false, false, 0, true, false));
        
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(EndFeel());
    }
    
    
    // WHEN THE BUFF / HEAL UI HAS TO APPEAR
    public IEnumerator HealUIFeel(DataUnit leftData, DataUnit rightData, bool leftOrigin, int healValue, bool miss, bool crit)
    {
        CompetenceType currentCompetenceType = CompetenceType.heal;

        if (leftOrigin)
            SetupFeel(leftData.attackSprite, rightData.damageSprite, leftData, rightData);

        else
            SetupFeel(leftData.damageSprite, rightData.attackSprite, leftData, rightData);

        StartCoroutine(CharacterFeelHeal(leftOrigin, leftData, rightData, currentCompetenceType));

        StartCoroutine(TextFeel(leftOrigin, miss, crit, healValue, false, true));

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(EndFeel());
    }
    
    
    
    // SETUP THE DIFFERENT ELEMENTS
    public void SetupFeel(Sprite leftSprite, Sprite rightSprite, DataUnit leftData, DataUnit rightData)
    {
        currentWidthRatio = CameraManager.Instance.screenWidth / 800;
        currentHeightRatio = CameraManager.Instance.screenHeight / 300;

        leftWidthOffset = 800 * leftData.XPosModificator;
        leftHeightOffset = 300 * leftData.YPosModificator;

        rightWidthOffset = 800 * rightData.XPosModificator;
        rightHeightOffset = 300 * rightData.YPosModificator;


        CameraManager.Instance.canMove = false;
        UIBattleManager.Instance.buttonScript.SwitchButtonInteractible(false);
        attackUI.gameObject.SetActive(true);

        leftChara.gameObject.SetActive(true);
        rightChara.gameObject.SetActive(true);

        leftChara.sprite = leftSprite;
        rightChara.sprite = rightSprite;

        leftCharaParent.localScale = Vector3.one * leftData.attackSpriteSize;
        rightCharaParent.localScale = Vector3.one * rightData.attackSpriteSize;

        leftCharaParent.position = new Vector3(originalXLeft + leftWidthOffset, originalYLeft + leftHeightOffset, leftCharaParent.position.z);
        rightCharaParent.position = new Vector3(originalXRight + rightWidthOffset, originalYRight + rightHeightOffset, rightCharaParent.position.z);

        attackFond.DOFade(0.8f, apparitionFadeDuration);

        float fadeLeft = leftChara.material.GetFloat("_Alpha");
        DOTween.To(() => fadeLeft, x => fadeLeft = x, 1, fadeColorStartDuration)
            .OnUpdate(() => {
                leftChara.material.SetFloat("_Alpha", fadeLeft);
            });

        float fadeRight = rightChara.material.GetFloat("_Alpha");
        DOTween.To(() => fadeRight, x => fadeRight = x, 1, fadeColorStartDuration)
            .OnUpdate(() => {
                rightChara.material.SetFloat("_Alpha", fadeRight);
            });
    }


    // GENERATE THE MOVEMENT OF THE CHARACTERS
    public IEnumerator CharacterFeel(bool leftOrigin, DataUnit leftData, DataUnit rightData, CompetenceType currentCompetenceType)
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
                attackerImage.rectTransform.DOShakePosition(attackerShakeDuration, new Vector3(1.2f, 1, 0) * attackedShakeAmplitude * currentWidthRatio, attackerShakeVibrato);
                attackedImage.rectTransform.DOShakePosition(attackedShakeDuration, new Vector3(1.2f, 1, 0) * attackedShakeAmplitude * currentWidthRatio, attackedShakeVibrato);
            }
            else
            {
                attackerImage.rectTransform.DOShakePosition(attackerShakeDuration, new Vector3(1, 1, 0) * attackedShakeAmplitude * currentWidthRatio, attackerShakeVibrato);
                attackedImage.rectTransform.DOShakePosition(attackedShakeDuration, new Vector3(1, 1, 0) * attackedShakeAmplitude * currentWidthRatio, attackedShakeVibrato);
            }
        }

        attackerParent.DOMoveX(attackerParent.position.x + (attackerMovement * rightModificator) + leftWidthOffset * currentWidthRatio, attackerMovementDuration).SetEase(attackerMovementEase);
        attackedParent.DOMoveX(attackedParent.position.x + (attackedMovement * rightModificator) + rightWidthOffset * currentWidthRatio, attackedMovementDuration).SetEase(attackedMovementEase);

        attackerParent.DORotate(attackerParent.rotation.eulerAngles + new Vector3(0, 0, attackerRotation * rightModificator), attackerRotationDuration).SetEase(attackerRotationEase);
        attackedParent.DORotate(attackedParent.rotation.eulerAngles + new Vector3(0, 0, attackedRotation * rightModificator), attackedRotationDuration).SetEase(attackedRotationEase);

        attackerParent.DOScale(Vector3.one * (attackerScale * attackerData.attackSpriteSize), attackerScaleDuration);
        attackedParent.DOScale(Vector3.one * (attackedScale * attackedData.attackSpriteSize), attackedScaleDuration);

        Color colorAttacker = attackerImage.material.GetColor("_Color");
        DOTween.To(() => colorAttacker, x => colorAttacker = x, colorStandard, fadeColorStartDuration)
            .OnUpdate(() => {
                attackerImage.material.SetColor("_Color", colorAttacker);
            });


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

        attackerParent.DOMoveX(attackerParent.position.x + (attackerMovement * rightModificator) + leftWidthOffset * currentWidthRatio, attackerMovementDuration).SetEase(attackerMovementEase);
        attackedParent.DOMoveX(attackedParent.position.x + (attackedMovement * rightModificator) + rightWidthOffset * currentWidthRatio, attackedMovementDuration).SetEase(attackedMovementEase);

        attackerParent.DORotate(attackerParent.rotation.eulerAngles + new Vector3(0, 0, attackerRotation * rightModificator), attackerRotationDuration).SetEase(attackerRotationEase);
        attackedParent.DORotate(attackedParent.rotation.eulerAngles + new Vector3(0, 0, attackedRotation * rightModificator), attackedRotationDuration).SetEase(attackedRotationEase);

        attackerParent.DOScale(Vector3.one * (attackerScale * attackerData.attackSpriteSize), attackerScaleDuration);
        attackedParent.DOScale(Vector3.one * (attackedScale * attackedData.attackSpriteSize), attackedScaleDuration);

        Color colorAttacker = attackerImage.material.GetColor("_Color");
        DOTween.To(() => colorAttacker, x => colorAttacker = x, colorStandard, fadeColorStartDuration)
            .OnUpdate(() => {
                attackerImage.material.SetColor("_Color", colorAttacker);
            });


        Color wantedColor = colorHeal;

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
        if (leftOrigin)
        {
            damageNumber.rectTransform.position = new Vector3(textXOrigin * currentWidthRatio, textYOrigin * currentHeightRatio, 0);
            damageNumber.rectTransform.rotation = Quaternion.Euler(0, 0, -textOriginalRot);
            damageNumber.rectTransform.localScale = Vector3.one * textOriginalSize;

            damageNumber.rectTransform.DOMoveX(textXEnd * currentWidthRatio, textMoveDuration).SetEase(textMoveEase);
            damageNumber.rectTransform.DOMoveY(textYEnd * currentHeightRatio, textMoveDuration).SetEase(textMoveEase);

            damageNumber.rectTransform.DOScale(Vector3.one * textEndSize, textMoveDuration);

            damageNumber.rectTransform.DORotate(new Vector3(0, 0, -textEndRot), textMoveDuration).SetEase(textRotateEase);
        }
        else
        {
            damageNumber.rectTransform.position = new Vector3(800 - textXOrigin * currentWidthRatio, textYOrigin * currentHeightRatio, 0);
            damageNumber.rectTransform.rotation = Quaternion.Euler(0, 0, textOriginalRot);
            damageNumber.rectTransform.localScale = Vector3.one * textOriginalSize;

            damageNumber.rectTransform.DOMoveX(800 - textXEnd * currentWidthRatio, textMoveDuration).SetEase(textMoveEase);
            damageNumber.rectTransform.DOMoveY(textYEnd * currentHeightRatio, textMoveDuration).SetEase(textMoveEase);

            damageNumber.rectTransform.DOScale(Vector3.one * textEndSize, textMoveDuration);

            damageNumber.rectTransform.DORotate(new Vector3(0, 0, textEndRot), textMoveDuration).SetEase(textRotateEase);
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
        attackFond.DOFade(0f, 0.1f);

        float fadeLeft = leftChara.material.GetFloat("_Alpha");
        DOTween.To(() => fadeLeft, x => fadeLeft = x, 0, fadeColorEndDuration)
            .OnUpdate(() => {
                leftChara.material.SetFloat("_Alpha", fadeLeft);
            });

        float fadeRight = rightChara.material.GetFloat("_Alpha");
        DOTween.To(() => fadeRight, x => fadeRight = x, 0, fadeColorEndDuration)
            .OnUpdate(() => {
                rightChara.material.SetFloat("_Alpha", fadeRight);
            });


        CameraManager.Instance.ExitCameraBattle();
        
        yield return new WaitForSeconds(0.1f);

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
}
