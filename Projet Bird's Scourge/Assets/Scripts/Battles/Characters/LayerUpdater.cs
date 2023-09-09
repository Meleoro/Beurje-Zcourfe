using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerUpdate : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public int offset;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    
    void Update()
    {
        spriteRenderer.sortingOrder = (int)Mathf.Clamp(-transform.position.y * 2 + offset + 100, -100, 300);
    }
}
