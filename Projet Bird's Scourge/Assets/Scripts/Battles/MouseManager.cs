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
    
    [Header("References")]
    private PathFinder pathFinder;


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
    }


    void Update()
    {
        GameObject currentTile = GetFocusedTile();

        if (currentTile != null)
        {
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


    // SEEK ON WHICH THE CHARACTER CLICKED
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
                StartCoroutine(selectedUnit.MoveToTile(pathFinder.FindPath(selectedUnit.currentTile, clickedObject.GetComponent<OverlayTile>())));
            }
        }
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
}
