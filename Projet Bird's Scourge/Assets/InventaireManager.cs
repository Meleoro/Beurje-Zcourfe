using System;
using System.Collections;
using System.Collections.Generic;
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
   
    public bool isOpen;
    public Animator anim;
    public GameObject fondInventaire;
    public Vector2 openPos;
    public Vector2 closePos;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    public void OpenCloseInentaire()
    {
        if (!isOpen)
        {
            anim.SetBool("Open",true);
            fondInventaire.transform.DOLocalMove(openPos, 0.3f);
        }
        else
        {
            anim.SetBool("Open",false);
            fondInventaire.transform.DOLocalMove(closePos, 0.3f);
        }

        isOpen = !isOpen;
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            itemImageList[i].sprite = inventory[i].image;
            itemImageList[i].color = Color.white;
        }

      
    }

    public void UseItem(int index)
    {
        
    }
}
