using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.WSA;

public class RangeFinder
{
    // FINDS THE TILES WHERE AN ENNEMY CAN MOVE
    public List<OverlayTile> FindMoveTilesEnnemy(OverlayTile start, List<ListBool> movePatern)
    {
        List<Vector2Int> coordinates = new List<Vector2Int>();
        List<OverlayTile> moveTiles = new List<OverlayTile>();

        for (int y = 0; y < movePatern.Count; y++)
        {
            for (int x = 0; x < movePatern[y].list.Count; x++)
            {
                if (movePatern[y].list[x])
                {
                    int newY = movePatern.Count - y - 1;
                    
                    coordinates.Add(new Vector2Int( start.posOverlayTile.x + x - movePatern.Count / 2, start.posOverlayTile.y + newY - movePatern.Count / 2));
                }
            }
        }

        for (int i = 0; i < coordinates.Count; i++)
        {
            if (MapManager.Instance.map.ContainsKey(coordinates[i]))
            {
                moveTiles.Add(MapManager.Instance.map[coordinates[i]]);
            }
        }

        return moveTiles;
    }


    public List<OverlayTile> FindTilesCompetenceEnnemy(List<OverlayTile> possibleMoves, DataCompetence competenceUsed, int competenceLevel, OverlayTile currentTile, bool shyBehavior)
    {
        List<OverlayTile> result = new List<OverlayTile>();
        List<Vector2Int> tilesUnits = BattleManager.Instance.activeUnits.Keys.ToList();

        int greaterDistanceUnit = 0;
        int nearestDistanceUnit = 100;
        int smallerMoveDistance = 100;

        OverlayTile currentMoveTile = null;
        OverlayTile currentAttackTile = null;
        
        for (int i = 0; i < possibleMoves.Count; i++)
        {
            List<OverlayTile> attackTiles = FindTilesCompetence(possibleMoves[i], competenceUsed, competenceLevel);
            Vector2Int currentMoveCoordinates = (Vector2Int) possibleMoves[i].posOverlayTile;

            for (int j = 0; j < attackTiles.Count; j++)
            {
                Vector2Int currentAttackCoordinates = (Vector2Int) attackTiles[j].posOverlayTile;

                // If the attack hits an unit
                if (tilesUnits.Contains(currentAttackCoordinates))
                {
                    // Distance between the attack spot and the attacked spot
                    int currentDistance = CalculateDistance(currentMoveCoordinates, currentAttackCoordinates);
                    int currentMoveDistance = CalculateDistance((Vector2Int)currentTile.posOverlayTile, currentMoveCoordinates);

                    // If the ennemy wants to go as near as possible
                    if (!shyBehavior)
                    {
                        if (nearestDistanceUnit == currentDistance)
                        {

                            if (currentMoveDistance < smallerMoveDistance)
                            {
                                currentMoveTile = possibleMoves[i];
                                currentAttackTile = attackTiles[j];
                                smallerMoveDistance = currentMoveDistance;
                            }
                        }
                        
                        if (nearestDistanceUnit > currentDistance)
                        {
                            nearestDistanceUnit = currentDistance;

                            currentMoveTile = possibleMoves[i];
                            currentAttackTile = attackTiles[j];
                            smallerMoveDistance = currentMoveDistance;
                        }
                    }

                    // If the ennemy wants to go as far as possible
                    else
                    {
                        if (greaterDistanceUnit == currentDistance)
                        {
                            if (currentMoveDistance < smallerMoveDistance)
                            {
                                currentMoveTile = possibleMoves[i];
                                currentAttackTile = attackTiles[j];
                                smallerMoveDistance = currentMoveDistance;
                            }
                        }
                        
                        if (greaterDistanceUnit < currentDistance)
                        {
                            greaterDistanceUnit = currentDistance;

                            currentMoveTile = possibleMoves[i];
                            currentAttackTile = attackTiles[j];
                            smallerMoveDistance = currentMoveDistance;
                        }
                    }
                }
            }
        }

        if (currentMoveTile != null)
        {
            result.Add(currentMoveTile);
            result.Add(currentAttackTile);
        }

        // If no possible attack was found
        else
        {
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                for (int j = 0; j < tilesUnits.Count; j++)
                {
                    int currentDistance = CalculateDistance((Vector2Int) possibleMoves[i].posOverlayTile, tilesUnits[i]);
                    
                    if (nearestDistanceUnit > currentDistance)
                    {
                        nearestDistanceUnit = currentDistance;

                        currentMoveTile = possibleMoves[i];
                    }
                }
            }
            
            result.Add(currentMoveTile);
        }

        return result;
    }


    public int CalculateDistance(Vector2Int start, Vector2Int end)
    {
        return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y);
    }


    // FIND THE TILES CONCERNED WITH THE COMPETENCE IN THE PARAMETERS
    public List<OverlayTile> FindTilesCompetence(OverlayTile start, DataCompetence competenceUsed, int competenceLevel)
    {
        // First we find the coordinates to check
        List<Vector2Int> coordinates = new List<Vector2Int>();
        List<OverlayTile> tilesToColor = new List<OverlayTile>();

        if (competenceUsed.levels[competenceLevel].isCustom)
        {
            List<ListBool> paterne = competenceUsed.levels[competenceLevel].newPaterne;

            for (int y = 0; y < paterne.Count; y++)
            {
                for (int x = 0; x < paterne[y].list.Count; x++)
                {
                    if (paterne[y].list[x])
                    {
                        int newY = paterne.Count - y - 1;

                        coordinates.Add(new Vector2Int( start.posOverlayTile.x + x - paterne.Count / 2, start.posOverlayTile.y + newY - paterne.Count / 2));
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
