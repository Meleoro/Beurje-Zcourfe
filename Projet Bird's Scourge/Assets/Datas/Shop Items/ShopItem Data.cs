using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ShopItem")]
public class ShopItemData : ScriptableObject
{
    
    public enum UseType
    {
        selectUnit,
        selectEnnemy,
        selectRange,
        selectTile
    }
    public UseType useType = UseType.selectUnit;
    
    public enum EffectType
    {
        heal,
        damage
    }
    public EffectType effectType = EffectType.heal;
   
    public enum type
    {
        wood,
        stone,
        iron,
        food,
        item,
        blessing
    } 
    public type itemType = type.wood;
   
    [Header("Général")]
    public string name;
    public int price;
    public Sprite image;
    public string description;
  
    [Header("Resources")]
    public int amount;

    [Header("Item")]
    //public ItemData associatedItem;

    [Header("Blessing")]
    public int ID;
}
