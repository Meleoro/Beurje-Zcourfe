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

    [Header("StartBattleTransitionReferences")] [SerializeField]
    private Image flashImage;

    [SerializeField] private Image bande1Image;
    [SerializeField] private Image bande2Image;

    [Header("StartBattleTransitionParameters")] [SerializeField]
    private float flashDuration;

    [Header("Proto Pop Up Coffre")] public GameObject canvasCoffre;
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
    public float boisTotal;
    public float pierreTotal;
    public float ferTotal;
    public float foodTotal;
    public float goldTotal;

    [Header("Pop UP Event")] public EventData eventData;
    public GameObject canvasEvent;
    public TextMeshProUGUI eventText;
    public TextMeshProUGUI eventTitle;
    public TextMeshProUGUI option1Text;
    public TextMeshProUGUI option2Text;
    public Image eventImage;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        else
            Destroy(gameObject);

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

    
    public IEnumerator ChestPopUp()
    {
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
            if (boisTotal < boisTotal + bois) boisTotal += 1;
            compteurBois.text = boisTotal + "";
            yield return new WaitForSeconds(0.1f);
        }

        compteurBoisGot.text = "+ " + bois;

        int fer = Random.Range(5, 50);
        for (int i = 0; i < fer; i++)
        {
            GameObject CreatedFer = Instantiate(iconeFer, animChest.transform);
            CreatedFer.transform.DOMove(FerLocation.transform.position, 0.5f).OnComplete((() => Destroy(CreatedFer)));
            if (ferTotal < ferTotal + fer) ferTotal += 1;
            compteurFer.text = ferTotal + "";
            yield return new WaitForSeconds(0.1f);
        }

        compteurFerGot.text = "+ " + fer;

        int pierre = Random.Range(5, 50);
        for (int i = 0; i < pierre; i++)
        {
            GameObject CreatedPierre = Instantiate(iconePierre, animChest.transform);
            CreatedPierre.transform.DOMove(pierreLocation.transform.position, 0.5f)
                .OnComplete((() => Destroy(CreatedPierre)));
            if (pierreTotal < pierreTotal + pierre) pierreTotal += 1;
            compteurPierre.text = pierreTotal + "";
            yield return new WaitForSeconds(0.1f);
        }

        compteurPierreGot.text = "+ " + pierre;

        int gold = Random.Range(5, 50);
        for (int i = 0; i < gold; i++)
        {
            GameObject CreatedGold = Instantiate(iconeGold, animChest.transform);
            CreatedGold.transform.DOMove(goldLocation.transform.position, 0.5f)
                .OnComplete((() => Destroy(CreatedGold)));
            if (goldTotal < goldTotal + gold) goldTotal += 1;
            compteurGold.text = goldTotal + "";
            yield return new WaitForSeconds(0.1f);
        }

        compteurGoldGot.text = "+ " + gold;

        int food = Random.Range(5, 50);
        for (int i = 0; i < food; i++)
        {
            GameObject CreatedFood = Instantiate(iconeFood, animChest.transform);
            CreatedFood.transform.DOMove(foodLocation.transform.position, 0.5f)
                .OnComplete((() => Destroy(CreatedFood)));
            if (foodTotal < foodTotal + food) foodTotal += 1;
            compteurFood.text = foodTotal + "";
            yield return new WaitForSeconds(0.1f);
        }

        compteurFoodGot.text = "+ " + food;
    }

    
    public IEnumerator EventPopUp()
    {
        eventImage.sprite = eventData.eventImage;
        eventText.text = eventData.eventText;
        eventTitle.text = eventData.eventTitle;
        option1Text.text = eventData.option1Text;
        option2Text.text = eventData.option2Text;

        canvasEvent.SetActive(true);
        canvasCoffre.transform.DOScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }
}
