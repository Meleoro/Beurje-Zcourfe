using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InventaireManager : MonoBehaviour
{
    public static InventaireManager Instance;

    public List<ShopItemData> inventory;
    public List<Image> itemImageList;
    public List<Image> infoBubbleList;
    public List<Image> descriptionList;
    public List<Image> nameList;
    public List<Button> fuctionButtonList;
    public List<Boolean> isUsing;
    public bool isOpen;
    
    public Animator anim;
    public GameObject fondInventaire;
    public Vector2 openPos;
    public Vector2 closePos;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        UpdateInventoryUI();
    }
    
    public void GetNewItem(ShopItemData itemGot)
    {
        inventory.Add(itemGot); 
        UpdateInventoryUI();
    }
    
    public void OpenCloseInentaire()
    {
        StartCoroutine(waitInventory());

        for (int i = 0; i < isUsing.Count; i++)
        {
            if (isUsing[i])
            {
                ObjectController.instance.StopUseObject();
                isUsing[i] = false;
            }
        }
    }

    public IEnumerator waitInventory()
    {
        if (!isOpen)
        {
            for (int i = 0; i < 3; i++)
            {
                fuctionButtonList[i].interactable = false;
            }
            anim.SetBool("Open", true);
            yield return new WaitForSeconds(0.3f);
            fondInventaire.transform.DOLocalMove(openPos, 0.3f);
            yield return new WaitForSeconds(0.3f);
            
            for (int i = 0; i < 3; i++)
            {
                fuctionButtonList[i].interactable = true;
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                fuctionButtonList[i].interactable = false;
            }
            fondInventaire.transform.DOLocalMove(closePos, 0.3f);
            yield return new WaitForSeconds(0.25f);
            anim.SetBool("Open",false);
            for (int i = 0; i < 3; i++)
            {
                fuctionButtonList[i].interactable = true;
            }

        }
        isOpen = !isOpen; 
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < itemImageList.Count; i++)
        {
            if (inventory.Count - 1 >= i)
            {
                itemImageList[i].sprite = inventory[i].image;
                itemImageList[i].color = Color.white;
            }
            else
            {
                itemImageList[i].sprite = null;
                itemImageList[i].color = Color.clear;
            }
            
        }
    }

    public void UseItem(int index)
    {
        if (inventory.Count - 1 >= index)
        {
            ObjectController.instance.UseObject(inventory[index]);
            for (int i = 0; i < isUsing.Count; i++)
            {
                isUsing[i] = false;
            }
            isUsing[index] = true;
        }
      
    }

    public void ConsumeItem()
    {
        int index = 0;
        for (int i = 0; i < isUsing.Count; i++)
        {
            if(isUsing[i] == true) index = i;
        }
        inventory.Remove(inventory[index]);
        isUsing[index] = false;
        UpdateInventoryUI();
    }
}
