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

    [Header("Extensions")]
    public UIBattleButtons buttonScript;
    public UIBattleAttack attackScript;
    public UIBattleObject objectsScript;

    [Header("Paramètres")] 
    public float dureeAnimTour;
    public float dureeAnimAttaque;

    [Header("Unit Info")] 
    public Image fondBarreInfo;
    public Sprite fondBleu;
    public Sprite fondRouge;
    public Image unitArt;
    public Image unitShadow;
    public TextMeshProUGUI unitName;
    public TextMeshProUGUI unitLevel;
    public TextMeshProUGUI unitHP;
    public Slider LifeBar;

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


    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        
        else
            Destroy(gameObject);
    }


    private void Start()
    {
        attackScript.AttackUISetup();
        UpdateManaUI();
        animationTour["allié"].layer = 0;
        animationTour["ennemi"].layer = 1;
    }


    //--------------------------INFOS UI PART------------------------------
    
    // CHANGE THE UI TO SHOW THE INFOS OF THE CURRENTLY SELECTED UNIT
    public void OpenUnitInfos(DataUnit unitInfos, Unit unitScript,Ennemy ennemyScript)
    {
        buttonScript.ActualiseButtons(unitInfos,unitScript,ennemyScript);
        ActualiseUnitInfo(unitInfos, unitScript,ennemyScript);
    }

    // ACTUALISE THE UNIT INFOS
    public void ActualiseUnitInfo(DataUnit unitInfos, Unit unitScript, Ennemy ennemyScript)
    {
        bool isAllié = false;
        if ((unitInfos.isEnnemy)) isAllié = false;
        else isAllié = true;

        if (isAllié)
        {
            //fondBarreInfo.sprite = fondBleu;
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
        else
        {
            //fondBarreInfo.sprite = fondRouge;
            unitName.text = unitInfos.charaName;
        
            if (ennemyScript.currentHealth <= (unitInfos.levels[ennemyScript.CurrentLevel].PV) * 30 / 100)
            {
                unitArt.sprite = unitInfos.damageSprite;
            }
            else
            {
                unitArt.sprite = unitInfos.idleSprite;
            }
      
            unitShadow.sprite = unitArt.sprite;
            unitLevel.text = "LVL " + ennemyScript.CurrentLevel + 1;
            unitHP.text = ennemyScript.currentHealth + " / " + unitInfos.levels[ennemyScript.CurrentLevel].PV + " HP"; 
            LifeBar.maxValue = unitInfos.levels[ennemyScript.CurrentLevel].PV;
            LifeBar.value = ennemyScript.currentHealth;
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
                if (BattleManager.Instance.order[i].GetComponent<Ennemy>().currentHealth <= (BattleManager.Instance.order[i].GetComponent<Ennemy>().data.levels[BattleManager.Instance.order[i].GetComponent<Ennemy>().CurrentLevel].PV) * 30 / 100)
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
                                               BattleManager.Instance.order[i].GetComponent<Ennemy>().data.levels[BattleManager.Instance.order[i].GetComponent<Ennemy>().CurrentLevel].PV;
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
                listeLifeBarCases[i].maxValue = BattleManager.Instance.order[i].GetComponent<Ennemy>().data.levels[BattleManager.Instance.order[i].GetComponent<Ennemy>().CurrentLevel].PV;
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
        CameraManager.Instance.canMove = false;
        buttonScript.SwitchButtonInteractible(false);
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
            CameraManager.Instance.canMove = true;
        }
        buttonScript.SwitchButtonInteractible(BattleManager.Instance.order[0].CompareTag("Unit"));
        CameraManager.Instance.canMove = (bool)BattleManager.Instance.order[0].CompareTag("Unit");
    }
    
    // ACTUALISE LE COMPTEUR DE POINTS DE MOUVEMENT
    public void UpdateMovePointsUI(Unit currentUnit)
    {
        compteurPointsMouvement.text = currentUnit.PM.ToString();
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
        
        PvEnnemiPre.text = Ennemi.currentHealth + " / " + Ennemi.data.levels[Ennemi.CurrentLevel].PV + " >";
        BarreDeVieEnnemiPre.maxValue = Ennemi.data.levels[Ennemi.CurrentLevel].PV;
        BarreDeVieEnnemiPre.value = Ennemi.currentHealth;
        
        if(Ennemi.currentHealth - damage > 0) PvEnnemiPost.text = Ennemi.currentHealth - damage + " / " + Ennemi.data.levels[Ennemi.CurrentLevel].PV;
        else PvEnnemiPost.text = 0 + " / " + Ennemi.data.levels[Ennemi.CurrentLevel].PV;
        BarreDeVieEnnemiPost.maxValue = Ennemi.data.levels[Ennemi.CurrentLevel].PV;
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

    
    
}
