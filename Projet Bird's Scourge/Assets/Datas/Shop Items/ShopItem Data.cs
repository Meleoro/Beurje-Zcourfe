using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ShopItem")]
public class ShopItemData : ScriptableObject
{
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
        damage,
        buff,
        summon,
    }
    public EffectType effectType = EffectType.heal;

    public int healAmount;
    public int damageAmount;

    [Header("Buff Item")]
    public BuffManager.BuffType buffType = BuffManager.BuffType.damage;
    public int damageBuffAmount;
    public int accuracyBuffAmount;
    public int critBuffAmount;
    public int defenseBuffAmount;
    public int buffDuration;
    
   
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
