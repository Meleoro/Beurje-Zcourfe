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
    
    [Header("Other")]
    public Unit selectedUnit;
    private List<OverlayTile> tilesAtRangeDisplayed = new List<OverlayTile>();
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
        GameObject currentTile = GetFocusedTile();

        if (currentTile != null)
        {
            DisplayArrow(currentTile.GetComponent<OverlayTile>());
            
            transform.position = currentTile.transform.position;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            VerifyClickedElement();

            DisplayTilesAtRange();
        }
    }

    

    // DISPLAY ALL TILES AT RANGE OF THE SELECTED CHARACTER OR ERASE IF NO CHARACTER IS SELECTED
    public void DisplayTilesAtRange()
    {
        for (int i = 0; i < tilesAtRangeDisplayed.Count; i++)
        {
            tilesAtRangeDisplayed[i].HideTile();
        }

        if (selectedUnit != null)
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
            }

            else if(selectedUnit != null)
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
            StopSelection();
        }
    }

    private void StopSelection()
    {
        selectedUnit = null;
        
        DisplayTilesAtRange();
    }
    

    // SEEK ON WHICH TILE IS THE MOUSE
    private GameObject GetFocusedTile()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.CompareTag("Tile"))
            {
                return hits[i].collider.gameObject;
            }
        }

        return null;
    }

    
    private void DisplayArrow(OverlayTile focusedTile)
    {
        if (selectedUnit != null)
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
}
