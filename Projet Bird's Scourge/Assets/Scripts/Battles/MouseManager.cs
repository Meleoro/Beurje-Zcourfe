using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    void Update()
    {
        GameObject currentTile = GetFocusedTile();

        if (currentTile != null)
        {
            Debug.Log(currentTile.transform.position);
            
            transform.position = currentTile.transform.position;
        }
    }


    public GameObject GetFocusedTile()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits[0].collider.gameObject;
        }
        
        return null;
    }
}
