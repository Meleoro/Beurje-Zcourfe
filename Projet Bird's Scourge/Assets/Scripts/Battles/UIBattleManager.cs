using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleManager : MonoBehaviour
{
    [Header("Instance")] 
    private static UIBattleManager _instance;
    public static UIBattleManager Instance
    {
        get { return _instance; }
    }
    
    [Header("Buttons")]
    public TextMeshProUGUI attackButton;
    public TextMeshProUGUI competence1Button;
    public TextMeshProUGUI competence2Button;

    [Header("AttackUI")] 
    public RectTransform attackUI;
    public Image leftChara;
    public Image rightChara;
    public Image attackFond;


    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        
        else
            Destroy(gameObject);
    }


    private void Start()
    {
        AttackUISetup();
        
        attackUI.gameObject.SetActive(false);
    }


    //--------------------------INFOS UI PART------------------------------
    
    // CHANGE THE UI TO SHOW THE INFOS OF THE CURRENTLY SELECTED UNIT
    public void OpenUnitInfos(DataUnit unitInfos)
    {
        ActualiseButtons(unitInfos);
    }

    
    // ACTUALISE THE BUTTONS INFOS
    public void ActualiseButtons(DataUnit unitInfos)
    {
        if(unitInfos.attaqueData != null)
            attackButton.text = unitInfos.attaqueData.competenceName;
        
        if(unitInfos.competence1Data != null)
           competence1Button.text = unitInfos.competence1Data.competenceName;
        
        if(unitInfos.competence2Data != null)
           competence2Button.text = unitInfos.competence2Data.competenceName;
    }


    // INFORM OTHER SCRIPT THAT A BUTTON HAS BEEN PRESSED
    public void ClickButton(int index)
    {
        MouseManager.Instance.ChangeSelectedCompetence(index);
    }
    
    
    //--------------------------ATTACK PART------------------------------

    // SETUP EVERY ALPHAS WHEN THE SCENE START
    public void AttackUISetup()
    {
        attackFond.DOFade(0, 0);
        
        leftChara.DOFade(0, 0);
        rightChara.DOFade(0, 0);

        attackUI.DORotate(Vector3.zero, 0);
        attackUI.localScale = Vector3.one;
    }
    
    
    // WHEN THE ATTACK UI HAS TO APPEAR
    public IEnumerator AttackUIFeel(Sprite leftSprite, Sprite rightSprite, bool leftAttack)
    {
        attackUI.gameObject.SetActive(true);

        leftChara.gameObject.SetActive(true);
        rightChara.gameObject.SetActive(true);

        leftChara.sprite = leftSprite;
        rightChara.sprite = rightSprite;

        attackFond.DOFade(0.8f, 0.07f);
        
        leftChara.DOFade(1, 0.07f);
        rightChara.DOFade(1, 0.07f);

        if (leftAttack)
        {
            leftChara.rectTransform.DOMoveX(leftChara.rectTransform.position.x + 30, 0.5f);
            leftChara.rectTransform.DOMoveX(leftChara.rectTransform.position.x + 15, 0.5f);

            attackUI.DOShakePosition(1f, 10f);
            attackUI.DOScale(attackUI.localScale * 1.1f, 0.2f);
            attackUI.DORotate(new Vector3(0, 0, -5), 0.1f);
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
    }
    
}
