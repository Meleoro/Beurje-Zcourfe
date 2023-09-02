using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EventData")]
public class EventData : ScriptableObject
{
   public int ID;
   public Sprite eventImage;
   public string eventTitle;
   public string eventText;
   public string option1Text;
   public string option2Text;
}
