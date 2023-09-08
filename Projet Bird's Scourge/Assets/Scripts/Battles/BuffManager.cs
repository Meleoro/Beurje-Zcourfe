using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager Instance;

    public enum BuffType
    {
        damage,
        accuracy,
        crit,
        defense
    }

    [Header("Lists")] 
    [SerializeField] public List<UnitBuff> characters = new List<UnitBuff>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);
    }



    public void InitialiseListCharacters()
    {
        characters = new List<UnitBuff>();

        for (int i = 0; i < BattleManager.Instance.currentUnits.Count; i++)
        {
            characters.Add(new UnitBuff(BattleManager.Instance.currentUnits[i], null));
        }
        
        for (int i = 0; i < BattleManager.Instance.currentEnnemies.Count; i++)
        {
            characters.Add(new UnitBuff(null, BattleManager.Instance.currentEnnemies[i]));
        }
    }
    
    
    
    
    // -------------------------- APPLY BUFFS ------------------------------

    // RETURNS DAMAGE BUFF
    public int GetDamageBuff(int currentDamages, Unit currentUnit, Ennemy currentEnnemy)
    {
        int addedDamages = 0;
        
        for (int i = 0; i < characters.Count; i++)
        {
            bool isOkay = false;
            
            if (characters[i].unit == currentUnit && currentUnit != null)
                isOkay = true;
            
            else if (characters[i].ennemy == currentEnnemy && currentEnnemy != null)
                isOkay = true;
            
            
            if (isOkay)
            {
                // We go through every of the unit's buffs to check if the unit has a damage buff
                for (int j = 0; j < characters[i].currentBuffs.Count; j++)
                {
                    if (characters[i].currentBuffs[j].buffType == BuffType.damage)
                    {
                        addedDamages += (int)((characters[i].currentBuffs[j].buffValue / 100f) * currentDamages);
                    }
                }
            }
        }

        return addedDamages;
    }
    
    
    // RETURNS ACCURACY BUFF
    public int GetAccuracyBuff(int currentAccuracyPercentage, Unit currentUnit, Ennemy currentEnnemy)
    {
        int addedAccuracyPercentage = 0;
        
        for (int i = 0; i < characters.Count; i++)
        {
            bool isOkay = false;
            
            if (characters[i].unit == currentUnit && currentUnit != null)
                isOkay = true;
            
            else if (characters[i].ennemy == currentEnnemy && currentEnnemy != null)
                isOkay = true;

            
            if (isOkay)
            {
                // We go through every of the unit's buffs to check if the unit has a damage buff
                for (int j = 0; j < characters[i].currentBuffs.Count; j++)
                {
                    if (characters[i].currentBuffs[j].buffType == BuffType.accuracy)
                    {
                        addedAccuracyPercentage += (int)((characters[i].currentBuffs[j].buffValue / 100f) * currentAccuracyPercentage);
                    }
                }
            }
        }

        return addedAccuracyPercentage;
    }
    
    
    // RETURNS CRIT BUFF
    public int GetCritBuff(int currentCritPercentage, Unit currentUnit, Ennemy currentEnnemy)
    {
        int addedCritPercentage = 0;
        
        for (int i = 0; i < characters.Count; i++)
        {
            bool isOkay = false;
            
            if (characters[i].unit == currentUnit && currentUnit != null)
                isOkay = true;
            
            else if (characters[i].ennemy == currentEnnemy && currentEnnemy != null)
                isOkay = true;

            
            if (isOkay)
            {
                // We go through every of the unit's buffs to check if the unit has a damage buff
                for (int j = 0; j < characters[i].currentBuffs.Count; j++)
                {
                    if (characters[i].currentBuffs[j].buffType == BuffType.crit)
                    {
                        addedCritPercentage += (int)((characters[i].currentBuffs[j].buffValue / 100f) * currentCritPercentage);
                    }
                }
            }
        }

        return addedCritPercentage;
    }
    
    
    // RETURNS DEFENSE BUFF
    public int GetDefenseBuff(int currentDefense, Unit currentUnit, Ennemy currentEnnemy)
    {
        int addedDefense = 0;
        
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].unit == currentUnit)
            {
                bool isOkay = false;
            
                if (characters[i].unit == currentUnit && currentUnit != null)
                    isOkay = true;
            
                else if (characters[i].ennemy == currentEnnemy && currentEnnemy != null)
                    isOkay = true;

                
                if (isOkay)
                {
                    for (int j = 0; j < characters[i].currentBuffs.Count; j++)
                    {
                        if (characters[i].currentBuffs[j].buffType == BuffType.defense)
                        {
                            addedDefense += (int)((characters[i].currentBuffs[j].buffValue / 100f) * currentDefense);
                        }
                    }
                }
            }
        }

        return addedDefense;
    }
    
    
    
    
    // ------------------------- MANAGE BUFFS -----------------------------

    // CALLED TO ADD A BUFF TO A LIST OF UNITS
    public void AddBuff(BuffType buffType, int buffValue, int buffDuration, List<Unit> concernedUnit, List<Ennemy> concernedEnnemies)
    {
        if (concernedUnit is not null)
        {
            for (int i = 0; i < concernedUnit.Count; i++)
            {
                for (int j = 0; j < characters.Count; j++)
                {
                    if (characters[j].unit == concernedUnit[i])
                    {
                        Debug.Log("Je gagne le buff");
                        characters[j].currentBuffs.Add(new Buff(buffType, buffValue, buffDuration));
                    }
                }
            }
        }
        
        else if (concernedEnnemies is not null)
        {
            for (int i = 0; i < concernedEnnemies.Count; i++)
            {
                for (int j = 0; j < characters.Count; j++)
                {
                    if (characters[j].ennemy == concernedEnnemies[i])
                    {
                        characters[j].currentBuffs.Add(new Buff(buffType, buffValue, buffDuration));
                    }
                }
            }
        }
    }

    // CALLED TO ACTUALISE THE DURATION OF EVERY BUFFS
    public void NextTurn()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            for (int j = characters[i].currentBuffs.Count - 1; j <= 0; j--)
            {
                characters[i].currentBuffs[j].buffDuration -= 1;

                if (characters[i].currentBuffs[j].buffDuration <= 0)
                {
                    characters[i].currentBuffs.RemoveAt(j);
                }
            }
        }
    }
}





[Serializable]
public class UnitBuff
{
    public Unit unit;
    public Ennemy ennemy;
    
    public List<Buff> currentBuffs = new List<Buff>();
    
    
    public UnitBuff(Unit currentUnit, Ennemy currentEnnemy)
    {
        unit = currentUnit;
        ennemy = currentEnnemy;
    }
}


[Serializable]
public class Buff
{
    public BuffManager.BuffType buffType;
    public int buffValue;    // EST EN POURCETAGE ATTENTION
    public int buffDuration;

    public Buff(BuffManager.BuffType currentBuffType, int currentBuffValue, int currentBuffDuration)
    {
        buffType = currentBuffType;
        buffValue = currentBuffValue;
        buffDuration = currentBuffDuration;
    }
}
