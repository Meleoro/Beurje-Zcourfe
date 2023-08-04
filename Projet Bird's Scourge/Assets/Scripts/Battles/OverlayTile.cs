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

    [Header("Other")] 
    private Vector3 originalPos;
    private bool isFlickering;
    


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
    
    public IEnumerator SelectEffect(float effectDuration, Color newColor)
    {
        _spriteRenderer.DOColor(new Color(newColor.r, newColor.g, newColor.b, _spriteRenderer.color.a), 0);
        _spriteRenderer.DOFade(wantedTransparency, effectDuration);

        transform.DOMoveY(transform.position.y + strengthApparitionEffect * 0.5f, effectDuration * 0.8f);

        yield return new WaitForSeconds(effectDuration * 0.8f);
        
        transform.DOMoveY(originalPos.y, effectDuration * 0.5f);
    }
    
    public void DeselectEffect(float effectDuration, Color newColor)
    {
        DOTween.Kill(transform);
        DOTween.Kill(_spriteRenderer);
        StopAllCoroutines();
        
        _spriteRenderer.DOColor(new Color(newColor.r, newColor.g, newColor.b, _spriteRenderer.color.a), 0);
        _spriteRenderer.DOFade(wantedTransparency, 0);

        transform.position = originalPos;
    } 


    public void AppearEffectLauncher(float effectDuration, Color newColor)
    {
        StartCoroutine(AppearEffect(effectDuration, newColor));
    }
    
    public void ResetTile()
    {
        DOTween.Kill(transform);
        DOTween.Kill(_spriteRenderer);
        StopAllCoroutines();

        transform.position = originalPos;
        HideTile();
    }


    public void LaunchFlicker(float flickerSpeed, Color flickerColor)
    {
        if (!isFlickering)
        {
            isFlickering = true;
            StartCoroutine(Flicker(flickerSpeed, flickerColor));
        }
    }

    public void StopFlicker()
    {
        isFlickering = false;
        StopAllCoroutines();
        DOTween.Kill(_spriteRenderer);
        
        _spriteRenderer.DOFade(0, 0);
    }

    private IEnumerator Flicker(float flickerSpeed, Color flickerColor)
    {
        if (flickerColor != null)
        {
            _spriteRenderer.color = new Color(flickerColor.r, flickerColor.g, flickerColor.b, _spriteRenderer.color.a);
        }
        
        _spriteRenderer.DOFade(1, flickerSpeed);

        yield return new WaitForSeconds(flickerSpeed + 0.01f);
        
        _spriteRenderer.DOFade(0, flickerSpeed);
        
        yield return new WaitForSeconds(flickerSpeed + 0.01f);

        StartCoroutine(Flicker(flickerSpeed, flickerColor));
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
