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
    
    [Header("Unit Info")]
    public Image unitArt;
    public Image unitShadow;
    public TextMeshProUGUI unitName;
    public TextMeshProUGUI unitLevel;
    public TextMeshProUGUI unitHP;
    public Slider LifeBar;
    
    [Header("Buttons")]
        public TextMeshProUGUI attackName;
    public TextMeshProUGUI competence1Name;
    public TextMeshProUGUI competence2Name;
        public TextMeshProUGUI attackDescription;
    public TextMeshProUGUI competence1Description;
    public TextMeshProUGUI competence2Description;
        public TextMeshProUGUI attackManaCost;
    public TextMeshProUGUI competence1ManaCost;
    public TextMeshProUGUI competence2ManaCost;
        public TextMeshProUGUI attackDamageMultiplier;
    public TextMeshProUGUI competence1DamageMultiplier;
    public TextMeshProUGUI competence2DamageMultiplier;

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
        ActualiseUnitInfo(unitInfos);
    }

    // ACTUALISE THE UNIT INFOS
    public void ActualiseUnitInfo(DataUnit unitInfos)
    {
        unitName.text = unitInfos.charaName;
        unitShadow.sprite = unitArt.sprite;
        unitArt.sprite = unitInfos.idleSprite;
        unitLevel.text = "LVL " + unitInfos.levels[0].level;  // how to track the current level ?
        unitHP.text = "currentHP" + " / " + unitInfos.levels[0].PV + " HP"; // how to track the current level ?
        LifeBar.maxValue = unitInfos.levels[0].PV; // how to track the current level ?
        //LifeBar.value = "currentHP"      // how to track the current HP amount ?
    }
    
    // ACTUALISE THE BUTTONS INFOS
    public void ActualiseButtons(DataUnit unitInfos)
    {
        if (unitInfos.attaqueData is not null)
        {
            attackName.text = unitInfos.attaqueData.competenceName;
            attackDescription.text = unitInfos.attaqueData.levels[0].competenceDescription;
            attackManaCost.text = unitInfos.attaqueData.levels[unitInfos.levelUnlockCompetence1].competenceManaCost.ToString(); // how to track the current level ?
            attackDamageMultiplier.text = "STR x" + unitInfos.attaqueData.levels[unitInfos.levelUnlockCompetence2].damageMultiplier; // how to track the current level ?
        }

        if (unitInfos.competence1Data is not null)
        {
            competence1Name.text = unitInfos.competence1Data.competenceName;
            competence1Description.text = unitInfos.competence1Data.levels[0].competenceDescription;
            competence1ManaCost.text = unitInfos.competence1Data.levels[unitInfos.levelUnlockCompetence1].competenceManaCost.ToString(); // how to track the current level ?
            competence1DamageMultiplier.text = "STR x" + unitInfos.competence1Data.levels[unitInfos.levelUnlockCompetence2].damageMultiplier; // how to track the current level ?
        }
        
        if (unitInfos.competence2Data is not null)
        {
            competence2Name.text = unitInfos.competence2Data.competenceName;
            competence2Description.text = unitInfos.competence2Data.levels[0].competenceDescription;
            competence2ManaCost.text = unitInfos.competence2Data.levels[unitInfos.levelUnlockCompetence1].competenceManaCost.ToString(); // how to track the current level ?
            competence2DamageMultiplier.text = "STR x" + unitInfos.competence2Data.levels[unitInfos.levelUnlockCompetence2].damageMultiplier; // how to track the current level ?
        } 
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
