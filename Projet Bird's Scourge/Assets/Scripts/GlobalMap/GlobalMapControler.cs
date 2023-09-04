using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMapControler : MonoBehaviour
{
    [Header("CurrentInfos")] 
    public bool noControl;
    public SpriteRenderer currentOverlayedElement;
    public ZoneMap currentOverlayedElementScript;
    public SpriteRenderer currentClickedElement;
    
    [Header("Colors")] 
    public Color baseColor;
    public Color overlayColor;
    public Color clickColor;
    public Color lockBaseColor;
    public Color lockOverlayColor;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !noControl)
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
                ManageColorsZone(hits[i].collider.GetComponent<SpriteRenderer>(), null);

                break;
            }

            if (hits[i].collider.CompareTag("Region"))
            {
                ManageColorsRegions(hits[i].collider.GetComponent<SpriteRenderer>(), null);

                break;
            }
        }
        
        if(hits.Length == 0)
            ManageColorsZone(null, null);
    }
    

    private void VerifyClickedElement()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.CompareTag("Zone"))
            {
                ZoneMap currentZone = hits[i].collider.GetComponent<ZoneMap>();
                
                StartCoroutine(GameManager.Instance.EnterAventure(currentZone.zoneData, currentZone.regionIndex, currentZone.zoneIndex));
                ManageColorsZone(null, hits[i].collider.GetComponent<SpriteRenderer>());

                break;
            }

            if (hits[i].collider.CompareTag("Region"))
            {
                RegionMap currentScript = hits[i].collider.GetComponent<RegionMap>();
                
                StartCoroutine(GlobalMapManager.Instance.EnterRegion(currentScript.regionObject, currentScript.newPosCam, currentScript.newSizeCam));
                ManageColorsRegions(null, hits[i].collider.GetComponent<SpriteRenderer>());

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



    private void ManageColorsZone(SpriteRenderer overlayedZone, SpriteRenderer clickedZone)
    {
        if (overlayedZone != null)
        {
            if (currentOverlayedElement != null && currentOverlayedElementScript != null)
            {
                ZoneMap currentScript = overlayedZone.GetComponent<ZoneMap>();

                if (currentOverlayedElementScript.isLocked)
                {
                    currentOverlayedElement.color = lockBaseColor;
                }
                else
                {
                    currentOverlayedElement.color = baseColor;
                }

                currentOverlayedElementScript = currentScript;
                currentOverlayedElement = overlayedZone;
                
                if (currentScript.isLocked)
                {
                    currentOverlayedElement.color = lockOverlayColor;
                }
                else
                {
                    currentOverlayedElement.color = overlayColor;
                }
            }

            else
            {
                currentOverlayedElement = overlayedZone;
                currentOverlayedElementScript = overlayedZone.GetComponent<ZoneMap>();;
                
                if (currentOverlayedElementScript.isLocked)
                {
                    currentOverlayedElement.color = lockOverlayColor;
                }
                else
                {
                    currentOverlayedElement.color = overlayColor;
                }
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
                currentOverlayedElementScript = null;
                currentOverlayedElement = null;
            }

            if (currentClickedElement != null)
            {
                currentClickedElement.color = baseColor;
                currentOverlayedElementScript = null;
                currentClickedElement = null;
            }
        }
    }
    
    private void ManageColorsRegions(SpriteRenderer overlayedZone, SpriteRenderer clickedZone)
    {
        if (overlayedZone != null)
        {
            if (currentOverlayedElement != null)
            {
                currentOverlayedElement.color = baseColor;

                currentOverlayedElement = overlayedZone;
                
                currentOverlayedElement.color = overlayColor;
            }

            else
            {
                currentOverlayedElement = overlayedZone;

                currentOverlayedElement.color = overlayColor;
                
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
