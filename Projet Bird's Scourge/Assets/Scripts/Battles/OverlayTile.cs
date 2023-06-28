using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayTile : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    [Header("Pathfinding")]
    private int costG;
    private int costH;
    
    public int costF
    {
        get { return costG + costF;  }
    }


    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        HideTile();
    }
    

    public void ShowTile()
    {
        _spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void HideTile()
    {
        _spriteRenderer.color = new Color(1, 1, 1, 0);
    }
}
