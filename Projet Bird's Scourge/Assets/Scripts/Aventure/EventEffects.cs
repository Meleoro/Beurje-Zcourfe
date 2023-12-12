using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EventEffects : MonoBehaviour
{
    public static EventEffects instance;
    public EventData eventData;
    public GameObject canvasEvent;
    public TextMeshProUGUI eventText;
    public TextMeshProUGUI eventTitle;
    public TextMeshProUGUI option1Text;
    public TextMeshProUGUI option2Text;
    public Button boutonHaut;
    public Button boutonBas;
    public int currentStep = 0;
    
    #region Bar d'état

    public TextMeshProUGUI compteurBoisState;
    public TextMeshProUGUI compteurPierreState;
    public TextMeshProUGUI compteurFerState;
    public TextMeshProUGUI compteurFoodState;
    public TextMeshProUGUI compteurGoldState;
    public TextMeshProUGUI nomUnit1;
    public TextMeshProUGUI nomUnit2;
    public TextMeshProUGUI nomUnit3;
    public TextMeshProUGUI compteurHPUnit1;
    public TextMeshProUGUI compteurHPUnit2;
    public TextMeshProUGUI compteurHPUnit3;
    public Slider lifeBarUnit1;
    public Slider lifeBarUnit2;
    public Slider lifeBarUnit3;

    #endregion
   
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void EventEffect(int index)
    {
        switch (eventData.ID)
            {
                case 1 :     // The Dark Deity ------------------------------------------
                    if (index == 1)
                    {
                        if (currentStep == 0)
                        {
                            StartCoroutine(ChangeStep(1));
                        }

                        if (currentStep == 1)
                        {
                            StartCoroutine(ChangeStep(2));
                            for (int i = 0; i < AventureManager.Instance.squadData.units.Count; i++)
                            {
                                AventureManager.Instance.squadData.unitsHealth[i] -= 5;
                                
                                if(AventureManager.Instance.squadData.unitsHealth[i] <= 0)
                                    AventureManager.Instance.squadData.unitsHealth[i] = 1;
                            }
                            UpdateStateBar();
                        }
                        
                        if (currentStep == 2)
                        {
                            StartCoroutine(ChangeStep(3));
                            
                            for (int i = 0; i < AventureManager.Instance.squadData.units.Count; i++)
                            {
                                AventureManager.Instance.squadData.unitsHealth[i] -= 7;
                                
                                if(AventureManager.Instance.squadData.unitsHealth[i] <= 0)
                                    AventureManager.Instance.squadData.unitsHealth[i] = 1;
                            }
                            UpdateStateBar();
                        }
                        
                        if (currentStep == 3)
                        {
                            ClosePopUp();
                            Debug.Log("New Blessing");
                            UpdateStateBar();
                        }
                    }
                    else
                    {
                        if (currentStep == 0)
                        {
                            ClosePopUp();
                        }
                        
                        if (currentStep == 1)
                        {
                            for (int i = 0; i < AventureManager.Instance.squadData.units.Count; i++)
                            {
                                AventureManager.Instance.squadData.unitsHealth[i] -= 3;
                                
                                if(AventureManager.Instance.squadData.unitsHealth[i] <= 0)
                                    AventureManager.Instance.squadData.unitsHealth[i] = 1;
                            }
                            ClosePopUp();
                            UpdateStateBar();
                        }
                        
                        if (currentStep == 2)
                        {
                            for (int i = 0; i < AventureManager.Instance.squadData.units.Count; i++)
                            {
                                AventureManager.Instance.squadData.unitsHealth[i] -= 5;
                                
                                if(AventureManager.Instance.squadData.unitsHealth[i] <= 0)
                                    AventureManager.Instance.squadData.unitsHealth[i] = 1;
                            }
                            ClosePopUp();
                            UpdateStateBar();
                        }
                        
                        if (currentStep == 3)
                        {
                            ClosePopUp();
                            Debug.Log("New Blessing");
                            UpdateStateBar();
                        }
                    }
                    break;
                
                case 2 :     // The Wanderer --------------------------------------------
                    if (index == 1)
                    {
                        if (currentStep == 0 && ResourcesSaveManager.Instance.gold >= 10)
                        {
                            StartCoroutine(ChangeStep(1));
                            ResourcesSaveManager.Instance.gold -= 10;
                            UpdateStateBar();
                        }
                        if (currentStep == 1)
                        {
                            ResourcesSaveManager.Instance.gold += 50;
                            UpdateStateBar();
                            ClosePopUp();
                            StartCoroutine(ChangeStep(0));
                        }
                        if (currentStep == 2)
                        {
                            for (int i = 0; i < AventureManager.Instance.squadData.units.Count; i++)
                            {
                                AventureManager.Instance.squadData.unitsHealth[i] -= 3;
                                
                                if(AventureManager.Instance.squadData.unitsHealth[i] <= 0)
                                    AventureManager.Instance.squadData.unitsHealth[i] = 1;
                            }
                            UpdateStateBar();
                            ClosePopUp();
                            StartCoroutine(ChangeStep(0));
                        }
                    }
                    else
                    {
                        if (currentStep == 0)
                        {
                            StartCoroutine(ChangeStep(2));
                        }
                        if (currentStep == 1)
                        {
                            ResourcesSaveManager.Instance.gold += 50;
                            UpdateStateBar();
                            ClosePopUp();
                            StartCoroutine(ChangeStep(0));
                        }
                        if (currentStep == 2)
                        {
                            for (int i = 0; i < AventureManager.Instance.squadData.units.Count; i++)
                            {
                                AventureManager.Instance.squadData.unitsHealth[i] -= 3;
                                
                                if(AventureManager.Instance.squadData.unitsHealth[i] <= 0)
                                    AventureManager.Instance.squadData.unitsHealth[i] = 1;
                            }
                            UpdateStateBar();
                            ClosePopUp();
                            StartCoroutine(ChangeStep(0));
                        } 
                    }
                   
                    break;
                
                case 3 :     // The Meal ------------------------------------------------
                    if (index == 1)
                    {
                        if (currentStep == 0)
                        {
                         
                            if (ResourcesSaveManager.Instance.food >= 10)
                            {
                                ResourcesSaveManager.Instance.food -= 10;
                                for (int i = 0; i < AventureManager.Instance.squadData.units.Count; i++)
                                {
                                    AventureManager.Instance.squadData.unitsHealth[i] += 7;

                                    int maxHP = AventureManager.Instance.squadData.units[i].GetComponent<Unit>().data.levels[
                                            AventureManager.Instance.squadData.units[i].GetComponent<Unit>()
                                                .CurrentLevel].PV;
                                
                                    if(AventureManager.Instance.squadData.unitsHealth[i] > maxHP)
                                        AventureManager.Instance.squadData.unitsHealth[i] = maxHP;
                                }
                                StartCoroutine(ChangeStep(1));
                                UpdateStateBar();
                            }
                        }

                        if (currentStep == 1)
                        {
                           ClosePopUp();
                        }
                    }
                    else
                    {
                        if (currentStep == 0)
                        {
                            ClosePopUp();
                        }
                        if (currentStep == 1)
                        {
                            ClosePopUp();
                        }
                    }
                    break;
                
                case 4 :     // The Old Sage --------------------------------------------
                    if (index == 1)
                    {
                        if (currentStep == 0)
                        {
                            StartCoroutine(ChangeStep(1));
                            // ajouet 50% d'XP à toutes les unités
                            //AventureManager.Instance.unit1.GetComponent<Unit>().xp += AventureManager.Instance.unit1.GetComponent<Unit>().data.levels[AventureManager.Instance.unit1.GetComponent<Unit>().CurrentLevel].maxXP * 50 / 100;
                            //AventureManager.Instance.unit2.GetComponent<Unit>().xp += AventureManager.Instance.unit2.GetComponent<Unit>().data.levels[AventureManager.Instance.unit2.GetComponent<Unit>().CurrentLevel].maxXP * 50 / 100;
                            //AventureManager.Instance.unit3.GetComponent<Unit>().xp += AventureManager.Instance.unit3.GetComponent<Unit>().data.levels[AventureManager.Instance.unit3.GetComponent<Unit>().CurrentLevel].maxXP * 50 / 100;
                            UpdateStateBar();
                        }
                        
                        if (currentStep == 1)
                        {
                            StartCoroutine(ChangeStep(1));
                            GameObject selectedUnit = AventureManager.Instance.squadData.units[Random.Range(0, 2)];
                            selectedUnit.GetComponent<Unit>().currentHealth /= 2 ;
                            UpdateStateBar();
                            ClosePopUp();
                        }
                    }
                    else
                    {
                        if (currentStep == 0)
                        {
                            ClosePopUp();
                        }
                        
                        if (currentStep == 1)
                        {
                            StartCoroutine(ChangeStep(1));
                            GameObject selectedUnit = AventureManager.Instance.squadData.units[Random.Range(0, 2)];
                            selectedUnit.GetComponent<Unit>().currentHealth /= 2 ;
                            UpdateStateBar();
                            ClosePopUp();
                        }
                    }
                    break;
              
                case 5 :     // The Merchant --------------------------------------------
                    if (index == 1)
                    {
                        if (currentStep == 0)
                        {
                            StartCoroutine(ChangeStep(1));
                            //if (il y a un item dans l'inventaire)
                            ResourcesSaveManager.Instance.gold += 20;
                            // Enlever un item aléatoire de l'invenaire
                            UpdateStateBar();
                        }

                        if (currentStep == 1)
                        {
                            ClosePopUp();
                        }
                    }
                    else
                    {
                        if (currentStep == 0)
                        {
                            ClosePopUp();
                        }
                        
                        if (currentStep == 1)
                        {
                            ClosePopUp();
                        }
                    }
                    break;
                
                case 6 :     // The Knight ---------------------------------------------- 
                    if (index == 1)
                    {
                        if (currentStep == 0)
                        {
                            StartCoroutine(ChangeStep(1));
                        }
                        
                        if (currentStep == 1)
                        {
                            ClosePopUp();
                        }
                        
                        if (currentStep == 2)
                        {
                            Debug.Log("ajoute une potion de force à l'inventaire");
                            UpdateStateBar();
                            ClosePopUp();
                        }
                    }
                    else
                    {
                        if (currentStep == 0)
                        {
                            StartCoroutine(ChangeStep(2)); 
                        }
                        
                        if (currentStep == 1)
                        {
                            ClosePopUp();
                        }
                        
                        if (currentStep == 2)
                        {
                            Debug.Log("ajoute une potion de force à l'inventaire");
                            UpdateStateBar();
                           ClosePopUp();
                        }
                    }
                    break;
                
                case 7 :     // The Camp------------------------------------------------- 
                    if (index == 1)
                    {
                        if (currentStep == 0)
                        {
                            StartCoroutine(ChangeStep(1));
                            ResourcesSaveManager.Instance.food += 20;
                            UpdateStateBar();
                        }
                        
                        if (currentStep == 1)
                        {
                            ClosePopUp();
                        }
                    }
                    else
                    {
                        if (currentStep == 0)
                        {
                            StartCoroutine(ChangeStep(1)); 
                            ResourcesSaveManager.Instance.wood += 20;
                            UpdateStateBar();
                        }
                        if (currentStep == 1)
                        {
                            ClosePopUp();
                        }
                    }
                    break;

                case 8 :     // --------------------------------------- 
                    if (index == 1)
                    {
                        if (currentStep == 0)
                        {
                            StartCoroutine(ChangeStep(1)); 
                        }
                        
                        if (currentStep == 1)
                        {
                            StartCoroutine(ChangeStep(3)); 
                        }
                        
                        if (currentStep == 2)
                        {
                            ClosePopUp();
                        }
                        
                        if (currentStep == 3)
                        {
                            ResourcesSaveManager.Instance.gold += 35;
                            UpdateStateBar();
                            ClosePopUp();
                        }
                    }
                    else
                    {
                        if (currentStep == 0)
                        {
                            StartCoroutine(ChangeStep(1)); 
                        }
                        
                        if (currentStep == 1)
                        {
                            StartCoroutine(ChangeStep(2)); 
                        }
                        
                        if (currentStep == 2)
                        {
                            ClosePopUp();
                        }
                        
                        if (currentStep == 3)
                        {
                            ResourcesSaveManager.Instance.gold += 35;
                            UpdateStateBar();
                            ClosePopUp();
                        }
                    }
                    break;
                
                case 999 :     // --------------------------------------- 
                    if (index == 1)
                    {
                        if (currentStep == 1)
                        {
                            StartCoroutine(ChangeStep(1));
                        }
                    }
                    else
                    {
                        if (currentStep == 1)
                        {
                            StartCoroutine(ChangeStep(1)); 
                        }
                    }
                    break;
            }
    }

    #region Fonctions Générales

    public void CoolDownButton()
    {
        StartCoroutine(CoolDownBouton());
    }
    
     public IEnumerator CoolDownBouton()
    {
        boutonBas.interactable = false;
        boutonHaut.interactable = false;
        yield return new WaitForSeconds(0.05f);
        boutonBas.interactable = true;
        boutonHaut.interactable = true;
    }
    
    public IEnumerator ChangeStep(int nextStep)
    {
        yield return new WaitForSeconds(0.1f);
        currentStep = nextStep;
        eventText.text = eventData.eventTexts[currentStep];
        option1Text.text = eventData.option1Text[currentStep];
        option2Text.text = eventData.option2Text[currentStep];
    }
    
    public void UpdateStateBar()
    {
        // Texte compteur de Ressources
        compteurBoisState.text = ResourcesSaveManager.Instance.wood.ToString();
        compteurPierreState.text = ResourcesSaveManager.Instance.stone.ToString();
        compteurFerState.text = ResourcesSaveManager.Instance.iron.ToString();
        compteurGoldState.text = ResourcesSaveManager.Instance.gold.ToString();
        compteurFoodState.text = ResourcesSaveManager.Instance.food.ToString();

        // Texte compteur de HP
        compteurHPUnit1.text = AventureManager.Instance.squadData.units[0].GetComponent<Unit>().currentHealth + " / " + AventureManager.Instance.squadData.units[0].GetComponent<Unit>()
            .data.levels[AventureManager.Instance.squadData.units[0].GetComponent<Unit>().CurrentLevel].PV + " HP";
        
        compteurHPUnit2.text = AventureManager.Instance.squadData.units[1].GetComponent<Unit>().currentHealth + " / " + AventureManager.Instance.squadData.units[1].GetComponent<Unit>()
            .data.levels[AventureManager.Instance.squadData.units[1].GetComponent<Unit>().CurrentLevel].PV + " HP";
        
        compteurHPUnit3.text = AventureManager.Instance.squadData.units[2].GetComponent<Unit>().currentHealth + " / " + AventureManager.Instance.squadData.units[2].GetComponent<Unit>()
            .data.levels[AventureManager.Instance.squadData.units[2].GetComponent<Unit>().CurrentLevel].PV + " HP";
        
        // Barres de vie
        lifeBarUnit1.maxValue = AventureManager.Instance.squadData.units[0].GetComponent<Unit>()
            .data.levels[AventureManager.Instance.squadData.units[0].GetComponent<Unit>().CurrentLevel].PV;

        lifeBarUnit1.value = AventureManager.Instance.squadData.units[0].GetComponent<Unit>().currentHealth;
        
        lifeBarUnit2.maxValue = AventureManager.Instance.squadData.units[1].GetComponent<Unit>()
            .data.levels[AventureManager.Instance.squadData.units[1].GetComponent<Unit>().CurrentLevel].PV;

        lifeBarUnit2.value = AventureManager.Instance.squadData.units[1].GetComponent<Unit>().currentHealth;
        
        lifeBarUnit3.maxValue = AventureManager.Instance.squadData.units[2].GetComponent<Unit>()
            .data.levels[AventureManager.Instance.squadData.units[2].GetComponent<Unit>().CurrentLevel].PV;

        lifeBarUnit3.value = AventureManager.Instance.squadData.units[2].GetComponent<Unit>().currentHealth;
       
    }
    
    public void ClosePopUp()
    {
        canvasEvent.transform.DOScale(Vector3.zero, 0.5f);
        currentStep = 0;
    }

    #endregion
    
   
}
