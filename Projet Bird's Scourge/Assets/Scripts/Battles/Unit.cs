using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Vector2Int startTile;
    
    public OverlayTile currentTile;


    private void Start()
    {
        currentTile = MapManager.Instance.map[startTile];

        MoveToTile(currentTile.transform.position);
    }


    public void MoveToTile(Vector2 newPos)
    {
        transform.position = newPos + new Vector2(0, 0.4f);
    }
    
    public IEnumerator MoveToTile(List<OverlayTile> path)
    {
        for(int i = 0; i < path.Count; i++)
        {
            transform.position = path[i].transform.position + new Vector3(0, 0.4f, -1);
            
            yield return new WaitForSeconds(0.2f);
        }
        
        
        currentTile = path[path.Count - 1];
    }
}
