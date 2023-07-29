using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    [Header("Instance")] 
    private static MouseManager _instance;
    public static MouseManager Instance
    {
        get { return _instance; }
    }

    [Header("Param�tres")]
    [SerializeField] private bool outlineWhenOver;
    [SerializeField] private Color outlineSelectedUnit;
    [SerializeField] private Color outlineSelectedEnnemy;
    [SerializeField] private Color tilesMovementColor;
    [SerializeField] private Color tilesMovementColorSelected;
    [SerializeField] private Color tilesAttackColor;

    [Header("OverlayTiles")]
    [HideInInspector] public List<OverlayTile> tilesCompetenceDisplayed = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesAtRangeDisplayed = new List<OverlayTile>();
    private int indexCompetence;
    private int competenceLevel;
    [HideInInspector] public DataCompetence competenceUsed;
    private bool competenceSelect;
    private bool unitSelect;

    [Header("OverlayUnit")]
    private Unit currentOverlayedUnit; 
    private Ennemy currentOverlayedEnnemy;
    [HideInInspector] public Unit selectedUnit;
    [HideInInspector] public Ennemy selectedEnnemy;

    [Header("Other")]
    [HideInInspector] public bool noControl;
    [HideInInspector] public bool isOnButton;
    private List<OverlayTile> currentPath = new List<OverlayTile>();

    [Header("References")]
    private PathFinder pathFinder;
    private ArrowCreator arrowCreator;
    private EffectMaker effectMaker;
    

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
        effectMaker = new EffectMaker();
    }


    void Update()
    {
        if (!noControl)
        {
            OverlayTile currentTile = GetFocusedElement();

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
                
                //ManageOverlayTiles();
            }
        }
    }

    
    // ---------------- SELECTION PART ----------------

    // SEEK ON WHICH ELEMENT THE CHARACTER CLICKED
    private void VerifyClickedElement()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        if(hits.Length > 0 && !isOnButton)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                GameObject clickedObject = hits[i].collider.gameObject;

                if (clickedObject.CompareTag("Unit"))
                {
                    if (competenceSelect)
                    {
                        selectedUnit.LaunchAttack(null, clickedObject.GetComponent<Unit>(), tilesCompetenceDisplayed, competenceUsed, competenceLevel);

                        break;
                    }
                    
                    SelectUnit(clickedObject.GetComponent<Unit>());

                    break;
                }

                else if (clickedObject.CompareTag("Ennemy"))
                {
                    if (competenceSelect)
                    {
                        selectedUnit.LaunchAttack(clickedObject.GetComponent<Ennemy>(), null, tilesCompetenceDisplayed, competenceUsed, competenceLevel);

                        break;
                    }
                }

                else if (unitSelect && clickedObject.CompareTag("Tile") && !competenceSelect)
                {
                    if (tilesAtRangeDisplayed.Contains(clickedObject.GetComponent<OverlayTile>()))
                    {
                        StartCoroutine(selectedUnit.MoveToTile(currentPath));

                        break;
                    }

                    else if (!isOnButton)
                    {
                        ManageOverlayUnit(null, null, true);

                        StopSelection();

                        break;
                    }
                }
            }
        }
        else if(!isOnButton)
        {
            StopSelection();
        }
    }
    
    
    // SEEK ON WHICH ELEMENT IS THE MOUSE
    private OverlayTile GetFocusedElement()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        for (int i = 0; i < hits.Length; i++)
        {
            // Overlay of an ennemy
            if (hits[i].collider.gameObject.CompareTag("Ennemy"))
            {
                Ennemy currentEnnemy = hits[i].collider.GetComponent<Ennemy>();
                ManageOverlayUnit(null, currentEnnemy, false);

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

            // Overlay of an unit
            if (hits[i].collider.gameObject.CompareTag("Unit"))
            {
                Unit currentUnit = hits[i].collider.GetComponent<Unit>();
                
                ManageOverlayUnit(currentUnit, null, false);

                UIBattleManager.Instance.OpenUnitInfos(currentUnit.data, currentUnit, null);
                
                return currentUnit.currentTile;
            }

            
            // Shows the infos of the currently playing unit
            if (BattleManager.Instance.order[0].CompareTag("Unit"))
            {
                UIBattleManager.Instance.OpenUnitInfos(BattleManager.Instance.order[0].GetComponent<Unit>().data,
                    BattleManager.Instance.order[0].GetComponent<Unit>(), null);
            }

            // Shows the infos of the currently playing ennemy
            else if (BattleManager.Instance.order[0].CompareTag("Ennemy"))
            {
                UIBattleManager.Instance.OpenUnitInfos(BattleManager.Instance.order[0].GetComponent<Ennemy>().data,
                    null, BattleManager.Instance.order[0].GetComponent<Ennemy>());
            }

            if (hits[i].collider.gameObject.CompareTag("Tile"))
            {
                ManageOverlayUnit(null, null, false);

                UIBattleManager.Instance.CloseAttackPreview();
                
                return hits[i].collider.gameObject.GetComponent<OverlayTile>();
            }
        }

        ManageOverlayUnit(null, null, false);

        return null;
    }



    // ---------------- SELECTED UNIT PART ----------------
    
    // MANAGES WHAT HAS TO BE OUTLINED 
    public void ManageOverlayUnit(Unit currentUnit, Ennemy currentEnnemy, bool click)
    {
        if (!click)
        {
            // If nothing is selected
            if (currentUnit == null && currentEnnemy == null)
            {
                if (!unitSelect)
                {
                    if (currentOverlayedEnnemy != null)
                    {
                        if (outlineWhenOver)
                            currentOverlayedEnnemy.DesactivateOutline();

                        currentOverlayedEnnemy = null;
                    }

                    else if (currentOverlayedUnit != null)
                    {
                        if (outlineWhenOver)
                            currentOverlayedUnit.DesactivateOutline();

                        currentOverlayedUnit = null;
                    }
                    
                    ManageOverlayTiles();
                }
            }

            // If it's an unit
            else if (currentEnnemy == null)
            {
                if (currentUnit != currentOverlayedUnit)
                {
                    if (outlineWhenOver)
                        currentOverlayedUnit.ActivateOutline(outlineSelectedUnit);

                    currentUnit.FindTilesAtRange();
                    currentOverlayedUnit = currentUnit;
                    
                    ManageOverlayTiles();
                }
            }

            // If it's an ennemy
            else
            {
                if (currentEnnemy != currentOverlayedEnnemy)
                {
                    if (outlineWhenOver)
                        currentOverlayedEnnemy.ActivateOutline(outlineSelectedEnnemy);

                    currentOverlayedEnnemy = currentEnnemy;
                    
                    ManageOverlayTiles();
                }
            }
        }

        else
        {
            if(currentUnit != null)
            {
                if (selectedUnit != currentUnit && selectedUnit != null)
                {
                    selectedUnit.DesactivateOutline();
                }
                
                currentUnit.ActivateOutline(outlineSelectedUnit);

                selectedUnit = currentUnit;
                selectedEnnemy = null;
                
                
                // If it's an automatic selection
                if (currentUnit != currentOverlayedUnit)
                {
                    ManageOverlayTiles(true, true);

                    currentOverlayedUnit = currentUnit;
                }

                else
                {
                    ManageOverlayTiles(false, true);
                }
            }

            else if(currentEnnemy != null)
            {
                if (selectedEnnemy != currentEnnemy && selectedEnnemy != null)
                {
                    selectedEnnemy.DesactivateOutline();
                }

                currentEnnemy.ActivateOutline(outlineSelectedEnnemy);

                selectedEnnemy = currentEnnemy;
                selectedUnit = null;
            }

            else
            {
                if (selectedUnit != null)
                    selectedUnit.DesactivateOutline();

                else if (selectedEnnemy != null)
                    selectedEnnemy.DesactivateOutline();

                selectedUnit = null;
                selectedEnnemy = null;
            }
        }
    }


    // SETUP EVERYTHING WHEN WE WANT TO SELECT AN UNIT
    public void SelectUnit(Unit currentUnit)
    {
        UIBattleManager.Instance.OpenUnitInfos(currentUnit.data, currentUnit, null);
        UIBattleManager.Instance.UpdateTurnUISelectedUnit(currentUnit);

        StartCoroutine(effectMaker.SquishTransform(currentUnit.transform, 1.2f, 0.07f));
        CameraManager.Instance.SelectUnit(currentUnit);

        competenceSelect = false;
        unitSelect = true;
        
        ManageOverlayUnit(currentUnit, null, true);
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

    
    
    // ---------------- TILES PART ----------------

    // MANAGE WHICH COLOR HAS TO HAVE EVERYTILES DEPENDING ON THE SITUATION
    public void ManageOverlayTiles(bool forceReset = false, bool forceChange = false)
    {
        ResetOverlayTiles(forceReset);
        
        // If a competence is selected
        if (competenceSelect) 
        {
            DisplayTilesCompetence(indexCompetence);
        }
        
        // If only the character is selected
        else 
        {
            if(selectedUnit == null && selectedEnnemy == null)
            {
                DisplayTilesAtRange(currentOverlayedUnit, currentOverlayedEnnemy, forceChange);
            }
            else
            {
                DisplayTilesAtRange(selectedUnit, selectedEnnemy, forceChange);
            }
        }
    }


    private void ResetOverlayTiles(bool forceReset)
    {
        if ((!unitSelect && !competenceSelect) || forceReset)
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
            //currentUnit.currentTilesAtRange != tilesAtRangeDisplayed
            if ((currentUnit != selectedUnit || forceChange))
            {
                tilesAtRangeDisplayed = currentUnit.currentTilesAtRange;

                if(selectedUnit is null)
                    StartCoroutine(effectMaker.MoveTilesAppear(currentUnit.currentTile, tilesAtRangeDisplayed, 0.05f, tilesMovementColor));
                
                else
                    StartCoroutine(effectMaker.MoveTilesAppear(currentUnit.currentTile, tilesAtRangeDisplayed, 0.05f, tilesMovementColorSelected));

                for (int i = 0; i < tilesAtRangeDisplayed.Count; i++)
                {
                    //tilesAtRangeDisplayed[i].ShowTile();

                    tilesAtRangeDisplayed[i].ChangeColor(tilesMovementColor);
                }
            }
        }
        else if(currentEnnemy != null)
        {
            tilesAtRangeDisplayed = currentEnnemy.currentMoveTiles;

            for (int i = 0; i < tilesAtRangeDisplayed.Count; i++)
            {
                tilesAtRangeDisplayed[i].ShowTile();

                tilesAtRangeDisplayed[i].ChangeColor(tilesMovementColor);
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

            tilesCompetenceDisplayed[i].ChangeColor(tilesAttackColor);
        }
    }
    
    
    
    // ---------------- OTHER PART ----------------
    
    
    // DISPLAY ALL TILES AT RANGE OF THE SELECTED CHARACTER OR ERASE IF NO CHARACTER IS SELECTED
    public void ChangeSelectedCompetence(int index)
    {
        if (indexCompetence == index && competenceSelect)
        {
            competenceSelect = false;
            competenceUsed = null;
        }
        else
        {
            indexCompetence = index;
            competenceSelect = true;
            
            for (int i = 0; i < tilesCompetenceDisplayed.Count; i++)
            {
                tilesCompetenceDisplayed[i].HideTile();
            }

        }
        
        
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
        
        
        unitSelect = true;


        ManageOverlayTiles(false);
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
                
                currentPath = pathFinder.FindPath(selectedUnit.currentTile, focusedTile, false);

                for (int i = 0; i < currentPath.Count; i++)
                {
                    OverlayTile previousTile = i > 0 ? currentPath[i - 1] : selectedUnit.currentTile;
                    OverlayTile nextTile = i < currentPath.Count - 1 ? currentPath[i + 1] : null;
                    
                    currentPath[i].DisplayArrow(arrowCreator.CreateArrow(previousTile, currentPath[i], nextTile));
                }
            }
        }
    }
}
