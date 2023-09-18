using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour
{
    [Header("Instance")] 
    private static MouseManager _instance;
    public static MouseManager Instance
    {
        get { return _instance; }
    }

    [Header("Param�tres")]
    public Color tilesAttackColorOver;
    public Color unitTurnOutlineColor;
    public float turnOutlineSpeed;

    [Header("OverlayTiles")]
    [HideInInspector] public List<OverlayTile> tilesCompetenceDisplayed = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesAtRangeDisplayed = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesSelectedZone = new List<OverlayTile>();
    [HideInInspector] public int indexCompetence;
    [HideInInspector] public int competenceLevel;
    [HideInInspector] public DataCompetence competenceUsed;
    [HideInInspector] public bool competenceSelect;
    [HideInInspector] public bool unitSelect;
    [HideInInspector] public bool competenceDisplayed;
    [HideInInspector] public OverlayTile currentSelectedTile;

    [Header("OverlayUnit")]
    [HideInInspector] public Unit overlayedUnit; 
    [HideInInspector] public Ennemy overlayedEnnemy;
    [HideInInspector] public Unit selectedUnit;
    [HideInInspector] public Ennemy selectedEnnemy;

    [Header("Objects")] 
    public ShopItemData testItem;
    public bool isUsingObject;

    [Header("Other")]
    [HideInInspector] public bool noControl;
    [HideInInspector] public bool isOnButton;
    private List<OverlayTile> currentPath = new List<OverlayTile>();
    private List<Ennemy> clickedEnnemies = new List<Ennemy>();
    private List<Ennemy> clickedSummons = new List<Ennemy>();
    private List<Unit> clickedUnits = new List<Unit>();

    [Header("References")] 
    private ObjectController scriptObject;
    private OutlineManage outlineScript;
    [HideInInspector] public TilesMouseManager tilesScript;
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

        scriptObject = GetComponent<ObjectController>();
        outlineScript = GetComponent<OutlineManage>();
        tilesScript = GetComponent<TilesMouseManager>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            scriptObject.UseObject(testItem);
        }
        
        if (!noControl && !isUsingObject)
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
        
        else if(isUsingObject)
        {
            scriptObject.ObjectSelectionUpdate();
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
                        ActualiseClickedCharas(clickedObject.GetComponent<Unit>().currentTile);
                        if (clickedUnits.Count != 0 && clickedSummons.Count != 0)
                        {
                            selectedUnit.LaunchAttack(clickedEnnemies, clickedUnits, clickedSummons, tilesCompetenceDisplayed, competenceUsed, competenceLevel);
                        }
                        break;
                    }
                    
                    SelectUnit(clickedObject.GetComponent<Unit>());
                    break;
                }

                if (clickedObject.CompareTag("Ennemy"))
                {
                    if (competenceSelect)
                    {
                        ActualiseClickedCharas(clickedObject.GetComponent<Ennemy>().currentTile);
                        if(clickedEnnemies.Count != 0)
                            selectedUnit.LaunchAttack(clickedEnnemies, clickedUnits, clickedSummons, tilesCompetenceDisplayed, competenceUsed, competenceLevel);
                        
                        break;
                    }
                    
                    SelectEnnemy(clickedObject.GetComponent<Ennemy>());
                }

                else if (unitSelect && clickedObject.CompareTag("Tile"))
                {
                    CLickTile(clickedObject.GetComponent<OverlayTile>());

                    break;
                }
            }
        }
        else if(!isOnButton)
        {
            StopSelection();
        }
    }

    private void CLickTile(OverlayTile clickedTile)
    {
        if (competenceSelect)
        {
            ActualiseClickedCharas(clickedTile);
            
            selectedUnit.LaunchAttack(clickedEnnemies, clickedUnits, clickedSummons, tilesCompetenceDisplayed, competenceUsed, competenceLevel);
        }
        
        else if (tilesAtRangeDisplayed.Contains(clickedTile))
        {
            StartCoroutine(selectedUnit.MoveToTile(currentPath));
        }

        else if (!isOnButton)
        {
            outlineScript.ManageClickedElement(null, null);

            StopSelection();
        }
    }

    private void ActualiseClickedCharas(OverlayTile clickedTile)
    {
        clickedUnits.Clear();
        clickedEnnemies.Clear();
        clickedSummons.Clear();
        
        if (competenceUsed.levels[competenceLevel].isZoneEffect)
        {
            for (int i = 0; i < tilesSelectedZone.Count; i++)
            {
                if (BattleManager.Instance.activeUnits.Keys.Contains((Vector2Int)tilesSelectedZone[i].posOverlayTile))
                {
                    clickedUnits.Add(BattleManager.Instance.activeUnits[(Vector2Int)tilesSelectedZone[i].posOverlayTile]);
                }
                    
                else if (BattleManager.Instance.activeEnnemies.Keys.Contains((Vector2Int)tilesSelectedZone[i].posOverlayTile))
                {
                    clickedEnnemies.Add(BattleManager.Instance.activeEnnemies[(Vector2Int)tilesSelectedZone[i].posOverlayTile]);
                }
                    
                else if (BattleManager.Instance.activeSummons.Keys.Contains((Vector2Int)tilesSelectedZone[i].posOverlayTile))
                {
                    clickedSummons.Add(BattleManager.Instance.activeSummons[(Vector2Int)tilesSelectedZone[i].posOverlayTile]);
                }
            }
        }

        else
        {
            if (BattleManager.Instance.activeUnits.Keys.Contains((Vector2Int)clickedTile.posOverlayTile))
            {
                clickedUnits.Add(BattleManager.Instance.activeUnits[(Vector2Int)clickedTile.posOverlayTile]);
            }
                    
            else if (BattleManager.Instance.activeEnnemies.Keys.Contains((Vector2Int)clickedTile.posOverlayTile))
            {
                clickedEnnemies.Add(BattleManager.Instance.activeEnnemies[(Vector2Int)clickedTile.posOverlayTile]);
            }
                    
            else if (BattleManager.Instance.activeSummons.Keys.Contains((Vector2Int)clickedTile.posOverlayTile))
            {
                clickedSummons.Add(BattleManager.Instance.activeSummons[(Vector2Int)clickedTile.posOverlayTile]);
            }
        }
    }
    
    
    
    // --------------------------------- OVERLAY PART -----------------------------------
    
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
                OverlayEnnemy(hits[i].collider.GetComponent<Ennemy>());

                return hits[i].collider.GetComponent<Ennemy>().currentTile;
            }

            
            // Overlay of an unit
            if (hits[i].collider.gameObject.CompareTag("Unit"))
            {
                OverlayUnit(hits[i].collider.GetComponent<Unit>());
                
                return hits[i].collider.GetComponent<Unit>().currentTile;
            }

            
            // Shows the infos of the currently playing unit
            if (BattleManager.Instance.order[0].CompareTag("Unit"))
            {
                UIBattleManager.Instance.OpenUnitInfos(BattleManager.Instance.order[0].GetComponent<Unit>().data,
                    BattleManager.Instance.order[0].GetComponent<Unit>(), null);
            }

            // Shows the infos of the currently playing ennemy
            else if (BattleManager.Instance.order[0].CompareTag("Ennemy") || BattleManager.Instance.order[0].CompareTag("Summon"))
            {
                UIBattleManager.Instance.OpenUnitInfos(BattleManager.Instance.order[0].GetComponent<Ennemy>().data,
                    null, BattleManager.Instance.order[0].GetComponent<Ennemy>());
            }

            if (hits[i].collider.gameObject.CompareTag("Tile"))
            {
                OverlayTile(hits[i].collider.GetComponent<OverlayTile>());
                
                return hits[i].collider.gameObject.GetComponent<OverlayTile>();
            }
        }

        outlineScript.ManageOverlayElement(null, null);

        return null;
    }
    

    private void OverlayUnit(Unit currentOverlayedUnit)
    {
        outlineScript.ManageOverlayElement(currentOverlayedUnit, null);

        UIBattleManager.Instance.OpenUnitInfos(currentOverlayedUnit.data, currentOverlayedUnit, null);
    }

    
    private void OverlayEnnemy(Ennemy currentOverlayedEnnemy)
    {
        outlineScript.ManageOverlayElement(null, currentOverlayedEnnemy);

        if (competenceSelect && selectedUnit != null)
        {
            selectedUnit.DisplayBattleStats(currentOverlayedEnnemy, competenceUsed, competenceLevel);

            overlayedEnnemy = currentOverlayedEnnemy;
            overlayedEnnemy.ActivateFlicker();
        }
        else
        {
            UIBattleManager.Instance.OpenUnitInfos(currentOverlayedEnnemy.data,null, currentOverlayedEnnemy);
        }
    }

    
    private void OverlayTile(OverlayTile overlayedTile)
    {
        outlineScript.ManageOverlayElement(null, null);

        if (competenceSelect)
        {
            tilesScript.DisplaySelectedTile(overlayedTile);
        }

        UIBattleManager.Instance.CloseAttackPreview();
    }



    // ---------------- SELECTED UNIT PART ----------------
    
    // SETUP EVERYTHING WHEN WE WANT TO SELECT AN UNIT
    public void SelectUnit(Unit currentUnit)
    {
        UIBattleManager.Instance.OpenUnitInfos(currentUnit.data, currentUnit, null);
        UIBattleManager.Instance.UpdateTurnUISelectedUnit(currentUnit);

        StartCoroutine(effectMaker.SquishTransform(currentUnit.transform, 1.2f, 0.07f));
        CameraManager.Instance.SelectCharacter(currentUnit, null);
        
        currentUnit.SelectUnit();

        competenceSelect = false;
        unitSelect = true;

        outlineScript.ManageClickedElement(currentUnit, null);
    }
    
    
    // SETUP EVERYTHING WHEN WE WANT TO SELECT AN ENNEMY
    public void SelectEnnemy(Ennemy currentEnnemy)
    {
        UIBattleManager.Instance.OpenUnitInfos(currentEnnemy.data, null, currentEnnemy);

        StartCoroutine(effectMaker.SquishTransform(currentEnnemy.transform, 1.2f, 0.07f));
        CameraManager.Instance.SelectCharacter(null, currentEnnemy);
        
        //currentEnnemy.SelectUnit();

        competenceSelect = false;
        unitSelect = true;

        outlineScript.ManageClickedElement(null, currentEnnemy);
    }
    
        
    // WHEN THE PLAYER DESELECT A CHARACTER
    public void StopSelection()
    {
        competenceSelect = false;
        competenceDisplayed = false;
        
        unitSelect = false;
        selectedUnit = null;
        
        UIBattleManager.Instance.UpdateTurnUISelectedUnit(selectedUnit);
        tilesScript.ManageOverlayTiles();
    }

    
    public void ResetSelection()
    {
        competenceSelect = false;
        competenceDisplayed = false;
        
        unitSelect = false;
        selectedUnit = null;
        
        selectedEnnemy = null;

        overlayedUnit = null;
        overlayedEnnemy = null;
        
        UIBattleManager.Instance.UpdateTurnUISelectedUnit(selectedUnit);
        tilesScript.ManageOverlayTiles();
        outlineScript.ManageClickedElement(null, null);
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
        competenceDisplayed = false;


        tilesScript.ManageOverlayTiles(true, true);
    }
    
    
    // DISPLAY THE ARROW OF THE PATH THAT WILL USE THE UNIT
    private void DisplayArrow(OverlayTile focusedTile)
    {
        if (unitSelect && !competenceSelect && selectedEnnemy == null)
        {
            if (selectedUnit.mustBeSelected)
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
}
