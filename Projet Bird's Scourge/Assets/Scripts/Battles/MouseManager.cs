using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    [Header("Instance")] 
    private static MouseManager _instance;
    public static MouseManager Instance
    {
        get { return _instance; }
    }
    
    [Header("OverlayTiles")]
    private List<OverlayTile> tilesCompetenceDisplayed = new List<OverlayTile>();
    private List<OverlayTile> tilesAtRangeDisplayed = new List<OverlayTile>();
    private int indexCompetence;
    private int competenceLevel;
    [HideInInspector] public DataCompetence competenceUsed;
    private bool competenceSelect;
    private bool unitSelect;

    [Header("OverlayUnit")]
    private Unit currentOverlayedUnit; 
    private Ennemy currentOverlayedEnnemy;

    [Header("Other")]
    [HideInInspector] public Unit selectedUnit;
    [HideInInspector] public bool noControl;
    private List<OverlayTile> currentPath = new List<OverlayTile>();

    [Header("References")]
    private PathFinder pathFinder;
    private ArrowCreator arrowCreator;


    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        
        else
            Destroy(_instance);
    }


    private void Start()
    {
        pathFinder = new PathFinder();
        arrowCreator = new ArrowCreator();
    }


    void Update()
    {
        if (!noControl)
        {
            OverlayTile currentTile = GetFocusedTile();

            if(currentTile is not null)
                transform.position = currentTile.transform.position;

            if (unitSelect)
            {
                if (currentTile is not null)
                {
                    DisplayArrow(currentTile);
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                VerifyClickedElement();

                ManageOverlayTiles();
            }
        }
    }


    // SEEK ON WHICH ELEMENT THE CHARACTER CLICKED
    private void VerifyClickedElement()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        for (int i = 0; i < hits.Length; i++)
        {
            GameObject clickedObject = hits[i].collider.gameObject;

            if (clickedObject.CompareTag("Unit"))
            {
                selectedUnit = clickedObject.GetComponent<Unit>();
                selectedUnit.ActivateOutline();
                
                UIBattleManager.Instance.OpenUnitInfos(selectedUnit.data, selectedUnit,null);
                UIBattleManager.Instance.UpdateTurnUISelectedUnit(selectedUnit);

                competenceSelect = false;
                unitSelect = true;
                
                break;
            }
            
            else if (clickedObject.CompareTag("Ennemy"))
            {
                if (competenceSelect)
                {
                    StartCoroutine(selectedUnit.AttackEnnemies(clickedObject.GetComponent<Ennemy>(), tilesCompetenceDisplayed, competenceUsed, competenceLevel));
                    
                    break;
                }
            }

            else if(unitSelect && clickedObject.CompareTag("Tile") && !competenceSelect)
            {
                if (tilesAtRangeDisplayed.Contains(clickedObject.GetComponent<OverlayTile>()))
                {
                    StartCoroutine(selectedUnit.MoveToTile(currentPath));
                    
                    break;
                }
                else
                {
                    StopSelection();
                }
            }
        }

        //StopSelection();
    }

    
    // WHEN THE PLAYER DESELECT A CHARACTER
    private void StopSelection()
    {
        competenceSelect = false;
        unitSelect = false;
        selectedUnit = null;
        UIBattleManager.Instance.UpdateTurnUISelectedUnit(selectedUnit);
        ManageOverlayTiles();
    }
    

    // SEEK ON WHICH TILE IS THE MOUSE
    private OverlayTile GetFocusedTile()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.CompareTag("Ennemy"))
            {
                Ennemy currentEnnemy = hits[i].collider.GetComponent<Ennemy>();
                ManageOverlayUnit(null, currentEnnemy);

                if (competenceSelect)
                {
                    selectedUnit.DisplayBattleStats(currentEnnemy, competenceUsed, competenceLevel);
                }
                else
                {
                    UIBattleManager.Instance.OpenUnitInfos(currentEnnemy.data,null, currentEnnemy);
                }

                return currentEnnemy.currentTile;
            }

            else if (hits[i].collider.gameObject.CompareTag("Unit"))
            {
                Unit currentUnit = hits[i].collider.GetComponent<Unit>();
                ManageOverlayUnit(currentUnit, null);

                UIBattleManager.Instance.OpenUnitInfos(currentUnit.data, currentUnit, null);

                return currentUnit.currentTile;
            }

            else if (BattleManager.Instance.order[0].CompareTag("Unit"))
            {
                UIBattleManager.Instance.OpenUnitInfos(BattleManager.Instance.order[0].GetComponent<Unit>().data,
                    BattleManager.Instance.order[0].GetComponent<Unit>(), null);
            }

            else if (BattleManager.Instance.order[0].CompareTag("Ennemy"))
            {
                UIBattleManager.Instance.OpenUnitInfos(BattleManager.Instance.order[0].GetComponent<Ennemy>().data,
                    null, BattleManager.Instance.order[0].GetComponent<Ennemy>());
            }

            if (hits[i].collider.gameObject.CompareTag("Tile"))
            {
                ManageOverlayUnit(null, null);

                UIBattleManager.Instance.CloseAttackPreview();
                
                return hits[i].collider.gameObject.GetComponent<OverlayTile>();
            }
        }

        ManageOverlayUnit(null, null);

        return null;
    }


    // MANAGES WHAT HAS TO BE OUTLINED 
    public void ManageOverlayUnit(Unit currentUnit, Ennemy currentEnnemy)
    {
        // If nothing is selected
        if(currentUnit == null && currentEnnemy == null)
        {
            if (currentOverlayedEnnemy != null)
            {
                currentOverlayedEnnemy.DesactivateOutline();
                currentOverlayedEnnemy = null;
            }
                
            else if(currentOverlayedUnit != null)
            {
                currentOverlayedUnit.DesactivateOutline();
                currentOverlayedUnit = null;
            }
        }

        // If it's an unit
        if(currentEnnemy == null)
        {
            if(currentUnit != currentOverlayedUnit)
            {
                currentOverlayedUnit = currentUnit;
                currentOverlayedUnit.ActivateOutline();
            }
        }

        // If it's an ennemy
        else
        {
            if (currentEnnemy != currentOverlayedEnnemy)
            {
                currentOverlayedEnnemy = currentEnnemy;
                currentOverlayedEnnemy.ActivateOutline();
            }
        }
    }
    
   
    
    // DISPLAY THE ARROW OF THE PATH THAT WILL USE THE UNIT
    private void DisplayArrow(OverlayTile focusedTile)
    {
        if (unitSelect && !competenceSelect)
        {
            if (tilesAtRangeDisplayed.Contains(focusedTile))
            {
                for (int i = 0; i < currentPath.Count; i++)
                {
                    currentPath[i].HideArrow();
                }
                
                currentPath = pathFinder.FindPath(selectedUnit.currentTile, focusedTile);

                for (int i = 0; i < currentPath.Count; i++)
                {
                    OverlayTile previousTile = i > 0 ? currentPath[i - 1] : selectedUnit.currentTile;
                    OverlayTile nextTile = i < currentPath.Count - 1 ? currentPath[i + 1] : null;
                    
                    currentPath[i].DisplayArrow(arrowCreator.CreateArrow(previousTile, currentPath[i], nextTile));
                }
            }
        }
    }

    
    

    // MANAGE WHICH COLOR HAS TO HAVE EVERYTILES DEPENDING ON THE SITUATION
    public void ManageOverlayTiles()
    {
        for (int i = 0; i < tilesAtRangeDisplayed.Count; i++)
        {
            tilesAtRangeDisplayed[i].HideTile();
        }

        for (int i = 0; i < tilesCompetenceDisplayed.Count; i++)
        {
            tilesCompetenceDisplayed[i].HideTile();
        }

        
        // If a competence is selected
        if (competenceSelect) 
        {
            DisplayTilesCompetence(indexCompetence);
        }
        
        // If only the character is selected
        else 
        {
            DisplayTilesAtRange();
        }
    }
    
    
    // DISPLAY ALL TILES AT RANGE OF THE SELECTED CHARACTER OR ERASE IF NO CHARACTER IS SELECTED
    private void DisplayTilesAtRange()
    {
        if (unitSelect)
        {
            tilesAtRangeDisplayed = selectedUnit.currentTilesAtRange;

            for (int i = 0; i < tilesAtRangeDisplayed.Count; i++)
            {
                tilesAtRangeDisplayed[i].ShowTile();
            }
        }
        else
        {
            tilesAtRangeDisplayed = new List<OverlayTile>();
        }
    }
    
    
    // DISPLAY ALL TILES AT RANGE OF THE SELECTED CHARACTER OR ERASE IF NO CHARACTER IS SELECTED
    private void DisplayTilesCompetence(int index)
    {
        for (int i = 0; i < tilesCompetenceDisplayed.Count; i++)
        {
            tilesCompetenceDisplayed[i].ShowTile();
        }
    }
    
    
    // DISPLAY ALL TILES AT RANGE OF THE SELECTED CHARACTER OR ERASE IF NO CHARACTER IS SELECTED
    public void ChangeSelectedCompetence(int index)
    {
        if (index == 0)
        {
            competenceUsed = selectedUnit.data.attaqueData;
            tilesCompetenceDisplayed = selectedUnit.tilesAttack;
            competenceLevel = selectedUnit.AttackLevel;
        }

        else if (index == 1)
        {
            competenceUsed = selectedUnit.data.competence1Data;
            tilesCompetenceDisplayed = selectedUnit.tilesCompetence1;
            competenceLevel = selectedUnit.Competence1Level;
        }

        else
        {
            competenceUsed = selectedUnit.data.competence2Data;
            tilesCompetenceDisplayed = selectedUnit.tilesCompetence2;
            competenceLevel = selectedUnit.Competence2Level;
        }


        if (indexCompetence == index && competenceSelect)
        {
            competenceSelect = false;
            competenceUsed = null;
        }
        else
        {
            indexCompetence = index;
            competenceSelect = true;
        }
        
        unitSelect = true;


        ManageOverlayTiles();
    }
}
