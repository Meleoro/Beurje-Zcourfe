using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsCalculator 
{
    public int CalculateDamages(int strength, float strengthMultiplicator, int defense)
    {
        return Mathf.RoundToInt((strength * strengthMultiplicator) - defense);
    }

    
    public int CalculateHitRate(int AGIUnit, float skillBaseHitRate, int AGIEnnemi)
    {
        return Mathf.RoundToInt(skillBaseHitRate + AGIUnit - AGIEnnemi);
    }
    
    public int CalculateCriticalRate(int CHAUnit, float critMultiplicator, int CHAEnnemi)
    {
        return Mathf.RoundToInt(((CHAUnit - CHAEnnemi) * 5) * critMultiplicator);
    }
}
