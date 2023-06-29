using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PathFinder
{
    // RENVOIE UNE LISTE QUI EST LE CHEMIN ENTRE DEUX POINTS
    public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> openList = new List<OverlayTile>();
        List<OverlayTile> closeList = new List<OverlayTile>();
        
        openList.Add(start);

        while (openList.Count >= 0)
        {
            int indexNearestTile = FindNearestTile(openList);
            OverlayTile currentTile = openList[indexNearestTile];
            
            openList.RemoveAt(indexNearestTile);
            closeList.Add(currentTile);

            if (currentTile == end)
            {
                return FindFinalPath(start, end);
            }

            List<OverlayTile> neighbors = FindNeighbors(currentTile);

            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors[i].isBlocked || closeList.Contains(neighbors[i]) || openList.Contains(neighbors[i]))
                {
                    continue;
                }

                neighbors[i].costG = GetManhattanDistance(start, neighbors[i]);
                neighbors[i].costH = GetManhattanDistance(end, neighbors[i]);

                neighbors[i].previous = currentTile;
                
                openList.Add(neighbors[i]);
            }
        }

        return new List<OverlayTile>();
    }


    // UNE FOIS TOUTES LES CASES TROUVEES, CREE UNE LISTE QUI SERA LA CHEMIN QUE L'UNITE DEVRA EMPRUNTER
    private List<OverlayTile> FindFinalPath(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> finishedList = new List<OverlayTile>();
        OverlayTile currentTile = end;

        while (currentTile != start)
        {
            finishedList.Add(currentTile);

            currentTile = currentTile.previous;
        }

        finishedList.Reverse();

        return finishedList;
    }


    // CALCULE LA DISTANCE ENTRE DEUX CASES AVEC LA FORMULE MANHATTAN
    private int GetManhattanDistance(OverlayTile start, OverlayTile currentTile)
    {
        return Mathf.Abs(start.posOverlayTile.x - currentTile.posOverlayTile.x) + Mathf.Abs(start.posOverlayTile.y - currentTile.posOverlayTile.y);
    }
    
    
    // REGARDE LES COUTS F (EN FONCTION DU DEPART) DE CHAQUES CASES D'UNE LISTE ET RENVOIE LA PLUS PROCHE
    private int FindNearestTile(List<OverlayTile> tiles)
    {
        int indexSelectedTile = 0;
        int nearestF = 100;
        
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].costF < nearestF)
            {
                indexSelectedTile = i;
                nearestF = tiles[i].costF;
            }
        }

        return indexSelectedTile;
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
            neighbors.Add(currentMap[newPos]);
        }
        
        // Down
        newPos = new Vector2Int(currentPos.x, currentPos.y - 1);
        
        if (currentMap.ContainsKey(newPos))
        {
            neighbors.Add(currentMap[newPos]);
        }
        
        // Left
        newPos = new Vector2Int(currentPos.x + 1, currentPos.y);
        
        if (currentMap.ContainsKey(newPos))
        {
            neighbors.Add(currentMap[newPos]);
        }
        
        // Right
        newPos = new Vector2Int(currentPos.x - 1, currentPos.y);
        
        if (currentMap.ContainsKey(newPos))
        {
            neighbors.Add(currentMap[newPos]);
        }

        return neighbors;
    }
}
