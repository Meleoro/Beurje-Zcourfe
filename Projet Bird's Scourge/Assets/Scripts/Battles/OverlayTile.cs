using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayTile : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _arrowSpriteRenderer;

    [Header("Pathfinding")] 
    public List<Sprite> arrows;
    [HideInInspector] public int costG;
    [HideInInspector] public int costH;
    public int costF { get { return costG + costH; } }
    [HideInInspector] public OverlayTile previous;
    [HideInInspector] public OverlayTile isBlocked;
    [HideInInspector] public Vector3Int posOverlayTile;
    


    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _arrowSpriteRenderer = GetComponentsInChildren<SpriteRenderer>()[1];
        
        HideTile();
    }
    

    public void ShowTile()
    {
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.b, _spriteRenderer.color.g, 1);
    }

    public void HideTile()
    {
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.b, _spriteRenderer.color.g, 0);
        
        HideArrow();
    }

    public void ChangeColor(Color newColor)
    {
        _spriteRenderer.color = new Color(newColor.r, newColor.b, newColor.g, _spriteRenderer.color.a);
    }

    public void HideArrow()
    {
        _arrowSpriteRenderer.color = new Color(1, 1, 1, 0);
    }


    public void DisplayArrow(ArrowCreator.ArrowDirection arrowDirection)
    {
        if (arrowDirection == ArrowCreator.ArrowDirection.none)
        {
            HideArrow();
        }
        else
        {
            _arrowSpriteRenderer.sprite = arrows[(int)arrowDirection];
            _arrowSpriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }
}
