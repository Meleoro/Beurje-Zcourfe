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

    [Header("ParametersText")] 
    public Color colorNormalAttack;
    public Color colorMissAttack;
    public Color colorCritAttack;
    public Color colorNormalHeal;
    public Color colorCritHeal;
    public Color colorSummon;
    
    [Header("References")] 
    public TextMeshProUGUI damageNumber;
    public RectTransform attackUI;
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
        attackFond.DOFade(0, 0);
        
        leftChara.DOFade(0, 0);
        rightChara.DOFade(0, 0);

        attackUI.DORotate(Vector3.zero, 0);
        attackUI.localScale = Vector3.one;
        damageNumber.rectTransform.localScale = Vector3.one;
    }
    
    
    // WHEN THE ATTACK UI HAS TO APPEAR
    public IEnumerator AttackUIFeel(Sprite leftSprite, Sprite rightSprite, bool leftOrigin, int damage, bool miss, bool crit)
    {
        SetupFeel(leftSprite, rightSprite);
        
        CharacterFeel(leftOrigin);

        TextFeel(leftOrigin, miss, crit, damage, false, true);

        yield return new WaitForSeconds(1.5f);

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
        
        leftChara.DOFade(1, apparitionFadeDuration);
        rightChara.DOFade(1, apparitionFadeDuration);
    }


    // GENERATE THE MOVEMENT OF THE CHARACTERS
    public void CharacterFeel(bool leftOrigin)
    {
        if (leftOrigin)
        {
            /*leftChara.rectTransform.DOMoveX(leftChara.rectTransform.position.x + 30, 0.5f);
            rightChara.rectTransform.DOMoveX(rightChara.rectTransform.position.x - 15, 0.5f);*/

            leftChara.rectTransform.DOShakePosition(attackerShakeDuration, attackerShakeAmplitude);
            rightChara.rectTransform.DOShakePosition(attackedShakeDuration, attackedShakeAmplitude);

            if (unifiedShake)
            {
                attackUI.DOShakePosition(1f, 10f);
            }

            attackUI.DOScale(attackUI.localScale * 1.1f, 0.2f);
            attackUI.DORotate(new Vector3(0, 0, -5), 0.1f);
        }

        else
        {
            /*rightChara.rectTransform.DOMoveX(rightChara.rectTransform.position.x - 30, 0.5f);
            leftChara.rectTransform.DOMoveX(leftChara.rectTransform.position.x + 15, 0.5f);*/
            
            rightChara.rectTransform.DOShakePosition(attackerShakeDuration, attackerShakeAmplitude);
            leftChara.rectTransform.DOShakePosition(attackedShakeDuration, attackedShakeAmplitude);

            if (unifiedShake)
            {
                attackUI.DOShakePosition(1f, 10f);
            }
            
            attackUI.DOScale(attackUI.localScale * 1.1f, 0.2f);
            attackUI.DORotate(new Vector3(0, 0, -5), 0.1f);
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
            damageNumber.color = colorSummon;
            
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
        
        leftChara.DOFade(0, 0.1f);
        rightChara.DOFade(0, 0.1f);
        

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
}
