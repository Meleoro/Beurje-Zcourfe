using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [Header("UnitSelect")] 
    public Color selectUnitOutline;
    
    
    private ShopItemData currentItem;
    
    
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
                UnitClicked();
                break;
            }

            if (hits[i].collider.CompareTag("Ennemy"))
            {
                EnnemyClicked();
                break;
            }

            if (hits[i].collider.CompareTag("Tile"))
            {
                TileClicked();
                break;
            }
        }
    }
    
    

    
    private void UnitClicked()
    {
        if (currentItem.useType == ShopItemData.UseType.selectUnit)
        {
            if (currentItem.effectType == ShopItemData.EffectType.heal)
            {
                
            }
        }
    }
    
    private void EnnemyClicked()
    {
        
    }
    
    private void TileClicked()
    {
        
    }
    
    
    
    
    // -------------------------- UI FUNCTIONS PART --------------------------
    
    public void UseObject(ShopItemData itemData)
    {
        currentItem = itemData;
        
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


    private void MustSelectUnit(ShopItemData itemData)
    {
        List<Unit> currentUnits = new List<Unit>(BattleManager.Instance.currentUnits);

        for (int i = 0; i < currentUnits.Count; i++)
        {
            currentUnits[i].ActivateOutline(selectUnitOutline);
        }
    }
}
