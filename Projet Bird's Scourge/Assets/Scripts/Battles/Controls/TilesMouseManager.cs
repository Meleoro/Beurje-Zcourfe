using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilesMouseManager : MonoBehaviour
{
    [Header("Parameters")] 
    [SerializeField] private Color tilesMovementColor;
    [SerializeField] private Color tilesMovementColorSelected;
    [SerializeField] private Color tilesAttackColor;
    [SerializeField] private Color tilesAttackColorSelected;
    [SerializeField] private Color tilesBuffColor;
    [SerializeField] private Color tilesBuffColorSelected;
    
    [Header("Tiles Lists")]
    [HideInInspector] public List<OverlayTile> tilesCompetenceDisplayed = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesAtRangeDisplayed = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesCompetenceSelected = new List<OverlayTile>();
    [HideInInspector] public OverlayTile currentSelectedTile;

    [Header("Characters")]
    private List<Ennemy> selectedEnnemies = new List<Ennemy>();
    private List<Ennemy> selectedSummons = new List<Ennemy>();
    private List<Unit> selectedUnits = new List<Unit>();
    
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
        tilesCompetenceSelected = mainScript.tilesSelectedZone;
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
        mainScript.tilesSelectedZone = tilesCompetenceSelected;
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

            if (!mainScript.kindCompetence)
            {
                if (selectedUnit is not null)
                {
                    StartCoroutine(effectMaker.AttackTilesAppear(selectedUnit.currentTile, tilesCompetenceDisplayed, 0.1f, tilesAttackColor));
                }
                else
                {
                    StartCoroutine(effectMaker.AttackTilesAppear(selectedEnnemy.currentTile, tilesCompetenceDisplayed, 0.1f, tilesAttackColor));
                }
            }

            else
            {
                if (selectedUnit is not null)
                {
                    StartCoroutine(effectMaker.AttackTilesAppear(selectedUnit.currentTile, tilesCompetenceDisplayed, 0.1f, tilesBuffColor));
                }
                else
                {
                    StartCoroutine(effectMaker.AttackTilesAppear(selectedEnnemy.currentTile, tilesCompetenceDisplayed, 0.1f, tilesBuffColor));
                }
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
                currentSelectedTile = currentTile;
                ManageSelectedZoneTiles(currentTile);
            }

            else
            {
                currentSelectedTile = currentTile;

                for(int i  = tilesCompetenceSelected.Count - 1; i >= 0; i--)
                {
                    if (tilesCompetenceDisplayed.Contains(tilesCompetenceSelected[i]))
                    {
                        if(!mainScript.kindCompetence) 
                            tilesCompetenceSelected[i].DeselectEffect(0.1f, tilesAttackColor);
                    
                        else
                            tilesCompetenceSelected[i].DeselectEffect(0.1f, tilesBuffColor);
                    }
                    else
                    {
                        tilesCompetenceSelected[i].ResetTile();
                    }
                        
                    tilesCompetenceSelected.RemoveAt(i);
                }
                
                ManageSelectedCharacters();
            }
        }
        
        SetInfos();
    }


    private void ManageSelectedZoneTiles(OverlayTile currentTile)
    {
        List<OverlayTile> currentTilesZone =
            rangeFinder.FindTilesCompetence(currentTile, currentCompetence, currentCompetenceLevel, true);

        for (int i = 0; i < currentTilesZone.Count; i++)
        {
            if (!tilesCompetenceSelected.Contains(currentTilesZone[i]))
            {
                if(!mainScript.kindCompetence)
                    StartCoroutine(currentTilesZone[i].SelectEffect(0.1f, tilesAttackColorSelected));
                
                else
                    StartCoroutine(currentTilesZone[i].SelectEffect(0.1f, tilesBuffColorSelected));
                
                
                tilesCompetenceSelected.Add(currentTilesZone[i]);
            }
        }
        
        for (int i = tilesCompetenceSelected.Count - 1; i >= 0; i--)
        {
            if (!currentTilesZone.Contains(tilesCompetenceSelected[i]))
            {
                if (tilesCompetenceDisplayed.Contains(tilesCompetenceSelected[i]))
                {
                    if(!mainScript.kindCompetence) 
                        tilesCompetenceSelected[i].DeselectEffect(0.1f, tilesAttackColor);
                    
                    else
                        tilesCompetenceSelected[i].DeselectEffect(0.1f, tilesBuffColor);
                }

                else
                    tilesCompetenceSelected[i].ResetTile();
                
                tilesCompetenceSelected.RemoveAt(i);
            }
        }

        ManageSelectedCharacters();
    }

    
    private void ManageSelectedCharacters()
    {
        List<Vector2Int> keysUnits = BattleManager.Instance.activeUnits.Keys.ToList();
        List<Vector2Int> keysEnnemies = BattleManager.Instance.activeEnnemies.Keys.ToList();
        List<Vector2Int> keysSummons = BattleManager.Instance.activeSummons.Keys.ToList();

        List<Unit> currentSelectedUnits = new List<Unit>();
        List<Ennemy> currentSelectedEnnemies = new List<Ennemy>();
        List<Ennemy> currentSelectedSummons = new List<Ennemy>();
        
        for (int i = 0; i < tilesCompetenceSelected.Count; i++)
        {
            if (keysUnits.Contains((Vector2Int)tilesCompetenceSelected[i].posOverlayTile) && mainScript.kindCompetence)
            {
                currentSelectedUnits.Add(BattleManager.Instance.activeUnits[(Vector2Int)tilesCompetenceSelected[i].posOverlayTile]);
            }
            
            else if (keysEnnemies.Contains((Vector2Int)tilesCompetenceSelected[i].posOverlayTile) && !mainScript.kindCompetence)
            {
                currentSelectedEnnemies.Add(BattleManager.Instance.activeEnnemies[(Vector2Int)tilesCompetenceSelected[i].posOverlayTile]);
            }
            
            else if (keysSummons.Contains((Vector2Int)tilesCompetenceSelected[i].posOverlayTile) && mainScript.kindCompetence)
            {
                currentSelectedSummons.Add(BattleManager.Instance.activeSummons[(Vector2Int)tilesCompetenceSelected[i].posOverlayTile]);
            }
        }
        
        
        // WE REMOVE ALL THE CHARACTERS WHO QUIT THE SELECTION
        for (int i = selectedUnits.Count - 1; i >= 0; i--)
        {
            if (!currentSelectedUnits.Contains(selectedUnits[i]))
            {
                selectedUnits[i].DesactivateOutline();
                selectedUnits.RemoveAt(i);
            }
        }
        
        for (int i = selectedEnnemies.Count - 1; i >= 0; i--)
        {
            if (!currentSelectedEnnemies.Contains(selectedEnnemies[i]))
            {
                selectedEnnemies[i].DesactivateOutline();
                selectedEnnemies.RemoveAt(i);
            }
        }
        
        for (int i = selectedSummons.Count - 1; i >= 0; i--)
        {
            if (!currentSelectedSummons.Contains(selectedSummons[i]))
            {
                selectedSummons[i].DesactivateOutline();
                selectedSummons.RemoveAt(i);
            }
        }

        
        // WE ADD THE NEW CHARACTERS IN THE SELECTION
        for (int i = currentSelectedUnits.Count - 1; i >= 0; i--)
        {
            if (!selectedUnits.Contains(currentSelectedUnits[i]))
            {
                currentSelectedUnits[i].ActivateOutline(Color.green);
                selectedUnits.Add(currentSelectedUnits[i]);
            }
        }
        
        for (int i = currentSelectedEnnemies.Count - 1; i >= 0; i--)
        {
            if (!selectedEnnemies.Contains(currentSelectedEnnemies[i]))
            {
                currentSelectedEnnemies[i].ActivateOutline(Color.red);
                selectedEnnemies.Add(currentSelectedEnnemies[i]);
            }
        }
        
        for (int i = currentSelectedSummons.Count - 1; i >= 0; i--)
        {
            if (!selectedSummons.Contains(currentSelectedSummons[i]))
            {
                currentSelectedSummons[i].ActivateOutline(Color.green);
                selectedSummons.Add(currentSelectedSummons[i]);
            }
        }
    }
    
}
