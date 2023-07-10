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

    [Header("Paramètres")] 
    public float dureeAnimTour;
    
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
    public TextMeshProUGUI compteurPointsMouvement;
    
    [Header("Mana")] 
    public TextMeshProUGUI conteurMana;
    public List<Image> manaIconList;
    public Sprite filledManaIcon;
    public Sprite emptyManaIcon;

    [Header("Menu Preview Attaques")] 
    public GameObject previewMenu;
    public TextMeshProUGUI textDMG;
    public TextMeshProUGUI textACC;
    public TextMeshProUGUI textCRT;
    public TextMeshProUGUI nomAllié;
    public TextMeshProUGUI nomEnnemi;
    public TextMeshProUGUI PvAllié;
    public Slider BarreDeVieAllié;
    public TextMeshProUGUI PvEnnemiPre;
    public TextMeshProUGUI PvEnnemiPost;
    public Slider BarreDeVieEnnemiPre;
    public Slider BarreDeVieEnnemiPost;
    public Image ArtAllié;
    public Image OmbreAllié;
    public Image ArtEnnemi;
    public Image OmbreEnnemi;

    [Header("Annonce Nouveau Tour")] 
    public Image blackScreen;
    public Animation animationTour;
    public Image fondCase;
    public Image ArtUnité;
    public Image OmbreUnité;
    public TextMeshProUGUI textNomPerso;
    public Sprite FondAllié;
    public Sprite FondEnnemi;
    
    
    [Header("AttackUI")] 
    public TextMeshProUGUI damageNumber;
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
        animationTour["allié"].layer = 0;
        animationTour["ennemi"].layer = 1;
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
        
        if (unitScript.currentHealth <= (unitInfos.levels[unitScript.CurrentLevel].PV) * 30 / 100)
        {
            unitArt.sprite = unitInfos.damageSprite;
        }
        else
        {
            unitArt.sprite = unitInfos.idleSprite;
        }
      
        unitShadow.sprite = unitArt.sprite;
        unitLevel.text = "LVL " + unitScript.CurrentLevel + 1;
        unitHP.text = unitScript.currentHealth + " / " + unitInfos.levels[unitScript.CurrentLevel].PV + " HP"; 
        LifeBar.maxValue = unitInfos.levels[unitScript.CurrentLevel].PV;
        LifeBar.value = unitScript.currentHealth;
    }

    // ACTUALISE THE BUTTONS INFOS
    public void ActualiseButtons(DataUnit unitInfos, Unit unitScript)
    {
        if (unitInfos.attaqueData is not null)
        {
            attackName.text = unitInfos.attaqueData.competenceName;
            attackDescription.text = unitInfos.attaqueData.levels[unitScript.AttackLevel].competenceDescription;
            attackManaCost.text = unitInfos.attaqueData.levels[unitScript.AttackLevel].competenceManaCost.ToString(); 
            attackDamageMultiplier.text = "STR x" + unitInfos.attaqueData.levels[unitScript.AttackLevel].damageMultiplier; 
        }

        if (unitInfos.competence1Data is not null)
        {
            competence1Name.text = unitInfos.competence1Data.competenceName;
            competence1Description.text = unitInfos.competence1Data.levels[unitScript.Competence1Level].competenceDescription;
            competence1ManaCost.text = unitInfos.competence1Data.levels[unitScript.Competence1Level].competenceManaCost.ToString();
            competence1DamageMultiplier.text = "STR x" + unitInfos.competence1Data.levels[unitScript.Competence1Level].damageMultiplier; 
        }
        
        if (unitInfos.competence2Data is not null)
        {
            competence2Name.text = unitInfos.competence2Data.competenceName;
            competence2Description.text = unitInfos.competence2Data.levels[unitScript.Competence2Level].competenceDescription;
            competence2ManaCost.text = unitInfos.competence2Data.levels[unitScript.Competence2Level].competenceManaCost.ToString(); 
            competence2DamageMultiplier.text = "STR x" + unitInfos.competence2Data.levels[unitScript.Competence2Level].damageMultiplier;
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
                            .levels[(BattleManager.Instance.order[i].GetComponent<Unit>().CurrentLevel)].PV) * 30 / 100)
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
                                                   Instance.order[i].GetComponent<Unit>().CurrentLevel].PV;
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
                    Instance.order[i].GetComponent<Unit>().CurrentLevel].PV;
                listeLifeBarCases[i].value = BattleManager.Instance.order[i].GetComponent<Unit>().currentHealth;
            }
            else
            {
                listeLifeBarCases[i].maxValue = BattleManager.Instance.order[i].GetComponent<Ennemy>().data.maxHealth;
                listeLifeBarCases[i].value = BattleManager.Instance.order[i].GetComponent<Ennemy>().currentHealth; 
            }
        }
    }

    // MET LES CASE DE L'UNITÉ SÉLÉCTIONNÉE EN SURBRILLANCE
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

    public IEnumerator NewTurnAnimation()
    {
        bool isAllié = false;
        if ((BattleManager.Instance.order[0].CompareTag("Unit"))) isAllié = true;
        else isAllié = false;
        animationTour.gameObject.SetActive(true);
        
        if (isAllié)
        {
            fondCase.sprite = FondAllié;
            ArtUnité.sprite = BattleManager.Instance.order[0].GetComponent<Unit>().data.attackSprite;
            OmbreUnité.sprite = ArtUnité.sprite;
            textNomPerso.text = BattleManager.Instance.order[0].GetComponent<Unit>().data.charaName + "'s turn";
            
            blackScreen.DOFade(0.7f, 0.2f);
            animationTour.Play("allié");
            yield return new WaitForSeconds(animationTour["allié"].length);
            blackScreen.DOFade(0, 0.2f);
            animationTour.gameObject.SetActive(false);
        }
        else
        {
            fondCase.sprite = FondEnnemi;
            ArtUnité.sprite = BattleManager.Instance.order[0].GetComponent<Ennemy>().data.attackSprite;
            OmbreUnité.sprite = ArtUnité.sprite;
            textNomPerso.text = BattleManager.Instance.order[0].GetComponent<Ennemy>().data.charaName + "'s turn";;
            
            blackScreen.DOFade(0.7f, 0.2f);
            animationTour.Play("ennemi");
            yield return new WaitForSeconds(animationTour["ennemi"].length);
            blackScreen.DOFade(0, 0.2f);
            animationTour.gameObject.SetActive(false);
        }
    }
    
    // ACTUALISE LE COMPTEUR DE POINTS DE MOUVEMENT
    public void UpdateMovePointsUI(Unit currentUnit)
    {
        compteurPointsMouvement.text = currentUnit.PM.ToString();
    }

    // INFORM OTHER SCRIPT THAT A BUTTON HAS BEEN PRESSED
    public void ClickButton(int index)
    {
        MouseManager.Instance.ChangeSelectedCompetence(index);
    }

    // FAIT APPARAITRE OU DISPARAITRE LES BOUTONS D'ANULATION DES SKILLS
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


    public void OpenAttackPreview(int damage,int hitRate, int critRate, Unit Allié, Ennemy Ennemi)
    {
        previewMenu.SetActive(true);
        previewMenu.transform.position = Ennemi.transform.position + Vector3.up;
        textDMG.text = damage.ToString();
        textACC.text = hitRate.ToString();
        textCRT.text = critRate.ToString();
        
        nomAllié.text = Allié.data.name;
        nomEnnemi.text = Ennemi.data.charaName;
        
        PvAllié.text = Allié.currentHealth + " / " + Allié.data.levels[Allié.CurrentLevel].PV;
        BarreDeVieAllié.maxValue = Allié.data.levels[Allié.CurrentLevel].PV;
        BarreDeVieAllié.value = Allié.currentHealth;
        
        PvEnnemiPre.text = Ennemi.currentHealth + " / " + Ennemi.data.maxHealth + " >";
        BarreDeVieEnnemiPre.maxValue = Ennemi.data.maxHealth;
        BarreDeVieEnnemiPre.value = Ennemi.currentHealth;
        
        if(Ennemi.currentHealth - damage > 0) PvEnnemiPost.text = Ennemi.currentHealth - damage + " / " + Ennemi.data.maxHealth;
        else PvEnnemiPost.text = 0 + " / " + Ennemi.data.maxHealth;
        BarreDeVieEnnemiPost.maxValue = Ennemi.data.maxHealth;
        BarreDeVieEnnemiPost.value = Ennemi.currentHealth - damage;
        
        ArtAllié.sprite = Allié.data.attackSprite;
        OmbreAllié.sprite = ArtAllié.sprite;
        ArtEnnemi.sprite = Ennemi.data.damageSprite;
        OmbreEnnemi.sprite = ArtEnnemi.sprite;
    }

    public void CloseAttackPreview()
    {
        previewMenu.SetActive(false);
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
    }
    
}
