using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangeFinder 
{
    // VA CHERCHER TOUTES LES TILES DISPONIBLES DANS UNE CERTAINE RANGE 
    public List<OverlayTile> FindTilesInRange(OverlayTile start, int range)
    {
        List<OverlayTile> tilesInRange = new List<OverlayTile>();
        tilesInRange.Add(start);

        List<OverlayTile> previousStepTiles = new List<OverlayTile>();
        previousStepTiles.Add(start);

        int step = 0;

        while (step < range)
        {
            List<OverlayTile> surroundingTiles = new List<OverlayTile>();

            for (int i = 0; i < previousStepTiles.Count; i++)
            {
                surroundingTiles.AddRange(FindNeighbors(previousStepTiles[i]));
            }

            tilesInRange.AddRange(surroundingTiles);
            previousStepTiles = surroundingTiles.Distinct().ToList();
            
            step++;
        }

        return tilesInRange.Distinct().ToList();
    }
    
    
    // RENVOIE UNE LISTE DE TOUS LES VOISINS DE LA CASE EN PARAMETRES (SI CASE NON VIDE)
    private List<OverlayTile> FindNeighbors(OverlayTile currentTile)
    {
        Vector2Int currentPos = new Vector2Int(currentTile.posOverlayTile.x, currentTile.posOverlayTile.y);
        List<OverlayTile> neighbors = new List<OverlayTile>();
        Dictionary<Vector2Int, OverlayTile> currentMap = MapManager.Instance.map;

        // Up
        Vector2Int newPos = new Vector2Int(currentPos.x, currentPos.y + 1);
        
        if (currentMap.ContainsKey(newPos))
        {
            if(!currentMap[newPos].isBlocked)
                neighbors.Add(currentMap[newPos]);
        }
        
        // Down
        newPos = new Vector2Int(currentPos.x, currentPos.y - 1);
        
        if (currentMap.ContainsKey(newPos))
        {
            if(!currentMap[newPos].isBlocked)
                neighbors.Add(currentMap[newPos]);
        }
        
        // Left
        newPos = new Vector2Int(currentPos.x + 1, currentPos.y);
        
        if (currentMap.ContainsKey(newPos))
        {
            if(!currentMap[newPos].isBlocked)
                neighbors.Add(currentMap[newPos]);
        }
        
        // Right
        newPos = new Vector2Int(currentPos.x - 1, currentPos.y);
        
        if (currentMap.ContainsKey(newPos))
        {
            if(!currentMap[newPos].isBlocked)
                neighbors.Add(currentMap[newPos]);
        }

        return neighbors;
    }
}
