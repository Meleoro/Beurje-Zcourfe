using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangeFinder 
{
    // FIND THE TILES CONCERNED WITH THE COMPETENCE IN THE PARAMETERS
    public List<OverlayTile> FindTilesCompetence(OverlayTile start, DataCompetence competenceUsed, int competenceLevel)
    {
        // First we find the coordinates to check
        List<Vector2Int> coordinates = new List<Vector2Int>();
        List<OverlayTile> tilesToColor = new List<OverlayTile>();

        if (competenceUsed.levels[competenceLevel].isCustom)
        {
            List<ListBool> paterne = competenceUsed.levels[competenceLevel].newPaterne;

            for (int x = 0; x < paterne.Count; x++)
            {
                for (int y = 0; y < paterne[x].list.Count; y++)
                {
                    if (paterne[x].list[y])
                    {
                        coordinates.Add(new Vector2Int( start.posOverlayTile.x + x - 3, start.posOverlayTile.y + y - 3));
                    }
                }
            }
        }
        
        // Next we verify if they exist and, if so, add them to the returned list
        for (int i = 0; i < coordinates.Count; i++)
        {
            if (MapManager.Instance.map.ContainsKey(coordinates[i]))
            {
                tilesToColor.Add(MapManager.Instance.map[coordinates[i]]);
            }
        }
        
        return tilesToColor;
    }

    
    // SEEK ALL THE TILES AVAILABLE IN A SPECIFIC RANGE
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
