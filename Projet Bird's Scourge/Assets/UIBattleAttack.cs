using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIBattleAttack : MonoBehaviour
{
    [Header("AttackUI")] 
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
    public IEnumerator AttackUIFeel(Sprite leftSprite, Sprite rightSprite, bool leftAttack,int damage,bool miss,bool crit)
    {
        CameraManager.Instance.canMove = false;
        UIBattleManager.Instance.buttonScript.SwitchButtonInteractible(false);
        attackUI.gameObject.SetActive(true);

        leftChara.gameObject.SetActive(true);
        rightChara.gameObject.SetActive(true);

        leftChara.sprite = leftSprite;
        rightChara.sprite = rightSprite;

        attackFond.DOFade(0.8f, 0.07f);
        
        leftChara.DOFade(1, 0.07f);
        rightChara.DOFade(1, 0.07f);

        if (leftAttack) // SI UN ALLIÉ ATTAQUE ----------------------------------------------------------------------------------------
        {
            leftChara.rectTransform.DOMoveX(leftChara.rectTransform.position.x + 30, 0.5f);
            leftChara.rectTransform.DOMoveX(leftChara.rectTransform.position.x + 15, 0.5f);

            attackUI.DOShakePosition(1f, 10f);
            attackUI.DOScale(attackUI.localScale * 1.1f, 0.2f);
            attackUI.DORotate(new Vector3(0, 0, -5), 0.1f);

            damageNumber.rectTransform.anchoredPosition = new Vector3(143, 126, 0);
            damageNumber.rectTransform.rotation = new Quaternion(0, 0, 7,0);
            if (!miss)
            {
                if (crit)
                {
                    damageNumber.text = "CRIT " + damage.ToString();
                    damageNumber.color = new Color(255, 255, 0);
                }
                else
                {
                    damageNumber.text = damage.ToString();
                    damageNumber.color = new Color(255, 0, 0);
                }
            }
            else
            {
                damageNumber.text = "Miss";
                damageNumber.color = new Color(0, 0, 255);
            }
            

            damageNumber.DOFade(1, 0.07f);
            damageNumber.transform.DOScale(damageNumber.transform.localScale * 1.1f, 0.2f);
            damageNumber.transform.DORotate(new Vector3(0, 0, -5), 0.1f);
            damageNumber.transform.DOMove(damageNumber.transform.position + Vector3.up,0.2f);
        }
        else // SI UN ENNEMI ATTAQUE ------------------------------------------------------------------------------------------------------------
        {
            leftChara.rectTransform.DOMoveX(leftChara.rectTransform.position.x + 30, 0.5f);
            leftChara.rectTransform.DOMoveX(leftChara.rectTransform.position.x + 15, 0.5f);

            attackUI.DOShakePosition(1f, 10f);
            attackUI.DOScale(attackUI.localScale * 1.1f, 0.2f);
            attackUI.DORotate(new Vector3(0, 0, -5), 0.1f);

            
            damageNumber.rectTransform.anchoredPosition = new Vector3(-143, 126, 0);
            damageNumber.transform.rotation = new Quaternion(0, 0, -7,0);
            if (!miss)
            {
                if (crit)
                {
                    damageNumber.text = "CRIT " + damage.ToString();
                    damageNumber.color = new Color(255, 255, 0);
                }
                else
                {
                    damageNumber.text = damage.ToString();
                    damageNumber.color = new Color(255, 0, 0);
                }
            }
            else
            {
                damageNumber.text = "Miss";
                damageNumber.color = new Color(0, 0, 255);
            }
            
            damageNumber.DOFade(1, 0.07f);
            damageNumber.transform.DOScale(damageNumber.transform.localScale * 1.1f, 0.2f);
            damageNumber.transform.DORotate(new Vector3(0, 0, -5), 0.1f);
            damageNumber.transform.DOMove(damageNumber.transform.position + Vector3.up,0.2f);
        }

        yield return new WaitForSeconds(1.5f);
        
        attackFond.DOFade(0f, 0.1f);
        
        leftChara.DOFade(0, 0.1f);
        rightChara.DOFade(0, 0.1f);
        
        if (leftAttack)
        {
            leftChara.rectTransform.DOMoveX(leftChara.rectTransform.position.x - 20, 0.1f);
            leftChara.rectTransform.DOMoveX(leftChara.rectTransform.position.x - 15, 0.1f);
        }

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
    
    
    // WHEN THE ATTACK UI HAS TO APPEAR
    public IEnumerator SummonUIFeel(Sprite leftSprite, Sprite rightSprite)
    {
        CameraManager.Instance.canMove = false;
        UIBattleManager.Instance.buttonScript.SwitchButtonInteractible(false);
        attackUI.gameObject.SetActive(true);

        leftChara.gameObject.SetActive(true);
        rightChara.gameObject.SetActive(true);

        leftChara.sprite = leftSprite;
        rightChara.sprite = rightSprite;

        attackFond.DOFade(0.8f, 0.07f);
        
        leftChara.DOFade(1, 0.07f);
        rightChara.DOFade(1, 0.07f);

        
        leftChara.rectTransform.DOMoveX(leftChara.rectTransform.position.x + 30, 0.5f);
        leftChara.rectTransform.DOMoveX(leftChara.rectTransform.position.x + 15, 0.5f);

        attackUI.DOShakePosition(1f, 10f);
        attackUI.DOScale(attackUI.localScale * 1.1f, 0.2f);
        attackUI.DORotate(new Vector3(0, 0, -5), 0.1f);

            
        damageNumber.rectTransform.anchoredPosition = new Vector3(-143, 126, 0);
        damageNumber.transform.rotation = new Quaternion(0, 0, -7,0);
        
        damageNumber.text = "Summoned";
        damageNumber.color = new Color(0, 0, 255);
            
        damageNumber.DOFade(1, 0.07f);
        damageNumber.transform.DOScale(damageNumber.transform.localScale * 1.1f, 0.2f);
        damageNumber.transform.DORotate(new Vector3(0, 0, -5), 0.1f);
        damageNumber.transform.DOMove(damageNumber.transform.position + Vector3.up,0.2f);
        

        yield return new WaitForSeconds(1.5f);
        
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
