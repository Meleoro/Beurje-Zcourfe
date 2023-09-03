using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
        if (index == 1) // BOUTON DU HAUT ----------------------------------------------------------------------------------------
        {
            switch (eventData.ID)
            {
                case 1 :      // The Cursed Book ----------------------------------------
                    break;
                
                case 2 :     // The Wanderer --------------------------------------------
                    if (currentStep == 0 && ResourcesSaveManager.Instance.gold >= 10)
                    {
                        CoolDownBouton();
                        StartCoroutine(CoolDownChangeStep(1));
                       
                        ResourcesSaveManager.Instance.gold -= 10;
                        UpdateStateBar();
                    }
                    if (currentStep == 1)
                    {
                        CoolDownBouton();
                        ResourcesSaveManager.Instance.gold += 50;
                        UpdateStateBar();
                        ClosePopUp();
                        StartCoroutine(CoolDownChangeStep(0));
                    }
                    if (currentStep == 2)
                    {
                        CoolDownBouton();
                        if(AventureManager.Instance.unit1.GetComponent<Unit>().currentHealth > 3) AventureManager.Instance.unit1.GetComponent<Unit>().currentHealth -= 3;
                        if(AventureManager.Instance.unit2.GetComponent<Unit>().currentHealth > 3) AventureManager.Instance.unit2.GetComponent<Unit>().currentHealth -= 3;
                        if(AventureManager.Instance.unit3.GetComponent<Unit>().currentHealth > 3) AventureManager.Instance.unit3.GetComponent<Unit>().currentHealth -= 3;
                        UpdateStateBar();
                        ClosePopUp();
                        StartCoroutine(CoolDownChangeStep(0));
                    }
                    break;
                
                case 3 :       // The Meal ----------------------------------------------
                    if (ResourcesSaveManager.Instance.food >= 15)
                    {
                        ResourcesSaveManager.Instance.food -= 15;
                        UpdateStateBar();
                    }
                    break;
            }
        }
        else   // BOUTON DU BAS -----------------------------------------------------------------------------------------------
        {
            switch (eventData.ID)
            {
                case 1 :
                    break;
                case 2 :
                    if (currentStep == 0)
                    {
                        CoolDownBouton();
                        StartCoroutine(CoolDownChangeStep(2));
                    }
                    if (currentStep == 1)
                    {
                        CoolDownBouton();
                        ResourcesSaveManager.Instance.gold += 50;
                        UpdateStateBar();
                        ClosePopUp();
                        StartCoroutine(CoolDownChangeStep(0));
                    }
                    if (currentStep == 2)
                    {
                        CoolDownBouton();
                        if(AventureManager.Instance.unit1.GetComponent<Unit>().currentHealth > 3) AventureManager.Instance.unit1.GetComponent<Unit>().currentHealth -= 3;
                        if(AventureManager.Instance.unit2.GetComponent<Unit>().currentHealth > 3) AventureManager.Instance.unit2.GetComponent<Unit>().currentHealth -= 3;
                        if(AventureManager.Instance.unit3.GetComponent<Unit>().currentHealth > 3) AventureManager.Instance.unit3.GetComponent<Unit>().currentHealth -= 3;
                        UpdateStateBar();
                        ClosePopUp();
                        StartCoroutine(CoolDownChangeStep(0));
                    }
                    break;
            }
        }
    }

    public void CoolDownBouton()
    {
        StartCoroutine(CoolDownBoutonTimer());
    }

    public IEnumerator CoolDownBoutonTimer()
    {
        boutonBas.interactable = false;
        boutonHaut.interactable = false;
        yield return new WaitForSeconds(0.05f);
        boutonBas.interactable = true;
        boutonHaut.interactable = true;
    }
    
    public IEnumerator CoolDownChangeStep(int nextStep)
    {
        yield return new WaitForSeconds(0.5f);
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
        
        compteurHPUnit1.text = AventureManager.Instance.unit1.GetComponent<Unit>().currentHealth + " / " + AventureManager.Instance.unit1.GetComponent<Unit>()
            .data.levels[AventureManager.Instance.unit1.GetComponent<Unit>().CurrentLevel].PV + " HP";
        
        compteurHPUnit2.text = AventureManager.Instance.unit2.GetComponent<Unit>().currentHealth + " / " + AventureManager.Instance.unit2.GetComponent<Unit>()
            .data.levels[AventureManager.Instance.unit2.GetComponent<Unit>().CurrentLevel].PV + " HP";
        
        compteurHPUnit3.text = AventureManager.Instance.unit3.GetComponent<Unit>().currentHealth + " / " + AventureManager.Instance.unit3.GetComponent<Unit>()
            .data.levels[AventureManager.Instance.unit3.GetComponent<Unit>().CurrentLevel].PV + " HP";
        
        // Barres de vie
        
        lifeBarUnit1.maxValue = AventureManager.Instance.unit1.GetComponent<Unit>()
            .data.levels[AventureManager.Instance.unit1.GetComponent<Unit>().CurrentLevel].PV;

        lifeBarUnit1.value = AventureManager.Instance.unit1.GetComponent<Unit>().currentHealth;
        
        lifeBarUnit2.maxValue = AventureManager.Instance.unit2.GetComponent<Unit>()
            .data.levels[AventureManager.Instance.unit2.GetComponent<Unit>().CurrentLevel].PV;

        lifeBarUnit2.value = AventureManager.Instance.unit2.GetComponent<Unit>().currentHealth;
        
        lifeBarUnit3.maxValue = AventureManager.Instance.unit3.GetComponent<Unit>()
            .data.levels[AventureManager.Instance.unit3.GetComponent<Unit>().CurrentLevel].PV;

        lifeBarUnit3.value = AventureManager.Instance.unit3.GetComponent<Unit>().currentHealth;
       
    }
    public void ClosePopUp()
    {
        canvasEvent.transform.DOScale(Vector3.zero, 0.5f);
    }
}
