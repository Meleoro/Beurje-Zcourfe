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
    [Range(0f, 5f)] public float attackedShakeDuration;
    [Range(0f, 15f)] public float attackedShakeAmplitude;

    [Header("Movement")]
    [Range(-150f, 150f)] public float attackerMovement = 0;
    [Range(0f, 2f)] public float attackerMovementDuration = 0;
    [Range(-150f, 150f)] public float attackedMovement = 0;
    [Range(0f, 2f)] public float attackedMovementDuration = 0;
    private float originalXLeft;
    private float originalXRight;

    [Header("Rotation")]
    [Range(-20f, 20f)] public float attackerRotation = 0;
    [Range(0f, 2f)] public float attackerRotationDuration = 0;
    [Range(-20f, 20f)] public float attackedRotation = 0;
    [Range(0f, 2f)] public float attackedRotationDuration = 0;

    [Header("Scale")]
    [Range(0f, 2f)] public float attackerScale = 1;
    [Range(0f, 2f)] public float attackerScaleDuration = 0;
    [Range(0f, 2f)] public float attackedScale = 1;
    [Range(0f, 2f)] public float attackedScaleDuration = 0;

    [Header("Colors")]
    public Color colorStart;
    public Color colorStandard;
    public Color colorEnd;
    public Color colorDamage;
    public Color colorHeal;
    public Color colorSummon;
    [Range(0f, 2f)] public float fadeColorStartDuration = 0.2f;
    [Range(0f, 2f)] public float fadeColorEndDuration = 0.1f;
    [Range(0f, 2f)] public float flickerColorDuration = 0.3f;


    [Header("ParametersText")] 
    public Color colorNormalAttack;
    public Color colorMissAttack;
    public Color colorCritAttack;
    public Color colorNormalHeal;
    public Color colorCritHeal;
    public Color colorSummonText;
    
    [Header("References")] 
    public TextMeshProUGUI damageNumber;
    public RectTransform attackUI;
    public RectTransform leftCharaParent;
    public RectTransform rightCharaParent;
    public Image leftChara;
    public Image rightChara;
    public Image attackFond;



    public void Start()
    {
        attackUI.gameObject.SetActive(false);
    }


    // SETUP EVERY ALPHAS WHEN THE SCENE START
    public void AttackUISetup()
    {
        originalXLeft = leftCharaParent.transform.position.x;
        originalXRight = rightCharaParent.transform.position.x;

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
        damageNumber.rectTransform.localScale = Vector3.one;
    }
    
    
    // WHEN THE ATTACK UI HAS TO APPEAR
    public IEnumerator AttackUIFeel(Sprite leftSprite, Sprite rightSprite, bool leftOrigin, int damage, bool miss, bool crit)
    {
        SetupFeel(leftSprite, rightSprite);
        
        CharacterFeel(leftOrigin);

        TextFeel(leftOrigin, miss, crit, damage, false, false);

        yield return new WaitForSeconds(1.3f);

        StartCoroutine(EndFeel());
    }
    
    
    // WHEN THE SUMMON UI HAS TO APPEAR
    public IEnumerator SummonUIFeel(Sprite leftSprite, Sprite rightSprite)
    {
        SetupFeel(leftSprite, rightSprite);

        CharacterFeel(false);
        
        TextFeel(false, false, false, 0, true, false);
        
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(EndFeel());
    }
    
    
    // WHEN THE BUFF / HEAL UI HAS TO APPEAR
    public IEnumerator HealUIFeel(Sprite leftSprite, Sprite rightSprite, bool leftOrigin, int healValue, bool miss, bool crit)
    {
        SetupFeel(leftSprite, rightSprite);
        
        CharacterFeel(leftOrigin);

        TextFeel(leftOrigin, miss, crit, healValue, false, true);

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(EndFeel());
    }
    
    
    
    // SETUP THE DIFFERENT ELEMENTS
    public void SetupFeel(Sprite leftSprite, Sprite rightSprite)
    {
        CameraManager.Instance.canMove = false;
        UIBattleManager.Instance.buttonScript.SwitchButtonInteractible(false);
        attackUI.gameObject.SetActive(true);

        leftChara.gameObject.SetActive(true);
        rightChara.gameObject.SetActive(true);

        leftChara.sprite = leftSprite;
        rightChara.sprite = rightSprite;

        attackFond.DOFade(0.8f, apparitionFadeDuration);
        
        /*leftChara.DOFade(1, apparitionFadeDuration);
        rightChara.DOFade(1, apparitionFadeDuration);*/

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
    public void CharacterFeel(bool leftOrigin)
    {
        if (leftOrigin)
        {
            leftChara.rectTransform.DOShakePosition(attackerShakeDuration, new Vector3(1, 1, 0) * attackerShakeAmplitude);
            rightChara.rectTransform.DOShakePosition(attackedShakeDuration, new Vector3(1, 1, 0) * attackedShakeAmplitude);

            leftCharaParent.DOMoveX(leftCharaParent.position.x + attackerMovement, attackerMovementDuration).SetEase(Ease.OutQuint);
            rightCharaParent.DOMoveX(rightCharaParent.position.x + attackedMovement, attackedMovementDuration);

            leftCharaParent.DORotate(leftCharaParent.rotation.eulerAngles + new Vector3(0, 0, attackerRotation), attackerRotationDuration);
            rightCharaParent.DORotate(rightCharaParent.rotation.eulerAngles + new Vector3(0, 0, attackedRotation), attackedRotationDuration);

            leftCharaParent.DOScale(Vector3.one * attackerScale, attackerScaleDuration);
            rightCharaParent.DOScale(Vector3.one * attackedScale, attackedScaleDuration);

            Color colorLeft = leftChara.material.GetColor("_Color");
            DOTween.To(() => colorLeft, x => colorLeft = x, colorStandard, fadeColorStartDuration)
                .OnUpdate(() => {
                    leftChara.material.SetColor("_Color", colorLeft);
                });

            Color colorRight = rightChara.material.GetColor("_Color");
            DOTween.To(() => colorRight, x => colorRight = x, colorStandard, fadeColorStartDuration)
                .OnUpdate(() => {
                    rightChara.material.SetColor("_Color", colorRight);
                });
        }

        else
        {
            /*rightChara.rectTransform.DOMoveX(rightChara.rectTransform.position.x - 30, 0.5f);
            leftChara.rectTransform.DOMoveX(leftChara.rectTransform.position.x + 15, 0.5f);*/
            
            rightChara.rectTransform.DOShakePosition(attackerShakeDuration, attackerShakeAmplitude);
            leftChara.rectTransform.DOShakePosition(attackedShakeDuration, attackedShakeAmplitude);


            /*attackUI.DOShakePosition(1f, 10f);
            attackUI.DOScale(attackUI.localScale * 1.1f, 0.2f);
            attackUI.DORotate(new Vector3(0, 0, -5), 0.1f); */
        }
    }
    
    
    // MANAGE THE DAMAGE TEXT
    public void TextFeel(bool leftOrigin, bool miss, bool crit, int damage, bool isSummon, bool isHeal)
    {
        if (isSummon)
        {
            damageNumber.rectTransform.anchoredPosition = new Vector3(-143, 126, 0);
            damageNumber.transform.rotation = new Quaternion(0, 0, -7,0);
        
            damageNumber.text = "Summoned";
            damageNumber.color = colorSummonText;
            
            damageNumber.DOFade(1, 0.07f);
            damageNumber.transform.DOScale(damageNumber.transform.localScale * 1.1f, 0.2f);
            damageNumber.transform.DORotate(new Vector3(0, 0, -5), 0.1f);
            damageNumber.transform.DOMove(damageNumber.transform.position + Vector3.up,0.2f);
        }
        
        else if (leftOrigin) 
        {
            damageNumber.rectTransform.anchoredPosition = new Vector3(143, 126, 0);
            damageNumber.rectTransform.rotation = new Quaternion(0, 0, 7,0);
            
            if (!miss)
            {
                if (crit)
                {
                    damageNumber.text = "CRIT " + damage.ToString();
                    
                    if(isHeal)
                        damageNumber.color = colorCritHeal;
                    
                    else
                        damageNumber.color = colorCritAttack;
                }
                else
                {
                    damageNumber.text = damage.ToString();
                    
                    if(isHeal)
                        damageNumber.color = colorNormalHeal;
                    
                    else
                        damageNumber.color = colorNormalAttack;
                }
            }
            else
            {
                damageNumber.text = "Miss";
                damageNumber.color = colorMissAttack;
            }
            

            damageNumber.DOFade(1, 0.07f);
            damageNumber.transform.DOScale(damageNumber.transform.localScale * 1.1f, 0.2f);
            damageNumber.transform.DORotate(new Vector3(0, 0, -5), 0.1f);
            damageNumber.transform.DOMove(damageNumber.transform.position + Vector3.up,0.2f);
        }
        
        else 
        {
            damageNumber.rectTransform.anchoredPosition = new Vector3(-143, 126, 0);
            damageNumber.transform.rotation = new Quaternion(0, 0, -7,0);
            
            if (!miss)
            {
                if (crit)
                {
                    damageNumber.text = "CRIT " + damage.ToString();
                    
                    if(isHeal)
                        damageNumber.color = colorCritHeal;
                    
                    else
                        damageNumber.color = colorCritAttack;
                }
                else
                {
                    damageNumber.text = damage.ToString();
                    
                    if(isHeal)
                        damageNumber.color = colorNormalHeal;
                    
                    else
                        damageNumber.color = colorNormalAttack;
                }
            }
            else
            {
                damageNumber.text = "Miss";
                damageNumber.color = colorMissAttack;
            }
            
            damageNumber.DOFade(1, 0.07f);
            damageNumber.transform.DOScale(damageNumber.transform.localScale * 1.1f, 0.2f);
            damageNumber.transform.DORotate(new Vector3(0, 0, -5), 0.1f);
            damageNumber.transform.DOMove(damageNumber.transform.position + Vector3.up,0.2f);
        }
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

        leftCharaParent.DOMoveX(originalXLeft, 0);
        rightCharaParent.DOMoveX(originalXRight, 0);

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
}
