using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EventData")]
public class EventData : ScriptableObject
{
   public int ID;
   public Sprite eventImage;
   public string eventTitle;
   public List<string> eventTexts;
   public List<string> option1Text;
   public List<string> option2Text;
}
