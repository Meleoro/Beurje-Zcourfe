using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneMap : MonoBehaviour
{
    public AventureData zoneData;

    [Range(0, 3)] public int regionIndex;
    [Range(0, 3)] public int zoneIndex;

    public int currentProgression;
    public ZoneMap lockingZone;
    public int lockingZoneProgression;
    public bool isLocked;
    public GameObject lockImage;


    
    public void VerifyLock()
    {
        if (lockingZone != null)
        {
            if (lockingZone.currentProgression < lockingZoneProgression)
            {
                isLocked = true;
                lockImage.SetActive(true);
                
                GetComponent<SpriteRenderer>().color = GlobalMapManager.Instance.scriptController.lockBaseColor;
            }
            else
            {
                isLocked = false;
                lockImage.SetActive(false);
                
                GetComponent<SpriteRenderer>().color = GlobalMapManager.Instance.scriptController.baseColor;
            }
        }
        else
        {
            isLocked = false;
            lockImage.SetActive(false);
            
            GetComponent<SpriteRenderer>().color = GlobalMapManager.Instance.scriptController.baseColor;
        }
    }
}
