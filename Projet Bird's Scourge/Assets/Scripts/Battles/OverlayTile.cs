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
    public bool isBlocked;
    [HideInInspector] public Vector3Int posOverlayTile;
    


    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _arrowSpriteRenderer = GetComponentsInChildren<SpriteRenderer>()[1];
        
        HideTile();
    }
    

    public void ShowTile()
    {
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0.8f);
    }

    public void HideTile()
    {
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0);
        
        HideArrow();
    }

    public void ChangeColor(Color newColor)
    {
        _spriteRenderer.color = new Color(newColor.r, newColor.g, newColor.b, _spriteRenderer.color.a);
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
