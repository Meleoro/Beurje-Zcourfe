using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [Header("Current Infos")] 
    private Vector2 originalPos;
    private float originalSize;
    

    [Header("References")] 
    private CameraManager originalScript;
    private Camera _camera;


    private void Start()
    {
        originalScript = GetComponent<CameraManager>();
        _camera = GetComponent<Camera>();
    }


    public void Zoom(float newSize, Vector2 newPos, float duration)
    {
        originalScript.canMove = false;
        originalPos = transform.position;
        originalSize = _camera.orthographicSize;

        transform.DOMove(new Vector3(newPos.x, newPos.y, -10), duration);
        _camera.DOOrthoSize(newSize, duration);
    }

    public void ExitZoom(float duration)
    {
        transform.DOMove(new Vector3(originalPos.x, originalPos.y, -10), duration);
        _camera.DOOrthoSize(originalSize, duration).OnComplete((() => originalScript.canMove = true));
    }
}
