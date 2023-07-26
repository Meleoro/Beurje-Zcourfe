using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsCalculator 
{
    public int CalculateDamages(int strength, float strengthMultiplicator, int defense)
    {
        return Mathf.Clamp(Mathf.RoundToInt((strength * strengthMultiplicator) - defense), 0, 10000);
    }

    
    public int CalculateHitRate(int AGIUnit, float skillBaseHitRate, int AGIEnnemi)
    {
        return Mathf.Clamp(Mathf.RoundToInt(skillBaseHitRate + AGIUnit - AGIEnnemi),0,200);
    }
    
    public int CalculateCriticalRate(int CHAUnit, float critMultiplicator, int CHAEnnemi)
    {
        return  Mathf.Clamp(Mathf.RoundToInt(((CHAUnit - CHAEnnemi) * 5) * critMultiplicator),0,100);
    }
}
