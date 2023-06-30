using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCreator 
{
    public enum ArrowDirection
    {
        none = 0,
        up = 1,
        down = 2,
        left = 3,
        right = 4,
        upLeft = 5,
        upRight = 6,
        downLeft = 7,
        downRight = 8,
        endUp = 9,
        endDown = 10,
        endLeft = 11,
        endRight = 12
    }

    public ArrowDirection CreateArrow(OverlayTile previousTile, OverlayTile currentTile, OverlayTile futureTile)
    {
        bool isFinal = futureTile == null;

        Vector2Int pastDirection = previousTile != null
            ? new Vector2Int(currentTile.posOverlayTile.x - previousTile.posOverlayTile.x, currentTile.posOverlayTile.y - previousTile.posOverlayTile.y)
            : Vector2Int.zero;
        
        Vector2Int futureDirection = !isFinal 
            ? new Vector2Int(futureTile.posOverlayTile.x - currentTile.posOverlayTile.x, futureTile.posOverlayTile.y - currentTile.posOverlayTile.y) 
            : Vector2Int.zero;

        Vector2Int direction = futureDirection != pastDirection ? pastDirection + futureDirection : futureDirection;

        
        // STRAY LINES
        if (direction == new Vector2Int(0, 1) && !isFinal)
        {
            return ArrowDirection.up;
        }

        if (direction == new Vector2Int(0, -1) && !isFinal)
        {
            return ArrowDirection.down;
        }
        
        if (direction == new Vector2Int(-1, 0) && !isFinal)
        {
            return ArrowDirection.left;
        }
        
        if (direction == new Vector2Int(1, 0) && !isFinal)
        {
            return ArrowDirection.right;
        }

        // CORNERS
        if (direction == new Vector2Int(-1, 1) && !isFinal)
        {
            return ArrowDirection.upLeft;
        }
        
        if (direction == new Vector2Int(1, 1) && !isFinal)
        {
            return ArrowDirection.upRight;
        }
        
        if (direction == new Vector2Int(-1, -1) && !isFinal)
        {
            return ArrowDirection.downLeft;
        }
        
        if (direction == new Vector2Int(1, -1) && !isFinal)
        {
            return ArrowDirection.downRight;
        }
        
        // ENDS
        if (direction == new Vector2Int(0, 1) && isFinal)
        {
            return ArrowDirection.endUp;
        }

        if (direction == new Vector2Int(0, -1) && isFinal)
        {
            return ArrowDirection.endDown;
        }
        
        if (direction == new Vector2Int(-1, 0) && isFinal)
        {
            return ArrowDirection.endLeft;
        }
        
        if (direction == new Vector2Int(1, 0) && isFinal)
        {
            return ArrowDirection.endRight;
        }

        Debug.Log(12);
        return ArrowDirection.none;
    }
}
