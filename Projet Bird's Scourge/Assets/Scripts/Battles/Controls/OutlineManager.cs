using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineManage : MonoBehaviour
{
    [Header("Outlines Colors")] 
    public Color colorOutlineSelectedUnit;
    public Color colorOutlineSelectedEnnemy;

    [Header("Current Infos")] 
    public Unit overlayedUnit;
    public Unit selectedUnit;
    public Ennemy overlayedEnnemy;
    public Ennemy selectedEnnemy;

    [Header("References")] 
    private MouseManager originalScript;

    
    private void Start()
    {
        originalScript = GetComponent<MouseManager>();
    }



    public void ActualiseVariables()
    {
        overlayedUnit = originalScript.overlayedUnit;
        overlayedEnnemy = originalScript.overlayedEnnemy;

        selectedUnit = originalScript.selectedUnit;
        selectedEnnemy = originalScript.selectedEnnemy;
    }

    public void SendVariables()
    {
        originalScript.overlayedUnit = overlayedUnit;
        originalScript.overlayedEnnemy = overlayedEnnemy;

        originalScript.selectedUnit = selectedUnit;
        originalScript.selectedEnnemy = selectedEnnemy;
    }

    

    // MANAGES THE OUTLINES OF THE ELEMENT OVERLAYED BY THE MOUSE
    public void ManageOverlayElement(Unit currentOverlayedUnit, Ennemy currentOverlayedEnnemy)
    {
        ActualiseVariables();
        
        // If nothing is selected
        if (currentOverlayedUnit is null && currentOverlayedEnnemy is null)
        {
            if (overlayedEnnemy != null)
            {
                if (overlayedEnnemy != selectedEnnemy || selectedEnnemy == null)
                {
                    overlayedEnnemy.DesactivateOutline();
                }
                
                overlayedEnnemy = null;
            }

            else if (overlayedUnit != null)
            {
                if (overlayedUnit != selectedUnit || selectedUnit == null)
                {
                    overlayedUnit.DesactivateOutline();
                }
                
                overlayedUnit = null;
            }
             
            SendVariables();
            originalScript.ManageOverlayTiles();
        }

        // If it's an unit
        else if (currentOverlayedUnit is not null)
        {
            if (currentOverlayedUnit != overlayedUnit)
            {
                currentOverlayedUnit.ActivateOutline(colorOutlineSelectedUnit);

                currentOverlayedUnit.FindTilesAtRange();
                overlayedUnit = currentOverlayedUnit;
                    
                SendVariables();
                originalScript.ManageOverlayTiles();
            }
        }

        // If it's an ennemy
        else
        {
            if (currentOverlayedEnnemy != overlayedEnnemy)
            {
                currentOverlayedEnnemy.ActivateOutline(colorOutlineSelectedEnnemy);

                currentOverlayedEnnemy.FindTilesAtRange();
                overlayedEnnemy = currentOverlayedEnnemy;

                SendVariables();
                originalScript.ManageOverlayTiles();
            }
        }
    }

    
    // MANAGES THE OUTLINES OF THE CLICKED ELEMENT
    public void ManageClickedElement(Unit currentClickedUnit, Ennemy currentClickedEnnemy)
    {
        ActualiseVariables();
        
        // UNIT PART
        if(currentClickedUnit != null)
        {
            if (currentClickedUnit != selectedUnit && selectedUnit != null)
            {
                selectedUnit.DesactivateOutline();
                    
                selectedUnit = currentClickedUnit;
                selectedEnnemy = null;

                SendVariables();
                originalScript.ManageOverlayTiles(true, true);
            }
                
            // If it's an automatic selection
            /*else if (currentUnit != currentOverlayedUnit)
            {
                selectedUnit = currentUnit;
                selectedEnnemy = null;
                    
                ManageOverlayTiles(true, true);

                currentOverlayedUnit = currentUnit;
            }*/

            else
            {
                selectedUnit = currentClickedUnit;
                selectedEnnemy = null;
                    
                SendVariables();
                originalScript.ManageOverlayTiles(false, true);
            }
                
            selectedUnit.ActivateOutline(colorOutlineSelectedUnit);
        }

        // ENNEMY PART
        else if(currentClickedEnnemy != null)
        {
            if (selectedEnnemy != currentClickedEnnemy && selectedEnnemy != null)
            {
                selectedEnnemy.DesactivateOutline();
            }

            currentClickedEnnemy.ActivateOutline(colorOutlineSelectedEnnemy);

            selectedEnnemy = currentClickedEnnemy;
            selectedUnit = null;
        }

        else
        {
            if (selectedUnit != null)
            {
                selectedUnit.DesactivateOutline();
                selectedUnit.DeselectUnit();
            }

            else if (selectedEnnemy != null)
            {
                selectedEnnemy.DesactivateOutline();
            }

            selectedUnit = null;
            selectedEnnemy = null;
        }
        
        SendVariables();
    }
    
}
