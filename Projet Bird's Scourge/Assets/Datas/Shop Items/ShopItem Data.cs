using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ShopItem")]
public class ShopItemData : ScriptableObject
{
    public string name;
    public int price;
    public Sprite image;
    public string description;
    public int amount;
    public int PercentHPHealed;
    public enum type
    {
        wood,
        stone,
        iron,
        food,
        potion,
        item,
        blessing
    } 
    public type itemType = type.wood;
   
}
