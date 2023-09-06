
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class GodessStatueManage : MonoBehaviour
{
   public static GodessStatueManage instance;
   public List<ShopItemData> blessingList;
   public List<TextMeshProUGUI> blessingNameList;
   public List<Image> blessingImageList;
   public List<GameObject> infoBubbleList;
   public List<TextMeshProUGUI> bubbleDescriptionList;

   public void Awake()
   {
      if (instance == null)
      {
         instance = this;
      }   }

   public void GenerateBlessing()
   {
      for (int i = 0; i < 3; i++)
      {
         ShopItemData chosenBlessing =
            AventureManager.Instance.possibleBlessings[Random.Range(0, AventureManager.Instance.possibleBlessings.Count - 1)];

         AventureManager.Instance.possibleBlessings.Remove(chosenBlessing); // Enlève la blessing tirée de possibleBlessing pour pas la tirer 2 fois
         
         blessingList.Add(chosenBlessing);
      }
      UpdatePopUpUI();
   }

   public void AddBlessing(int index)
   {
      if (index == 0)     // Remet les 2 autres blessings dans possibleBlessing mais pas celle choisie
      {
         AventureManager.Instance.possibleBlessings.Add(blessingList[1]);
         AventureManager.Instance.possibleBlessings.Add(blessingList[2]);
      } 
      else if(index == 1)
      {
         AventureManager.Instance.possibleBlessings.Add(blessingList[0]);
         AventureManager.Instance.possibleBlessings.Add(blessingList[2]);
      }
      else if(index == 2)
      {
         AventureManager.Instance.possibleBlessings.Add(blessingList[0]);
         AventureManager.Instance.possibleBlessings.Add(blessingList[1]);
      }
      
      BenedictionManager.instance.GetNewBlessing(blessingList[index]);
      AventureManager.Instance.possibleBlessings.Remove(blessingList[index]);
      blessingList.Clear();
   }

   public void UpdatePopUpUI()
   {
      for (int i = 0; i < 3; i++)
      {
         blessingNameList[i].text = blessingList[i].name;
         blessingImageList[i].sprite = blessingList[i].image;
         bubbleDescriptionList[i].text = blessingList[i].description;
      }
   }
   
   public void InfoBubbleOn(int index)
   {
      infoBubbleList[index].transform.localScale = Vector3.zero;
      infoBubbleList[index].SetActive(true);
      infoBubbleList[index].transform.DOScale(Vector3.one, 0.3f);
   }
   
   public void InfoBubbleOff(int index)
   {
      infoBubbleList[index].transform.DOScale(Vector3.zero, 0.3f);
      infoBubbleList[index].SetActive(false);
   }
}
