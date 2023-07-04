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
    public GameObject attackCancelButton;
    public GameObject competence1CancelButton;
    public GameObject competence2CancelButton;

    [Header("Cases Tour")] 
    public List<Image> listeFondCases;
    public List<Image> listeFondIlluminé;
    public Sprite fondAllié;
    public Sprite fondEnnemi;
    public List<Image> listeArtCases;
    public List<Image> listeShadowCases;
    public List<TextMeshProUGUI> listeTextPvCases;
    public List<Slider> listeLifeBarCases;
    
    [Header("Mana")] 
    public TextMeshProUGUI conteurMana;
    public List<Image> manaIconList;
    public Sprite filledManaIcon;
    public Sprite emptyManaIcon;

    [Header("Menu Preview Attaques")] 
    public TextMeshProUGUI textDMG;
    public TextMeshProUGUI textACC;
    public TextMeshProUGUI textCRT;
    public TextMeshProUGUI nomAllié;
    public TextMeshProUGUI nomEnnemi;
    public TextMeshProUGUI PvAllié;
    public TextMeshProUGUI PvEnnemi;
    public Slider BarreDeVieAllié;
    public Slider BarreDeVieEnnemi;
    public Image ArtAllié;
    public Image OmbreAllié;
    public Image ArtEnnemi;
    public Image OmbreEnnemi;
    
    
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
        UpdateManaUI();
        attackUI.gameObject.SetActive(false);
    }


    //--------------------------INFOS UI PART------------------------------
    
    // CHANGE THE UI TO SHOW THE INFOS OF THE CURRENTLY SELECTED UNIT
    public void OpenUnitInfos(DataUnit unitInfos, Unit unitScript)
    {
        ActualiseButtons(unitInfos,unitScript);
        ActualiseUnitInfo(unitInfos, unitScript);
    }

    // ACTUALISE THE UNIT INFOS
    public void ActualiseUnitInfo(DataUnit unitInfos, Unit unitScript)
    {
        unitName.text = unitInfos.charaName;
        
        if (unitScript.currentHealth <= (unitInfos.levels[unitScript.currentLevel-1].PV) * 30 / 100)
        {
            unitArt.sprite = unitInfos.damageSprite;
        }
        else
        {
            unitArt.sprite = unitInfos.idleSprite;
        }
      
        unitShadow.sprite = unitArt.sprite;
        unitLevel.text = "LVL " + unitScript.currentLevel;
        unitHP.text = unitScript.currentHealth + " / " + unitInfos.levels[unitScript.currentLevel-1].PV + " HP"; 
        LifeBar.maxValue = unitInfos.levels[unitScript.currentLevel-1].PV;
        LifeBar.value = unitScript.currentHealth;
    }
    
    // ACTUALISE THE BUTTONS INFOS
    public void ActualiseButtons(DataUnit unitInfos, Unit unitScript)
    {
        if (unitInfos.attaqueData is not null)
        {
            attackName.text = unitInfos.attaqueData.competenceName;
            attackDescription.text = unitInfos.attaqueData.levels[unitScript.attackLevel-1].competenceDescription;
            attackManaCost.text = unitInfos.attaqueData.levels[unitScript.attackLevel-1].competenceManaCost.ToString(); 
            attackDamageMultiplier.text = "STR x" + unitInfos.attaqueData.levels[unitScript.attackLevel-1].damageMultiplier; 
        }

        if (unitInfos.competence1Data is not null)
        {
            competence1Name.text = unitInfos.competence1Data.competenceName;
            competence1Description.text = unitInfos.competence1Data.levels[unitScript.competence1Level-1].competenceDescription;
            competence1ManaCost.text = unitInfos.competence1Data.levels[unitScript.competence1Level-1].competenceManaCost.ToString();
            competence1DamageMultiplier.text = "STR x" + unitInfos.competence1Data.levels[unitScript.competence1Level-1].damageMultiplier; 
        }
        
        if (unitInfos.competence2Data is not null)
        {
            competence2Name.text = unitInfos.competence2Data.competenceName;
            competence2Description.text = unitInfos.competence2Data.levels[unitScript.competence2Level-1].competenceDescription;
            competence2ManaCost.text = unitInfos.competence2Data.levels[unitScript.competence2Level-1].competenceManaCost.ToString(); 
            competence2DamageMultiplier.text = "STR x" + unitInfos.competence2Data.levels[unitScript.competence2Level-1].damageMultiplier;
        } 
    }

    // ACTUALISE L'UI DU MANA
    public void UpdateManaUI()
    {
        conteurMana.text = BattleManager.Instance.currentMana.ToString();

        for (int i = 0; i < manaIconList.Count; i++)
        {
            manaIconList[i].sprite = emptyManaIcon;
        }
        
        for (int i = 0; i < BattleManager.Instance.currentMana; i++)
        {
            manaIconList[i].sprite = filledManaIcon;
        }
    }

    // ACTUALISE L'UI DE L'ORDRE DES TOURS
    public void UpdateTurnUI()
    {
        for (int i = 0; i < listeFondCases.Count; i++)
        {
            if (BattleManager.Instance.order[i].CompareTag("Ennemy")) 
            {
                listeFondCases[i].sprite = fondEnnemi;  
            }
            else if (BattleManager.Instance.order[i].CompareTag("Unit"))
            {
                listeFondCases[i].sprite = fondAllié;  
            }
        }

        for (int i = 0; i < listeArtCases.Count; i++)
        {
            if (i == 0)
            {
                if (BattleManager.Instance.order[i].CompareTag("Unit"))
                {
                    listeArtCases[i].sprite = BattleManager.Instance.order[i].GetComponent<Unit>().data.attackSprite;
                }
                else
                {
                    listeArtCases[i].sprite = BattleManager.Instance.order[i].GetComponent<Ennemy>().data.attackSprite;
                }
                listeShadowCases[i].sprite = listeArtCases[i].sprite;  
            }

            if (BattleManager.Instance.order[i].CompareTag("Unit"))
            {
                if (i != 0)
                {
                    if (BattleManager.Instance.order[i].GetComponent<Unit>().currentHealth <= (BattleManager.Instance.order[i].GetComponent<Unit>().data
                            .levels[(BattleManager.Instance.order[i].GetComponent<Unit>().currentLevel - 1)].PV) * 30 / 100)
                    {
                        listeArtCases[i].sprite = BattleManager.Instance.order[i].GetComponent<Unit>().data.damageSprite;
                    }
                    else
                    {
                        listeArtCases[i].sprite = BattleManager.Instance.order[i].GetComponent<Unit>().data.idleSprite;
                    }
                    listeShadowCases[i].sprite = listeArtCases[i].sprite;  
                }
            }
            else
            {
                if (BattleManager.Instance.order[i].GetComponent<Ennemy>().currentHealth <= (BattleManager.Instance.order[i].GetComponent<Ennemy>().data
                      .maxHealth) * 30 / 100)
                {
                    listeArtCases[i].sprite = BattleManager.Instance.order[i].GetComponent<Ennemy>().data.damageSprite;
                }
                else
                {
                    listeArtCases[i].sprite = BattleManager.Instance.order[i].GetComponent<Ennemy>().data.idleSprite;
                }
                listeShadowCases[i].sprite = listeArtCases[i].sprite;  
            }
          
        }

        for (int i = 0; i < listeTextPvCases.Count; i++)
        {
                if (BattleManager.Instance.order[i].CompareTag("Unit"))
                {
                    listeTextPvCases[i].text = BattleManager.Instance.order[i].GetComponent<Unit>().currentHealth + "/" + 
                                               BattleManager.Instance.order[i].GetComponent<Unit>().data.levels[BattleManager.
                                                   Instance.order[i].GetComponent<Unit>().currentLevel-1].PV;
                }
                else
                {
                    listeTextPvCases[i].text = BattleManager.Instance.order[i].GetComponent<Ennemy>().currentHealth + "/" +
                                               BattleManager.Instance.order[i].GetComponent<Ennemy>().data.maxHealth;
                }
        }

        for (int i = 0; i < listeLifeBarCases.Count; i++)
        {
            if (BattleManager.Instance.order[i].CompareTag("Unit"))
            {
                listeLifeBarCases[i].maxValue = BattleManager.Instance.order[i].GetComponent<Unit>().data.levels[BattleManager.
                    Instance.order[i].GetComponent<Unit>().currentLevel-1].PV;
                listeLifeBarCases[i].value = BattleManager.Instance.order[i].GetComponent<Unit>().currentHealth;
            }
            else
            {
                listeLifeBarCases[i].maxValue = BattleManager.Instance.order[i].GetComponent<Ennemy>().data.maxHealth;
                listeLifeBarCases[i].value = BattleManager.Instance.order[i].GetComponent<Ennemy>().currentHealth; 
            }
           
        }
    }

    public void UpdateTurnUISelectedUnit(Unit unitInfos)
    {
        if (MouseManager.Instance.selectedUnit != null)
        {
            for (int i = 0; i < listeFondIlluminé.Count; i++)
            {
                if (unitInfos == BattleManager.Instance.order[i].GetComponent<Unit>())
                {
                    listeFondIlluminé[i].gameObject.SetActive(true);
                }
                else
                {
                    listeFondIlluminé[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < listeFondIlluminé.Count; i++)
            {
                listeFondIlluminé[i].gameObject.SetActive(false);
            }
        }
      
    }

    public void UpdateAttackPreview()
    {
        
    }

    // INFORM OTHER SCRIPT THAT A BUTTON HAS BEEN PRESSED
    public void ClickButton(int index)
    {
        MouseManager.Instance.ChangeSelectedCompetence(index);
    }


    public void ChangeButtonState(int index)
    {
        if (index == 0)
        {
            if (MouseManager.Instance.competenceUsed == MouseManager.Instance.selectedUnit.data.attaqueData)
            {
                attackCancelButton.SetActive(false);
            }
            else
            {
                attackCancelButton.SetActive(true);
                competence1CancelButton.SetActive(false);
                competence2CancelButton.SetActive(false);
            }
        }
        
        if (index == 1)
        {
            if (MouseManager.Instance.competenceUsed == MouseManager.Instance.selectedUnit.data.competence1Data)
            {
                competence1CancelButton.SetActive(false);
            }
            else 
            {
                competence1CancelButton.SetActive(true);
                attackCancelButton.SetActive(false);
                competence2CancelButton.SetActive(false);
            }
        }

        if (index == 2)
        {
            if (MouseManager.Instance.competenceUsed == MouseManager.Instance.selectedUnit.data.competence2Data)
            {
                competence2CancelButton.SetActive(false);
            }
            else 
            {
                competence2CancelButton.SetActive(true);
                attackCancelButton.SetActive(false);
                competence1CancelButton.SetActive(false);
            }
        }
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
