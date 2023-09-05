
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class GodessStatueManage : MonoBehaviour
{
   public static GodessStatueManage instance;
   public List<BenedictionData> blessingList;
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
         BenedictionData chosenBlessing =
            AventureManager.Instance.possibleBlessings[Random.Range(0, AventureManager.Instance.possibleBlessings.Count - 1)];

         for (int j = 0; j < BenedictionManager.instance.possessedBlessings.Count; j++)
         {
            if (chosenBlessing.ID == BenedictionManager.instance.possessedBlessings[j].ID)
               return;
         }
         blessingList.Add(AventureManager.Instance.possibleBlessings[Random.Range(0,AventureManager.Instance.possibleBlessings.Count - 1)]);
      }
      UpdatePopUpUI();
   }

   public void AddBlessing(int index)
   {
      BenedictionManager.instance.GetNewBlessing(blessingList[index]);
   }
   
   public void UpdatePopUpUI()
   {
      for (int i = 0; i < 3; i++)
      {
         blessingNameList[i].text = blessingList[i].name;
         blessingImageList[i].sprite = blessingList[i].icon;
         bubbleDescriptionList[i].text = blessingList[i].description;
      }
   }

   public void InfoBubbleOn(int index)
   {
      infoBubbleList[index].SetActive(true);
   }
   
   public void InfoBubbleOff(int index)
   {
      infoBubbleList[index].SetActive(false);
   }
}
