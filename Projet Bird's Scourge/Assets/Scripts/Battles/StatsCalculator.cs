using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsCalculator 
{
    public int CalculateDamages(int strength, float strengthMultiplicator, int defense)
    {
        return Mathf.RoundToInt(defense - strength * strengthMultiplicator);
    }
}
