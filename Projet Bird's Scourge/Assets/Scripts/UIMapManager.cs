using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIMapManager : MonoBehaviour
{
    public static UIMapManager Instance;

    [Header("Battle")] 
    [SerializeField] private Image flashImage;
    [SerializeField] private Image bande1Image;
    [SerializeField] private Image bande2Image;
    [SerializeField] private float flashDuration;

    [Header("State Bar")] [SerializeField]
    private bool isOpen;
    public GameObject stateBar;
    public float openY;
    public float closeY;
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
    public Image imageUnit1;
    public Image imageUnit2;
    public Image imageUnit3;
    public GameObject quitConfirmation;
    
    [Header("Coffre")] [SerializeField]
    public GameObject canvasCoffre;
    public GameObject boisLocation;
    public GameObject FerLocation;
    public GameObject pierreLocation;
    public GameObject foodLocation;
    public GameObject goldLocation;
    public GameObject iconeBois;
    public GameObject iconePierre;
    public GameObject iconeFer;
    public GameObject iconeFood;
    public GameObject iconeGold;
    public Animator animChest;
    public TextMeshProUGUI compteurBois;
    public TextMeshProUGUI compteurPierre;
    public TextMeshProUGUI compteurFer;
    public TextMeshProUGUI compteurFood;
    public TextMeshProUGUI compteurGold;
    public TextMeshProUGUI compteurBoisGot;
    public TextMeshProUGUI compteurPierreGot;
    public TextMeshProUGUI compteurFerGot;
    public TextMeshProUGUI compteurFoodGot;
    public TextMeshProUGUI compteurGoldGot;
    
    [Header("Pop UP Event")] 
    public EventData eventData;
    public GameObject canvasEvent;
    public TextMeshProUGUI eventText;
    public TextMeshProUGUI eventTitle;
    public TextMeshProUGUI option1Text;
    public TextMeshProUGUI option2Text;
    public Image eventImage;
    private Rect eventImageRect;
    
    [Header("Pop UP Campement")] 
    public GameObject canvasCamp;
    public int percentHealed;
    
    [Header("Pop UP Statue")] 
    public GameObject canvasStatue;
    
    [Header("Pop UP Shop")] 
    public GameObject canvasShop;
    public TextMeshProUGUI merchantTalk;
    public TextMeshProUGUI itemName;
    public List<TextMeshProUGUI> pricesList;
    public List<Image> imagesList;
    public List<Button> slotList;
    [Range(1,4)] public int nbOfResources;
    [Range(1,7)] public int nbOfItems;
    [Range(1,7)] public int nbOfBlessings;
    public List<ShopItemData> possibleRessourcesList;
    public List<ShopItemData> finalItemList;
  

    #region Fonction Générales

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        else
            Destroy(gameObject);


        StartCoroutine(DelayUpdateStateBar());
    }
    

    public void LanguetteStateBar()
    {
        if (isOpen) stateBar.transform.DOMoveY(closeY, 0.5f);
        else stateBar.transform.DOMoveY(openY, 0.5f);
        isOpen = !isOpen;
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

    public IEnumerator DelayUpdateStateBar()
    {
        yield return new WaitForSeconds(1f);
        UpdateStateBar();
        nomUnit1.text = AventureManager.Instance.unit1.GetComponent<Unit>().data.charaName;
        nomUnit2.text = AventureManager.Instance.unit2.GetComponent<Unit>().data.charaName;
        nomUnit3.text = AventureManager.Instance.unit3.GetComponent<Unit>().data.charaName;
        imageUnit1.sprite = AventureManager.Instance.unit1.GetComponent<Unit>().data.idleSprite;
        imageUnit2.sprite = AventureManager.Instance.unit2.GetComponent<Unit>().data.idleSprite;
        imageUnit3.sprite = AventureManager.Instance.unit3.GetComponent<Unit>().data.idleSprite;
    }
    public void ClosePopUp()
    {
        canvasEvent.transform.DOScale(Vector3.zero, 0.5f);
        canvasCoffre.transform.DOScale(Vector3.zero, 0.5f);
        canvasCamp.transform.DOScale(Vector3.zero, 0.5f);
        canvasShop.transform.DOScale(Vector3.zero, 0.5f);
        canvasStatue.transform.DOScale(Vector3.zero, 0.5f);
    }

    public void QuitExploration()
    {
        quitConfirmation.transform.localScale = Vector3.zero;
        quitConfirmation.SetActive(true);
        quitConfirmation.transform.DOScale(Vector3.one, 0.5f);
    }
    
    public void QuitEffect(int index)
    {
        if (index == 1)
        {
            quitConfirmation.transform.DOScale(Vector3.zero, 0.5f);
        }
        else
        {
            Debug.Log("Quit Exploration");
        }
    }
    #endregion

    #region Fonctions Battle
    public IEnumerator StartBattleEffect()
    {
        flashImage.DOFade(1, flashDuration * 0.3f).OnComplete((() => flashImage.DOFade(0, flashDuration * 0.7f)));
        
        bande1Image.rectTransform.DOLocalMoveY(0, 0);
        bande2Image.rectTransform.DOLocalMoveY(0, 0);

        yield return new WaitForSeconds(flashDuration * 2);

        bande1Image.rectTransform.DOScaleY(280, 1);
        bande1Image.rectTransform.DOLocalMoveY(bande1Image.rectTransform.localPosition.y + 280 * 0.5f, 1);

        bande2Image.rectTransform.DOScaleY(280, 1);
        bande2Image.rectTransform.DOLocalMoveY(bande2Image.rectTransform.localPosition.y - 280 * 0.5f, 1);

        yield return new WaitForSeconds(1);

        bande1Image.rectTransform.DOScaleY(0, 1);
        bande1Image.rectTransform.DOLocalMoveY(bande1Image.rectTransform.localPosition.y + 280 * 0.5f, 1);

        bande2Image.rectTransform.DOScaleY(0, 1);
        bande2Image.rectTransform.DOLocalMoveY(bande2Image.rectTransform.localPosition.y - 280 * 0.5f, 1);
    }
    
    #endregion

    #region Fonctions Chest
  public IEnumerator ChestPopUp()
    {
        compteurBois.text = ResourcesSaveManager.Instance.wood + "";
        compteurPierre.text = ResourcesSaveManager.Instance.stone + "";
        compteurFer.text = ResourcesSaveManager.Instance.iron + "";
        compteurGold.text = ResourcesSaveManager.Instance.gold + "";
        compteurFood.text = ResourcesSaveManager.Instance.food + "";
        
        canvasEvent.transform.localScale = Vector3.zero;
        canvasCoffre.SetActive(true);
        canvasCoffre.transform.DOScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(0.8f);
        animChest.gameObject.transform.DOShakePosition(1, 2f);
        animChest.gameObject.transform.DOShakeRotation(1, 2f);
        yield return new WaitForSeconds(1f);
        animChest.SetBool("Open", true);
        yield return new WaitForSeconds(1f);


        int bois = Random.Range(5, 50);
        for (int i = 0; i < bois; i++)
        {
            GameObject CreatedBois = Instantiate(iconeBois, animChest.transform);
            CreatedBois.transform.DOMove(boisLocation.transform.position, 0.5f)
                .OnComplete((() => Destroy(CreatedBois)));
            if (ResourcesSaveManager.Instance.wood < ResourcesSaveManager.Instance.wood + bois) ResourcesSaveManager.Instance.wood += 1;
            compteurBois.text = ResourcesSaveManager.Instance.wood + "";
            yield return new WaitForSeconds(0.1f);
        }

        compteurBoisGot.text = "+ " + bois;

        int fer = Random.Range(5, 50);
        for (int i = 0; i < fer; i++)
        {
            GameObject CreatedFer = Instantiate(iconeFer, animChest.transform);
            CreatedFer.transform.DOMove(FerLocation.transform.position, 0.5f).OnComplete((() => Destroy(CreatedFer)));
            if (ResourcesSaveManager.Instance.iron < ResourcesSaveManager.Instance.iron + fer) ResourcesSaveManager.Instance.iron += 1;
            compteurFer.text = ResourcesSaveManager.Instance.iron + "";
            yield return new WaitForSeconds(0.1f);
        }

        compteurFerGot.text = "+ " + fer;

        int pierre = Random.Range(5, 50);
        for (int i = 0; i < pierre; i++)
        {
            GameObject CreatedPierre = Instantiate(iconePierre, animChest.transform);
            CreatedPierre.transform.DOMove(pierreLocation.transform.position, 0.5f)
                .OnComplete((() => Destroy(CreatedPierre)));
            if (ResourcesSaveManager.Instance.stone < ResourcesSaveManager.Instance.stone + pierre) ResourcesSaveManager.Instance.stone += 1;
            compteurPierre.text = ResourcesSaveManager.Instance.stone + "";
            yield return new WaitForSeconds(0.1f);
        }

        compteurPierreGot.text = "+ " + pierre;

        int gold = Random.Range(5, 50);
        for (int i = 0; i < gold; i++)
        {
            GameObject CreatedGold = Instantiate(iconeGold, animChest.transform);
            CreatedGold.transform.DOMove(goldLocation.transform.position, 0.5f)
                .OnComplete((() => Destroy(CreatedGold)));
            if (ResourcesSaveManager.Instance.gold < ResourcesSaveManager.Instance.gold + gold) ResourcesSaveManager.Instance.gold += 1;
            compteurGold.text = ResourcesSaveManager.Instance.gold + "";
            yield return new WaitForSeconds(0.1f);
        }

        compteurGoldGot.text = "+ " + gold;

        int food = Random.Range(5, 50);
        for (int i = 0; i < food; i++)
        {
            GameObject CreatedFood = Instantiate(iconeFood, animChest.transform);
            CreatedFood.transform.DOMove(foodLocation.transform.position, 0.5f)
                .OnComplete((() => Destroy(CreatedFood)));
            if (ResourcesSaveManager.Instance.food < ResourcesSaveManager.Instance.food + food) ResourcesSaveManager.Instance.food += 1;
            compteurFood.text = ResourcesSaveManager.Instance.food + "";
            yield return new WaitForSeconds(0.1f);
        }

        compteurFoodGot.text = "+ " + food;
        UpdateStateBar();
    }
    

    #endregion

    #region Fonctions Event

    public IEnumerator EventPopUp()
    {
        eventData = AventureManager.Instance.possibleEvents[Random.Range(0,AventureManager.Instance.possibleEvents.Count - 1)];
        eventImage.sprite = eventData.eventImage;
        EventEffects.instance.eventData = eventData;
        
        /* eventImageRect.width = eventData.eventImage.rect.width;
         eventImageRect.height = eventData.eventImage.rect.height;
         eventImage.rectTransform.rect.width = eventImageRect.width;*/
        
        eventTitle.text = eventData.eventTitle;
        eventText.text = eventData.eventTexts[0];
        option1Text.text = eventData.option1Text[0];
        option2Text.text = eventData.option2Text[0];

        canvasEvent.transform.localScale = Vector3.zero;
        canvasEvent.SetActive(true);
        canvasEvent.transform.DOScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }
    
    #endregion

    #region Fonctions Campement

    public IEnumerator CampPopUp()
    {
        canvasEvent.transform.localScale = Vector3.zero;
        canvasCamp.SetActive(true);
        canvasCamp.transform.DOScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    public void CampEffect(int index)
    {
        if (index == 1)
        {
            AventureManager.Instance.unit1.GetComponent<Unit>().currentHealth +=
                (AventureManager.Instance.unit1.GetComponent<Unit>()
                    .data.levels[AventureManager.Instance.unit1.GetComponent<Unit>().CurrentLevel].PV * percentHealed / 100);

            if (AventureManager.Instance.unit1.GetComponent<Unit>().currentHealth > AventureManager.Instance.unit1
                    .GetComponent<Unit>()
                    .data.levels[AventureManager.Instance.unit1.GetComponent<Unit>().CurrentLevel].PV)
            {
                AventureManager.Instance.unit1.GetComponent<Unit>().currentHealth = AventureManager.Instance.unit1
                    .GetComponent<Unit>()
                    .data.levels[AventureManager.Instance.unit1.GetComponent<Unit>().CurrentLevel].PV;
            }
              
            
            AventureManager.Instance.unit2.GetComponent<Unit>().currentHealth +=
                (AventureManager.Instance.unit2.GetComponent<Unit>()
                    .data.levels[AventureManager.Instance.unit2.GetComponent<Unit>().CurrentLevel].PV * percentHealed / 100);
            
            if (AventureManager.Instance.unit2.GetComponent<Unit>().currentHealth > AventureManager.Instance.unit2
                    .GetComponent<Unit>()
                    .data.levels[AventureManager.Instance.unit2.GetComponent<Unit>().CurrentLevel].PV)
            {
                AventureManager.Instance.unit2.GetComponent<Unit>().currentHealth = AventureManager.Instance.unit2
                    .GetComponent<Unit>()
                    .data.levels[AventureManager.Instance.unit2.GetComponent<Unit>().CurrentLevel].PV;
            }
            
            AventureManager.Instance.unit3.GetComponent<Unit>().currentHealth +=
                (AventureManager.Instance.unit3.GetComponent<Unit>()
                    .data.levels[AventureManager.Instance.unit3.GetComponent<Unit>().CurrentLevel].PV * percentHealed / 100);
            
            if (AventureManager.Instance.unit3.GetComponent<Unit>().currentHealth > AventureManager.Instance.unit3
                    .GetComponent<Unit>()
                    .data.levels[AventureManager.Instance.unit3.GetComponent<Unit>().CurrentLevel].PV)
            {
                AventureManager.Instance.unit3.GetComponent<Unit>().currentHealth = AventureManager.Instance.unit3
                    .GetComponent<Unit>()
                    .data.levels[AventureManager.Instance.unit3.GetComponent<Unit>().CurrentLevel].PV;
            }
            
            UpdateStateBar();
            ClosePopUp();
        }
        else
        {
            Debug.Log("Quit Aventure");
        }
            
    }

    #endregion

    #region Fonctions Statue

    public IEnumerator PopUpStatue()
    {
        GodessStatueManage.instance.GenerateBlessing();
        canvasStatue.transform.localScale = Vector3.zero;
        canvasStatue.SetActive(true);
        canvasStatue.transform.DOScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    #endregion
    
    #region Fonctions Shop

    public IEnumerator PopUpShop()
    {
        GenerateItemList();
        InventaireManager.Instance.isShop = true;
        
        for (int i = 0; i < pricesList.Count; i++)
        {
            imagesList[i].sprite = finalItemList[i].image;
            pricesList[i].fontSize = 25f;
            pricesList[i].color = new Color32(128, 97, 66,255);
            pricesList[i].text = finalItemList[i].price.ToString();
            if (finalItemList[i].price > ResourcesSaveManager.Instance.gold)
            {
                pricesList[i].color = new Color32(170, 0, 0,255);
            }
        }
        
        canvasShop.transform.localScale = Vector3.zero;
        canvasShop.SetActive(true);
        canvasShop.transform.DOScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(0.5f);
        UpdateItemPrice();
    }

    public void UpdateMerchantText(int index)
    {
        if (finalItemList[index].itemType == ShopItemData.type.wood ||
            finalItemList[index].itemType == ShopItemData.type.stone ||
            finalItemList[index].itemType == ShopItemData.type.iron ||
            finalItemList[index].itemType == ShopItemData.type.food)
            itemName.text = finalItemList[index].amount + " " + finalItemList[index].name;
        else itemName.text = finalItemList[index].name;
        merchantTalk.text = finalItemList[index].description;
    }

    public void UpdateItemPrice()
    {
        for (int i = 0; i < pricesList.Count; i++)
        {
            if (finalItemList[i].price > ResourcesSaveManager.Instance.gold)
            {
                pricesList[i].color = new Color32(170, 0, 0,255);
            }
        }
    }

    public void GenerateItemList()
    {
        for (int i = 0; i < nbOfResources; i++)
        {
            ShopItemData chosenResource = possibleRessourcesList[Random.Range(0, possibleRessourcesList.Count - 1)];
            finalItemList.Add(chosenResource);
            possibleRessourcesList.Remove(chosenResource);
        }
        
        for (int i = 0; i < nbOfItems; i++)
        {
            finalItemList.Add(AventureManager.Instance.possibleItems[Random.Range(0,AventureManager.Instance.possibleItems.Count - 1)]);
        }
        
        for (int i = 0; i < nbOfBlessings; i++)
        {
            ShopItemData chosenBlessing =
                AventureManager.Instance.possibleBlessings[Random.Range(0, AventureManager.Instance.possibleBlessings.Count - 1)];

            AventureManager.Instance.possibleBlessings.Remove(chosenBlessing); // Enlève la blessing tirée de possibleBlessing pour pas la tirer 2 fois
            finalItemList.Add(chosenBlessing);
        }
    }
    
    public void BuyItem(int index)
    {
        if (ResourcesSaveManager.Instance.gold >= finalItemList[index].price) // Check si on peut l'acheter
        {
            ResourcesSaveManager.Instance.gold -= finalItemList[index].price; // Enlève l'agent
           
            switch (finalItemList[index].itemType) // Execute l'effet
            {
                case ShopItemData.type.wood:
                    ResourcesSaveManager.Instance.wood += finalItemList[index].amount;
                    break;

                case ShopItemData.type.stone:
                    ResourcesSaveManager.Instance.stone += finalItemList[index].amount;
                    break;

                case ShopItemData.type.iron:
                    ResourcesSaveManager.Instance.iron += finalItemList[index].amount;
                    break;

                case ShopItemData.type.food:
                    ResourcesSaveManager.Instance.food += finalItemList[index].amount;
                    break;
                case ShopItemData.type.blessing:
                    BenedictionManager.instance.GetNewBlessing(finalItemList[index]);
                    slotList[index].interactable = false;
                    pricesList[index].color = new Color32(170, 0, 0,255);
                    pricesList[index].fontSize = 21.5f;
                    pricesList[index].text = "Sold";
                    break;
                case ShopItemData.type.item:
                  // InventaireManager.Instance.GetNewItem(finalItemList[index]);
                   InventaireManager.Instance.inventory.Add(finalItemList[index]);
                   InventaireManager.Instance.UpdateInventoryUI();
                    slotList[index].interactable = false;
                    pricesList[index].color = new Color32(170, 0, 0,255);
                    pricesList[index].fontSize = 21.5f;
                    pricesList[index].text = "Sold";
                    break;
            }
            UpdateStateBar();
            UpdateItemPrice();
        }
    }

    public void CloseShop()
    {
        foreach (ShopItemData item in finalItemList)  // Remet les blessings non utilisée dans les possible blessings
        {
            if (item.itemType == ShopItemData.type.blessing)
            {
                if (!BenedictionManager.instance.possessedBlessings.Contains(item))
                {
                    AventureManager.Instance.possibleBlessings.Add(item);
                }
            }
            
            if (item.itemType == ShopItemData.type.wood || item.itemType == ShopItemData.type.stone || item.itemType == ShopItemData.type.iron || item.itemType == ShopItemData.type.food)
            {
                possibleRessourcesList.Add(item);
            }
        }

        InventaireManager.Instance.isShop = false;
        finalItemList.Clear();
    }

    #endregion
}
