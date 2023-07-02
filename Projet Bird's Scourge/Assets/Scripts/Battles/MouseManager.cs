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
    private bool competenceSelect;
    private bool unitSelect;
    
    [Header("Other")]
    public Unit selectedUnit;
    private List<OverlayTile> currentPath = new List<OverlayTile>();

    [Header("References")] 
    public UIBattle currentUI;
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
        OverlayTile currentTile = GetFocusedTile();

        if (unitSelect)
        {
            if (currentTile != null)
            {
                DisplayArrow(currentTile);
            
                transform.position = currentTile.transform.position;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            VerifyClickedElement();

            ManageOverlayTiles();
        }
    }


    // SEEK ON WHICH ELEMENT THE CHARACTER CLICKED
    private void VerifyClickedElement()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        if (hits.Length > 0)
        {
            GameObject clickedObject = hits[0].collider.gameObject;

            if (clickedObject.CompareTag("Unit"))
            {
                selectedUnit = clickedObject.GetComponent<Unit>();
                
                currentUI.OpenUnitInfos(selectedUnit.data);

                competenceSelect = false;
                unitSelect = true;
            }

            else if(unitSelect && clickedObject.CompareTag("Tile") && !competenceSelect)
            {
                if (tilesAtRangeDisplayed.Contains(clickedObject.GetComponent<OverlayTile>()))
                {
                    StartCoroutine(selectedUnit.MoveToTile(currentPath));
                }
                else
                {
                    StopSelection();
                }
            }
        }

        else
        {
            //StopSelection();
        }
    }

    
    // WHEN THE CHARACTER DESELECT A CHARACTER
    private void StopSelection()
    {
        competenceSelect = false;
        unitSelect = false;

        ManageOverlayTiles();
    }
    

    // SEEK ON WHICH TILE IS THE MOUSE
    private OverlayTile GetFocusedTile()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.CompareTag("Tile"))
            {
                return hits[i].collider.gameObject.GetComponent<OverlayTile>();
            }
        }

        return null;
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
            tilesCompetenceDisplayed = selectedUnit.tilesAttack;
        
        else if (index == 1)
            tilesCompetenceDisplayed = selectedUnit.tilesCompetence1;
        
        else
            tilesCompetenceDisplayed = selectedUnit.tilesCompetence2;

        
        if (indexCompetence == index && competenceSelect)
        {
            competenceSelect = false;
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
