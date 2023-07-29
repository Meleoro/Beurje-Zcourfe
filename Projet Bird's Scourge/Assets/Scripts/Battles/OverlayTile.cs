using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class OverlayTile : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _arrowSpriteRenderer;

    [Header("Parameters")] 
    [Range(0, 1)] public float wantedTransparency;
    [Range(0, 1)] public float strengthApparitionEffect;

    [Header("Pathfinding")] 
    public List<Sprite> arrows;
    [HideInInspector] public int costG;
    [HideInInspector] public int costH;
    public int costF { get { return costG + costH; } }
    [HideInInspector] public OverlayTile previous;
    public bool isBlocked;
    [HideInInspector] public Vector3Int posOverlayTile;

    [Header("Other")] private Vector3 originalPos;
    


    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _arrowSpriteRenderer = GetComponentsInChildren<SpriteRenderer>()[1];

        originalPos = transform.position;
        
        HideTile();
    }
    

    public void ShowTile()
    {
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, wantedTransparency);
    }

    public void HideTile()
    {
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0);
        
        HideArrow();
    }

    public IEnumerator AppearEffect(float effectDuration, Color newColor)
    {
        _spriteRenderer.DOColor(new Color(newColor.r, newColor.g, newColor.b, _spriteRenderer.color.a), 0);
        _spriteRenderer.DOFade(wantedTransparency, effectDuration);

        transform.DOMoveY(transform.position.y + strengthApparitionEffect, effectDuration * 0.8f);

        yield return new WaitForSeconds(effectDuration * 0.8f);
        
        transform.DOMoveY(transform.position.y - strengthApparitionEffect, effectDuration * 0.5f);
    }

    public void AppearEffectLauncher(float effectDuration, Color newColor)
    {
        StartCoroutine(AppearEffect(effectDuration, newColor));
    }
    
    public void ResetTile()
    {
        DOTween.KillAll();
        StopAllCoroutines();

        transform.position = originalPos;
        HideTile();
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
