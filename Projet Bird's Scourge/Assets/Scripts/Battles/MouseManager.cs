using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public Unit selectedUnit;
    private PathFinder pathFinder;

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
        }
    }

    
    // VA CHERCHER SUR QUEL ELEMENT LE JOUEUR A CLIQUE
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
    

    // VA CHERCHER LA TILE SUR LAQUELLE SE TROUVE ACTUELLEMENT LA SOURIS
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
