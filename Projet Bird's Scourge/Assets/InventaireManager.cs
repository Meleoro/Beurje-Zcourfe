using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventaireManager : MonoBehaviour
{
    public static InventaireManager Instance;

    public bool isCombat;
    public bool isShop;
    public bool destroyMode;
    public bool sellMode;
     
    public List<ShopItemData> inventory;
    public List<Image> itemImageList;
    public List<GameObject> infoBubbleList;
    public List<TextMeshProUGUI> descriptionList;
    public List<TextMeshProUGUI> nameList;
    public List<TextMeshProUGUI> sellPriceList;
    public List<Image> sellIconList;
    public List<Sprite> possibleIcons;
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
        if (!isOpen) // Ouvre
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
        else // Ferme
        {
            for (int i = 0; i < 3; i++)
            {
                fuctionButtonList[i].interactable = false;
            }
            fondInventaire.transform.DOLocalMove(closePos, 0.3f);
            yield return new WaitForSeconds(0.25f);
            anim.SetBool("Open",false);
            if(sellMode) ChangeSell();
            if(destroyMode) ChangeDestroy();
            fuctionButtonList[0].interactable = true;
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
                descriptionList[i].text = inventory[i].description;
                nameList[i].text = inventory[i].name;
                if (sellMode)
                {
                    sellPriceList[i].gameObject.SetActive(true);
                    sellIconList[i].gameObject.SetActive(true);
                    
                    sellPriceList[i].text = inventory[i].sellPrice.ToString();
                    sellIconList[i].sprite = possibleIcons[0];
                }
                if (destroyMode)
                {
                    sellPriceList[i].gameObject.SetActive(true);
                    sellIconList[i].gameObject.SetActive(true);
                        
                    sellPriceList[i].text = inventory[i].destroyResources.ToString();
                    switch (inventory[i].resourceGot)
                    {
                        case ShopItemData.PossibleRessources.wood:
                            sellIconList[i].sprite = possibleIcons[1];
                            break;
                        case ShopItemData.PossibleRessources.stone:
                            sellIconList[i].sprite = possibleIcons[2];
                            break;
                        case ShopItemData.PossibleRessources.iron:
                            sellIconList[i].sprite = possibleIcons[3];
                            break;
                        case ShopItemData.PossibleRessources.food:
                            sellIconList[i].sprite = possibleIcons[4];
                            break;
                    }
                }
            }
            else
            {
                itemImageList[i].sprite = null;
                itemImageList[i].color = Color.clear;
                sellPriceList[i].gameObject.SetActive(false);
                sellIconList[i].gameObject.SetActive(false);
            }
            
        }
    }

    public void UseItem(int index)
    {
        if (!destroyMode && !sellMode && isCombat)
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
        else if(sellMode)
        {
            if (inventory.Count - 1 >= index)
            {
                ResourcesSaveManager.Instance.gold += inventory[index].sellPrice;
                for (int i = 0; i < isUsing.Count; i++)
                {
                    isUsing[i] = false;
                }
                isUsing[index] = true;
                ConsumeItem();
                UIMapManager.Instance.UpdateStateBar();
                infoBubbleList[index].transform.DOScale(Vector3.zero, 0.3f);
                infoBubbleList[index].SetActive(false);
            }
        }
        else if (destroyMode)
        {
            if (inventory.Count - 1 >= index)
            {
                switch (inventory[index].resourceGot)
                {
                    case ShopItemData.PossibleRessources.wood:
                        ResourcesSaveManager.Instance.wood += inventory[index].destroyResources;
                        break;
                    case ShopItemData.PossibleRessources.stone:
                        ResourcesSaveManager.Instance.stone += inventory[index].destroyResources;
                        break;
                    case ShopItemData.PossibleRessources.iron:
                        ResourcesSaveManager.Instance.iron += inventory[index].destroyResources;
                        break;
                    case ShopItemData.PossibleRessources.food:
                        ResourcesSaveManager.Instance.food += inventory[index].destroyResources;
                        break;
                }
                ConsumeItem();
                UIMapManager.Instance.UpdateStateBar();
                infoBubbleList[index].transform.DOScale(Vector3.zero, 0.3f);
                infoBubbleList[index].SetActive(false);
            }
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

    public void OpenInfoBubble(int index)
    {
        if (inventory.Count - 1 >= index)
        {
            infoBubbleList[index].SetActive(true);
            infoBubbleList[index].transform.localScale = Vector3.zero;
            infoBubbleList[index].transform.DOScale(Vector3.one, 0.3f);
        }
    }
    
    public void CloseInfoBubble(int index)
    {
        if (inventory.Count - 1 >= index)
        {
            infoBubbleList[index].transform.DOScale(Vector3.zero, 0.3f);
            infoBubbleList[index].SetActive(false);
        }
    }

    public void ChangeDestroy()
    {
        destroyMode = !destroyMode;
        sellMode = false;
        UpdateInventoryUI();
    }
    
    public void ChangeSell()
    {
        destroyMode = false;
        if(isShop) sellMode = !sellMode;
        UpdateInventoryUI();
    }
}
