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

    [Header("Battle")] [SerializeField]
    private Image flashImage;
    [SerializeField] private Image bande1Image;
    [SerializeField] private Image bande2Image;
    private float flashDuration;

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
   
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        else
            Destroy(gameObject);
        
        
        UpdateStateBar();
    }
    
    public IEnumerator StartBattleEffect()
    {
        flashImage.DOFade(1, flashDuration * 0.3f).OnComplete((() => flashImage.DOFade(0, flashDuration * 0.7f)));

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
    
    public void LanguetteStateBar()
    {
        if (isOpen) stateBar.transform.DOMoveY(closeY, 0.5f);
        else stateBar.transform.DOMoveY(openY, 0.5f);
        isOpen = !isOpen;
    }
    
    public void UpdateStateBar()
    {
       compteurBoisState.text = ResourcesSaveManager.Instance.wood.ToString();
       compteurPierreState.text = ResourcesSaveManager.Instance.stone.ToString();
       compteurFerState.text = ResourcesSaveManager.Instance.iron.ToString();
       compteurGoldState.text = ResourcesSaveManager.Instance.gold.ToString();
       compteurFoodState.text = ResourcesSaveManager.Instance.food.ToString();
    }
    
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
    
    public IEnumerator EventPopUp()
    {
        eventData = AventureManager.Instance.possibleEvents[
            Random.Range(0, AventureManager.Instance.possibleEvents.Count - 1)];
        eventImage.sprite = eventData.eventImage;
        
       /* eventImageRect.width = eventData.eventImage.rect.width;
        eventImageRect.height = eventData.eventImage.rect.height;
        eventImage.rectTransform.rect.width = eventImageRect.width;*/
        
        eventText.text = eventData.eventText;
        eventTitle.text = eventData.eventTitle;
        option1Text.text = eventData.option1Text;
        option2Text.text = eventData.option2Text;

        canvasEvent.transform.localScale = Vector3.zero;
        canvasEvent.SetActive(true);
        canvasEvent.transform.DOScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    public void EventEffect(int index)
    {
        if (index == 1)
        {
            switch (eventData.ID)
            {
                case 1 :
                    break;
                case 2 :
                    ResourcesSaveManager.Instance.gold -= 10;
                    UpdateStateBar();
                    break;
                case 3 :
                    ResourcesSaveManager.Instance.food -= 15;
                    UpdateStateBar();
                    break;
            }
        }
        else
        {
            switch (eventData.ID)
            {
                case 1 :
                    break;
            }
        }
    }

    public void ClosePopUp()
    {
        canvasEvent.transform.DOScale(Vector3.zero, 0.5f);
        canvasCoffre.transform.DOScale(Vector3.zero, 0.5f);
        canvasCamp.transform.DOScale(Vector3.zero, 0.5f);
    }
    
    public IEnumerator CampPopUp()
    {
        canvasEvent.transform.localScale = Vector3.zero;
        canvasCamp.SetActive(true);
        canvasCamp.transform.DOScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }
    
   
}
