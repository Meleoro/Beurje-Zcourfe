using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenedictionManager : MonoBehaviour
{
    [Header("Additional PM")] 
    public bool additionalPM;

    public void GainAdditionalPM()
    {
        additionalPM = true;
    }

    public bool VerifyAdditionalPM()
    {
        if (additionalPM)
        {
            return true;
        }

        return false;
    }
}
