using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMapControler : MonoBehaviour
{
    [Header("CurrentInfos")] 
    public SpriteRenderer currentOverlayedElement;
    public SpriteRenderer currentClickedElement;
    
    [Header("Colors")] 
    public Color baseColor;
    public Color overlayColor;
    public Color clickColor;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            VerifyClickedElement();
        }
        else
        {
            VerifyOverlayedElement();
        }
    }

    
    
    private void VerifyOverlayedElement()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.CompareTag("Zone"))
            {
                ManageColors(hits[i].collider.GetComponent<SpriteRenderer>(), null);

                break;
            }

            if (hits[i].collider.CompareTag("Region"))
            {
                ManageColors(hits[i].collider.GetComponent<SpriteRenderer>(), null);

                break;
            }
        }
        
        if(hits.Length == 0)
            ManageColors(null, null);
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
                ManageColors(null, hits[i].collider.GetComponent<SpriteRenderer>());

                break;
            }

            if (hits[i].collider.CompareTag("Region"))
            {
                RegionMap currentScript = hits[i].collider.GetComponent<RegionMap>();
                
                StartCoroutine(GlobalMapManager.Instance.EnterRegion(currentScript.regionObject, currentScript.newPosCam, currentScript.newSizeCam));
                ManageColors(null, hits[i].collider.GetComponent<SpriteRenderer>());

                break;
            }
        }

        if (hits.Length == 0)
        {
            if (!GlobalMapManager.Instance.currentRegion.isGlobal)
            {
                StartCoroutine(GlobalMapManager.Instance.QuitRegion());
            }
        }
    }



    private void ManageColors(SpriteRenderer overlayedZone, SpriteRenderer clickedZone)
    {
        if (overlayedZone != null)
        {
            if (currentOverlayedElement != null)
            {
                currentOverlayedElement.color = baseColor;
                
                overlayedZone.color = overlayColor;
                currentOverlayedElement = overlayedZone;
            }

            else
            {
                currentOverlayedElement = overlayedZone;
                currentOverlayedElement.color = baseColor;
            }
        }
        
        else if (clickedZone != null)
        {
            if (currentClickedElement != null)
            {
                currentClickedElement.color = baseColor;
                
                clickedZone.color = clickColor; 
                currentClickedElement = clickedZone; 
            }

            else
            {
                currentClickedElement = clickedZone;
                currentClickedElement.color = baseColor;
            }
        }

        else
        {
            if (currentOverlayedElement != null)
            {
                currentOverlayedElement.color = baseColor;
                currentOverlayedElement = null;
            }

            if (currentClickedElement != null)
            {
                currentClickedElement.color = baseColor;
                currentClickedElement = null;
            }
        }
    }
}
