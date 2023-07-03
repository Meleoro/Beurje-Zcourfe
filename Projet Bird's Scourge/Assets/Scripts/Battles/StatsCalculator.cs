using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsCalculator 
{
    public int CalculateDamages(int strength, int strengthMultiplicator, int defense)
    {
        return defense - strength * strengthMultiplicator;
    }
}
