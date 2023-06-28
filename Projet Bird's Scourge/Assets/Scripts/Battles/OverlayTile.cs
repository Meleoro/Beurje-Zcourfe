using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayTile : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    [Header("Pathfinding")]
    [HideInInspector] public int costG;
    [HideInInspector] public int costH;
    public int costF { get { return costG + costH; } }
    [HideInInspector] public OverlayTile previous;
    [HideInInspector] public OverlayTile isBlocked;
    [HideInInspector] public Vector3Int posOverlayTile;


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
