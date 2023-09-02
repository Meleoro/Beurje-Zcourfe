using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMapControler : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            VerifyClickedElement();
        }
    }


    private void VerifyClickedElement()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.CompareTag("Zone"))
            {
                GlobalMapManager.Instance.LaunchAventure(hits[i].collider.GetComponent<ZoneMap>().zoneData);

                break;
            }

            if (hits[i].collider.CompareTag("Region"))
            {
                StartCoroutine(GlobalMapManager.Instance.EnterRegion(hits[i].collider.GetComponent<RegionMap>().regionObject));

                break;
            }
        }
    }
    
    
    
}
