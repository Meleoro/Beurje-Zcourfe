using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [Header("UnitSelect")] 
    public Color selectUnitOutline;
    private Unit mustBeSelectedSave;
    
    [Header("TilesSelect")] 
    public List<OverlayTile> tilesSelected;
    
    [Header("CurrentInfos")]
    private ShopItemData currentItem;

    [Header("References")] 
    private ObjectEffects effectScript;
    private MouseManager orignalScript;
    private RangeFinder rangeFinder;
    private EffectMaker effectMaker;


    private void Start()
    {
        effectScript = GetComponent<ObjectEffects>();
        orignalScript = GetComponent<MouseManager>();

        rangeFinder = new RangeFinder();
        effectMaker = new EffectMaker();
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
                TileClicked(hits[i].collider.GetComponent<OverlayTile>());
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
        if (currentItem.useType == ShopItemData.UseType.selectEnnemy)
        {
            if (currentItem.effectType == ShopItemData.EffectType.damage)
            {
                StartCoroutine(effectScript.DamageEffect(currentItem, clickedEnnemy));
            }
        }
    }
    
    private void TileClicked(OverlayTile clickedTile)
    {
        if (currentItem.useType == ShopItemData.UseType.selectTile)
        {
            if (currentItem.effectType == ShopItemData.EffectType.summon)
            {
                if (tilesSelected.Contains(clickedTile))
                {
                    Debug.Log("OUI");
                }
            }
        }
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
                MustSelectEnnemy(itemData);
                break;
            
            case ShopItemData.UseType.selectRange :
                MustSelectUnit(itemData);
                break;
            
            case ShopItemData.UseType.selectTile :
                MustSelectTile(itemData);
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
                StopMustSelectEnnemy(currentItem);
                break;
            
            case ShopItemData.UseType.selectRange :
                StopMustSelectUnit(currentItem);
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


    private void MustSelectEnnemy(ShopItemData itemData)
    {
        List<Ennemy> currentEnnemies = new List<Ennemy>(BattleManager.Instance.currentEnnemies);

        for (int i = 0; i < currentEnnemies.Count; i++)
        {
            currentEnnemies[i].StopAllCoroutines();
            currentEnnemies[i].ActivateFlicker(); 
        }
    }
    
    private void StopMustSelectEnnemy(ShopItemData itemData)
    {
        List<Ennemy> currentEnnemies = new List<Ennemy>(BattleManager.Instance.currentEnnemies);

        for (int i = 0; i < currentEnnemies.Count; i++)
        {
            currentEnnemies[i].StopAllCoroutines();
            currentEnnemies[i].DesactivateOutline(); 
        }
    }


    private void MustSelectTile(ShopItemData itemData)
    {
        Unit currentUnit = BattleManager.Instance.order[0].GetComponent<Unit>();

        tilesSelected = rangeFinder.FindTilesInRange(currentUnit.currentTile, 2);
        StartCoroutine(effectMaker.AttackTilesAppear(currentUnit.currentTile, tilesSelected, 0.05f, Color.magenta));
    }
}
