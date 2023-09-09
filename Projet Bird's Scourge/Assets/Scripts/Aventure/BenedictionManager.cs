using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class BenedictionManager : MonoBehaviour
{
    [Header("Général")] 
    public static BenedictionManager instance;
    public List<ShopItemData> possessedBlessings;

    [Header("UI")] 
    public List<Image> iconBlessingList;
    public List<GameObject> infoBubbleList;
    public List<TextMeshProUGUI> descriptionTextList;
    public List<TextMeshProUGUI> nameTextList;

    public void Awake()
    {
        if (instance == null) instance = this;
            else Destroy(gameObject);
        UpdateBlessingUI();
        
        for (int i = 0; i < possessedBlessings.Count; i++)
        {
            AventureManager.Instance.possibleBlessings.Remove(possessedBlessings[i]);
        }
    }

    public void GetNewBlessing(ShopItemData blessingGot)
    {
        possessedBlessings.Add(blessingGot);
        UpdateBlessingUI();
    }

    public bool CheckIfBlessingGot(int ID)
    {
        for (int i = 0; i < possessedBlessings.Count; i++)
        {
            if (ID == possessedBlessings[i].ID)
            {
                return true;
            }
        }
        return false;
    }
    

    public void UpdateBlessingUI()
    {
        for (int i = 0; i < possessedBlessings.Count; i++)
        {
            iconBlessingList[i].enabled = true;
            iconBlessingList[i].color = Color.white;
                iconBlessingList[i].sprite = possessedBlessings[i].image;
                nameTextList[i].text = possessedBlessings[i].name;
                descriptionTextList[i].text = possessedBlessings[i].description;
        }
    }

    public void OpenInfoBubble(int index)
    {
        if (possessedBlessings[index])
        {
            infoBubbleList[index].SetActive(true);
            infoBubbleList[index].transform.localScale = Vector3.zero;
            infoBubbleList[index].transform.DOScale(Vector3.one, 0.3f);
        }
    }
    
    public void CloseInfoBubble(int index)
    {
        if (possessedBlessings[index])
        {
            infoBubbleList[index].transform.DOScale(Vector3.zero, 0.3f);
            infoBubbleList[index].SetActive(false);
        }
    }

    public void BlessingEffect(int ID,Unit currentUnit, Ennemy currentEnnemy,int inflictedDamage)
    {
        List<Unit> concernedUnits = new List<Unit>();
        List<Ennemy> concernedEnnemys = new List<Ennemy>();
        concernedUnits.Add(currentUnit);
        concernedEnnemys.Add(currentEnnemy); 
        
        if (CheckIfBlessingGot(ID))
        {
            switch (ID)
            {
                case 0:
                    if (VerifyEnnemyHealth(currentEnnemy, 30, false))
                        BuffManager.Instance.AddBuff(BuffManager.BuffType.damage,20,1,concernedUnits,concernedEnnemys);
                    break;
                case 1:
                    BuffManager.Instance.AddBuff(BuffManager.BuffType.crit,5,1,concernedUnits,concernedEnnemys);
                        break;
                case 2:
                    if (VerifyUnitHealth(currentUnit, 30, false))
                        BuffManager.Instance.AddBuff(BuffManager.BuffType.accuracy,15,1,concernedUnits,concernedEnnemys);
                    break;
                case 3:
                    currentUnit.currentHealth += Mathf.RoundToInt(inflictedDamage / 10 * 100);
                    if (currentUnit.currentHealth >= currentUnit.data.levels[currentUnit.CurrentLevel].PV) 
                        currentUnit.currentHealth = currentUnit.data.levels[currentUnit.CurrentLevel].PV;
                    break;
                case 4:
                    if(VerifyEnnemyDead(currentEnnemy))
                        BuffManager.Instance.AddBuff(BuffManager.BuffType.damage,10,3,concernedUnits,concernedEnnemys);
                    break;
                case 5:
                    if(VerifyEnnemyDead(currentEnnemy))
                        currentUnit.currentHealth += Mathf.RoundToInt(currentUnit.data.levels[currentUnit.CurrentLevel].PV / 10 * 100);
                    if (currentUnit.currentHealth >= currentUnit.data.levels[currentUnit.CurrentLevel].PV)
                        currentUnit.currentHealth = currentUnit.data.levels[currentUnit.CurrentLevel].PV;
                    break;
                case 6:
                    if (inflictedDamage > 0) ResourcesSaveManager.Instance.gold += 1;
                    UIMapManager.Instance.UpdateStateBar();
                    break;
            } 
        }
        concernedUnits.Clear();
        concernedEnnemys.Clear();
    }
    
    
    // ---------------------------- Fonctions de Check de Conditions -------------------------------------------

    public bool VerifyUnitHealth(Unit currentUnit,int percentNeeded,bool moreOrLess)
    {
        if (moreOrLess)
        {
            if (currentUnit.currentHealth >= currentUnit.data.levels[currentUnit.CurrentLevel].PV * percentNeeded / 100) return true;
            else return false;

        }
        else
        {
            if (currentUnit.currentHealth <= currentUnit.data.levels[currentUnit.CurrentLevel].PV * percentNeeded / 100) return true;
            else return false;
        }
    }
    
    public bool VerifyEnnemyHealth(Ennemy currentEnnemy,int percentNeeded,bool moreOrLess)
    {
        if (moreOrLess)
        {
            if (currentEnnemy.currentHealth >= currentEnnemy.data.levels[currentEnnemy.CurrentLevel].PV * percentNeeded / 100) return true;
            else return false;

        }
        else
        {
            if (currentEnnemy.currentHealth <= currentEnnemy.data.levels[currentEnnemy.CurrentLevel].PV * percentNeeded / 100) return true;
            else return false;
        }
    }

    public bool VerifyEnnemyDead(Ennemy currentEnnemy)
    {
        if (currentEnnemy.currentHealth <= currentEnnemy.data.levels[currentEnnemy.CurrentLevel].PV) return true;
        else return false;
    }
    
}
