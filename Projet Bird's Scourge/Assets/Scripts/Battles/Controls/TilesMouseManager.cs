using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesMouseManager : MonoBehaviour
{
    [Header("Parameters")] 
    [SerializeField] private Color tilesMovementColor;
    [SerializeField] private Color tilesMovementColorSelected;
    [SerializeField] private Color tilesAttackColor;
    [SerializeField] private Color tilesAttackColorSelected;
    
    [Header("Tiles Lists")]
    [HideInInspector] public List<OverlayTile> tilesCompetenceDisplayed = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesAtRangeDisplayed = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesCompetenceSelected = new List<OverlayTile>();
    [HideInInspector] public OverlayTile currentSelectedTile;

    [Header("Controller Infos")] 
    private bool competenceSelect;
    private bool charaSelect;
    private Unit selectedUnit;
    private Ennemy selectedEnnemy;
    private Unit overlayedUnit;
    private Ennemy overlayedEnnemy;

    [Header("Current Infos")] 
    private bool competenceDisplayed;
    private DataCompetence currentCompetence;
    private int currentCompetenceLevel;

    [Header("References")] 
    private MouseManager mainScript;
    private EffectMaker effectMaker;
    private RangeFinder rangeFinder;


    private void Start()
    {
        mainScript = GetComponent<MouseManager>();
        effectMaker = new EffectMaker();
        rangeFinder = new RangeFinder();
    }

    

    private void GetInfos()
    {
        tilesCompetenceDisplayed = mainScript.tilesCompetenceDisplayed;
        tilesAtRangeDisplayed = mainScript.tilesAtRangeDisplayed;
        currentSelectedTile = mainScript.currentSelectedTile;
        
        
        selectedUnit = mainScript.selectedUnit;
        selectedEnnemy = mainScript.selectedEnnemy;

        overlayedUnit = mainScript.overlayedUnit;
        overlayedEnnemy = mainScript.overlayedEnnemy;

        competenceSelect = mainScript.competenceSelect;
        charaSelect = mainScript.unitSelect;
        
        
        competenceDisplayed = mainScript.competenceDisplayed;
        currentCompetenceLevel = mainScript.competenceLevel;
        currentCompetence = mainScript.competenceUsed;
    }

    private void SetInfos()
    {
        mainScript.tilesCompetenceDisplayed = tilesCompetenceDisplayed;
        mainScript.tilesAtRangeDisplayed = tilesAtRangeDisplayed;
        mainScript.currentSelectedTile = currentSelectedTile;
        
        
        mainScript.selectedUnit = selectedUnit;
        mainScript.selectedEnnemy = selectedEnnemy;

        mainScript.overlayedUnit = overlayedUnit;
        mainScript.overlayedEnnemy = overlayedEnnemy;

        mainScript.competenceSelect = competenceSelect;
        mainScript.unitSelect = charaSelect;
        
        
        mainScript.competenceDisplayed = competenceDisplayed;
    }
    
    
    // MANAGE WHICH COLOR HAS TO HAVE EVERYTILES DEPENDING ON THE SITUATION
    public void ManageOverlayTiles(bool forceReset = false, bool forceChange = false)
    {
        GetInfos();
        
        ResetOverlayTiles(forceReset);
        
        // If a competence is selected
        if (competenceSelect) 
        {
            DisplayTilesCompetence();
        }
        
        // If only the character is selected
        else 
        {
            if(selectedUnit == null && selectedEnnemy == null)
            {
                DisplayTilesAtRange(overlayedUnit, overlayedEnnemy, forceChange);
            }
            else
            {
                DisplayTilesAtRange(selectedUnit, selectedEnnemy, forceChange);
            }
        }
        
        SetInfos();
    }


    private void ResetOverlayTiles(bool forceReset)
    {
        if ((!charaSelect || competenceSelect || forceReset) && !competenceDisplayed)
        {
            for (int i = 0; i < tilesAtRangeDisplayed.Count; i++)
            {
                tilesAtRangeDisplayed[i].ResetTile();
            }

            for (int i = 0; i < tilesCompetenceDisplayed.Count; i++)
            {
                tilesCompetenceDisplayed[i].ResetTile();
            }
        }
    }


    // DISPLAY ALL TILES AT RANGE OF THE SELECTED CHARACTER OR ERASE IF NO CHARACTER IS SELECTED
    private void DisplayTilesAtRange(Unit currentUnit, Ennemy currentEnnemy, bool forceChange)
    {
        if (currentUnit != null)
        {
            if (currentUnit != selectedUnit || forceChange)
            {
                tilesAtRangeDisplayed = currentUnit.currentTilesAtRange;

                if(selectedUnit is null)
                    StartCoroutine(effectMaker.MoveTilesAppear(currentUnit.currentTile, tilesAtRangeDisplayed, 0.05f, tilesMovementColor));
                
                else
                    StartCoroutine(effectMaker.MoveTilesAppear(currentUnit.currentTile, tilesAtRangeDisplayed, 0.05f, tilesMovementColorSelected));
            }
        }
        
        else if(currentEnnemy != null)
        {
            if (currentEnnemy != selectedEnnemy || forceChange)
            {
                tilesAtRangeDisplayed = currentEnnemy.currentMoveTiles;
            
                if(selectedEnnemy is null)
                    StartCoroutine(effectMaker.MoveTilesAppear(currentEnnemy.currentTile, tilesAtRangeDisplayed, 0.05f, tilesMovementColor));
                
                else
                    StartCoroutine(effectMaker.MoveTilesAppear(currentEnnemy.currentTile, tilesAtRangeDisplayed, 0.05f, tilesMovementColorSelected));
            }
        }

        else
        {
            tilesAtRangeDisplayed = new List<OverlayTile>();
        }
    }
    
    
    // DISPLAY ALL TILES AT RANGE OF THE SELECTED CHARACTER OR ERASE IF NO CHARACTER IS SELECTED
    private void DisplayTilesCompetence()
    {
        if (!competenceDisplayed)
        {
            competenceDisplayed = true;
            
            if (selectedUnit is not null)
            {
                StartCoroutine(effectMaker.AttackTilesAppear(selectedUnit.currentTile, tilesCompetenceDisplayed, 0.05f, tilesAttackColor));
            }
            else
            {
                StartCoroutine(effectMaker.AttackTilesAppear(selectedEnnemy.currentTile, tilesCompetenceDisplayed, 0.05f, tilesAttackColor));
            }
        }
    }

    
    public void DisplaySelectedTile(OverlayTile currentTile)
    {
        GetInfos();
        
        if (currentSelectedTile != currentTile)
        {
            if (!currentCompetence.levels[currentCompetenceLevel].isZoneEffect)
            {
                if (currentSelectedTile is not null)
                {
                    currentSelectedTile.DeselectEffect(0.05f, tilesAttackColor);
                }

                if (currentTile is not null && tilesCompetenceDisplayed.Contains(currentTile))
                {
                    currentSelectedTile = currentTile;
                
                    StartCoroutine(currentSelectedTile.SelectEffect(0.05f, tilesAttackColorSelected));
                }
            }

            else if(tilesCompetenceDisplayed.Contains(currentTile))
            {
                ManageSelectedZoneTiles(currentTile);
            }
        }
        
        SetInfos();
    }


    private void ManageSelectedZoneTiles(OverlayTile currentTile)
    {
        List<OverlayTile> currentTilesZone =
            rangeFinder.FindTilesCompetence(currentTile, currentCompetence, currentCompetenceLevel);

        for (int i = 0; i < currentTilesZone.Count; i++)
        {
            if (!tilesCompetenceSelected.Contains(currentTilesZone[i]))
            {
                StartCoroutine(currentTilesZone[i].SelectEffect(0.1f, tilesAttackColorSelected));
                tilesCompetenceSelected.Add(currentTilesZone[i]);
            }
        }
        
        for (int i = tilesCompetenceSelected.Count - 1; i >= 0; i--)
        {
            if (!currentTilesZone.Contains(tilesCompetenceSelected[i]))
            {
                if(tilesCompetenceDisplayed.Contains(tilesCompetenceSelected[i]))
                    tilesCompetenceSelected[i].DeselectEffect(0.1f, tilesAttackColor);

                else
                    tilesCompetenceSelected[i].ResetTile();
                
                
                
                tilesCompetenceSelected.RemoveAt(i);
            }
        }
    }
}
