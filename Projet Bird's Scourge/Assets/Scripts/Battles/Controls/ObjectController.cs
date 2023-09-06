using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [Header("UnitSelect")] 
    public Color selectUnitOutline;
    private Unit mustBeSelectedSave;
    
    [Header("CurrentInfos")]
    private ShopItemData currentItem;

    [Header("References")] 
    private ObjectEffects effectScript;
    private MouseManager orignalScript;


    private void Start()
    {
        effectScript = GetComponent<ObjectEffects>();
        orignalScript = GetComponent<MouseManager>();
    }


    public void ObjectSelectionUpdate()
    {
        VerifyOverlayedElement();
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            VerifyClickedElement();
        }
    }

    

    private void VerifyOverlayedElement()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);
        
        
    }

    private void VerifyClickedElement()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.CompareTag("Unit"))
            {
                UnitClicked(hits[i].collider.GetComponent<Unit>());
                break;
            }

            if (hits[i].collider.CompareTag("Ennemy"))
            {
                EnnemyClicked(hits[i].collider.GetComponent<Ennemy>());
                break;
            }

            if (hits[i].collider.CompareTag("Tile"))
            {
                TileClicked();
                break;
            }
        }
    }
    
    

    
    private void UnitClicked(Unit clickedUnit)
    {
        if (currentItem.useType == ShopItemData.UseType.selectUnit)
        {
            if (currentItem.effectType == ShopItemData.EffectType.heal)
            {
                StartCoroutine(effectScript.HealEffect(currentItem, clickedUnit));
            }
        }
    }
    
    private void EnnemyClicked(Ennemy clickedEnnemy)
    {
        if (currentItem.useType == ShopItemData.UseType.selectUnit)
        {
            if (currentItem.effectType == ShopItemData.EffectType.heal)
            {
                StartCoroutine(effectScript.DamageEffect(currentItem, clickedEnnemy));
            }
        }
    }
    
    private void TileClicked()
    {
        
    }
    
    
    
    
    // -------------------------- UI FUNCTIONS PART --------------------------
    
    public void UseObject(ShopItemData itemData)
    {
        currentItem = itemData;
        orignalScript.isUsingObject = true;
        
        switch (itemData.useType)
        {
            case ShopItemData.UseType.selectUnit :
                MustSelectUnit(itemData);
                break;
            
            case ShopItemData.UseType.selectEnnemy :
                break;
            
            case ShopItemData.UseType.selectRange :
                break;
            
            case ShopItemData.UseType.selectTile :
                break;
        }
    }

    public void StopUseObject()
    {
        switch (currentItem.useType)
        {
            case ShopItemData.UseType.selectUnit :
                StopMustSelectUnit(currentItem);
                break;
            
            case ShopItemData.UseType.selectEnnemy :
                break;
            
            case ShopItemData.UseType.selectRange :
                break;
            
            case ShopItemData.UseType.selectTile :
                break;
        }
        
        currentItem = null;
        orignalScript.isUsingObject = false;
    }


    private void MustSelectUnit(ShopItemData itemData)
    {
        List<Unit> currentUnits = new List<Unit>(BattleManager.Instance.currentUnits);

        for (int i = 0; i < currentUnits.Count; i++)
        {
            if (currentUnits[i].mustBeSelected)
                mustBeSelectedSave = currentUnits[i];
                
            
            currentUnits[i].StopAllCoroutines();
            currentUnits[i].outlineTurnLauched = false;
            currentUnits[i].mustBeSelected = true;
            currentUnits[i].objectFlicker = true;
        }
    }
    
    private void StopMustSelectUnit(ShopItemData itemData)
    {
        List<Unit> currentUnits = new List<Unit>(BattleManager.Instance.currentUnits);

        for (int i = 0; i < currentUnits.Count; i++)
        {
            if(mustBeSelectedSave != currentUnits[i])
                currentUnits[i].mustBeSelected = false;

            currentUnits[i].StopAllCoroutines();
            currentUnits[i].DesactivateOutline();
            currentUnits[i].outlineTurnLauched = false;
            currentUnits[i].objectFlicker = false;
        }
    }
}
