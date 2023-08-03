using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class EffectMaker 
{
    // SQUISH AN ELEMENT 
    public IEnumerator SquishTransform(Transform currentTransform, float squishStrength, float squishDuration)
    {
        Vector3 originalScale = currentTransform.localScale;
        
        currentTransform.DOScaleX(currentTransform.localScale.x * squishStrength, squishDuration);
        currentTransform.DOScaleY(currentTransform.localScale.x * squishStrength * 1.1f, squishDuration);
        
        yield return new WaitForSeconds(squishDuration);

        currentTransform.DOScale(originalScale, squishDuration * 0.8f);
    }


    // MAKE THE TILES APPEAR PROGRESSIVELY 
    public IEnumerator MoveTilesAppear(OverlayTile center, List<OverlayTile> tiles, float effectSpeed, Color wantedColor)
    {
        List<OverlayTile> openList = new List<OverlayTile>();
        List<OverlayTile> closeList = new List<OverlayTile>();

        int limit = 10;
        
        openList.Add(center);

        while (closeList.Count < tiles.Count - 1 && tiles == MouseManager.Instance.tilesAtRangeDisplayed && limit > 0)
        {
            Debug.Log(limit);
            
            limit -= 1;
            
            List<OverlayTile> tilesToAppear = new List<OverlayTile>();
            List<OverlayTile> finalTilesToAppear = new List<OverlayTile>();
            
            
            for(int i = openList.Count - 1; i > -1; i--)
            {
                tilesToAppear.AddRange(FindNeighbors(openList[i]));

                openList.RemoveAt(i);
            }

            // We sort all the elements
            tilesToAppear = tilesToAppear.Distinct().ToList();
            for (int i = 0; i < tilesToAppear.Count; i++)
            {
                if (!closeList.Contains(tilesToAppear[i]) && tiles.Contains(tilesToAppear[i]))
                {
                    finalTilesToAppear.Add(tilesToAppear[i]);
                }
                else
                {
                    openList.Add(tilesToAppear[i]);
                }
            }

            // We make them appear
            for (int i = 0; i < finalTilesToAppear.Count; i++)
            {
                //finalTilesToAppear[i].ChangeColor(wantedColor);
                finalTilesToAppear[i].AppearEffectLauncher(0.1f, wantedColor);
                
                closeList.Add(finalTilesToAppear[i]);
                openList.Add(finalTilesToAppear[i]);
            }

            yield return new WaitForSeconds(effectSpeed);
        }
    }
    
    // ATTACK TILES APPEAR PROGRESSIVELY
    public IEnumerator AttackTilesAppear(OverlayTile center, List<OverlayTile> tiles, float effectSpeed, Color wantedColor)
    {
        List<OverlayTile> openList = new List<OverlayTile>();
        List<OverlayTile> closeList = new List<OverlayTile>();

        int limit = 10;
        
        openList.Add(center);

        while (closeList.Count < tiles.Count - 1 && limit > 0)
        {
            Debug.Log(limit);
            
            limit -= 1;
            
            List<OverlayTile> tilesToAppear = new List<OverlayTile>();
            List<OverlayTile> finalTilesToAppear = new List<OverlayTile>();
            
            
            for(int i = openList.Count - 1; i > -1; i--)
            {
                tilesToAppear.AddRange(FindNeighbors(openList[i]));

                openList.RemoveAt(i);
            }

            // We sort all the elements
            tilesToAppear = tilesToAppear.Distinct().ToList();
            for (int i = 0; i < tilesToAppear.Count; i++)
            {
                if (!closeList.Contains(tilesToAppear[i]) && tiles.Contains(tilesToAppear[i]))
                {
                    finalTilesToAppear.Add(tilesToAppear[i]);
                }
                else
                {
                    openList.Add(tilesToAppear[i]);
                }
            }

            // We make them appear
            for (int i = 0; i < finalTilesToAppear.Count; i++)
            {
                //finalTilesToAppear[i].ChangeColor(wantedColor);
                finalTilesToAppear[i].AppearEffectLauncher(0.1f, wantedColor);
                
                closeList.Add(finalTilesToAppear[i]);
                openList.Add(finalTilesToAppear[i]);
            }

            yield return new WaitForSeconds(effectSpeed);
        }
    }

    
    // RETURN ALL THE NEIGHBOR TILES OF THE CASE TILE PUT IN PARAMETER
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
