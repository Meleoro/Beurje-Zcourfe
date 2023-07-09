using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Instance")] 
    private static CameraManager _instance;
    public static CameraManager Instance
    {
        get { return _instance; }
    }

    [Header("Battle")]
    private Vector3 savePos;
    private float saveSize;

    [Header("Movement")] 
    public bool canMove;
    public float moveSpeed;
    public float smoothMoveFactor;
    private Vector3 moveVelocity;
    
    [Header("Zoom")] 
    public bool canZoom;
    public float zoomSpeed;
    public float smoothZoomFactor;
    public float maxZoom;
    public float minZoom;
    private float zoomVelocity;
    private float zoom;
    
    [Header("References")] 
    private Camera _camera;
    public RectTransform worldUI;


    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _camera = GetComponent<Camera>();
        zoom = _camera.orthographicSize;
    }

    private void Update()
    {
        Move();
        Zoom();
    }

    public void Move()
    {
        if (canMove)
        {
            Vector3 newPosition = transform.position + new Vector3(Input.GetAxisRaw("Horizontal")*moveSpeed,(Input.GetAxisRaw("Vertical")) * moveSpeed,0);
            transform.localPosition = Vector3.SmoothDamp(transform.position,newPosition,ref moveVelocity,smoothMoveFactor);
        }
    }
    
    public void Zoom()
    {
        if (canZoom)
        {
            worldUI.localScale = new Vector3(zoom,zoom,zoom)/3;
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            zoom -= scroll * zoomSpeed;
            zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
            _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, zoom,ref zoomVelocity,smoothZoomFactor);
        }
    }
    
    // MOVE THE CAMERA TO ZOOM ON ALL THE UNITS CONCERNED BY THE ATTACK
    public void EnterCameraBattle(List<Vector2> unitsPositions, float duration)
    {
        savePos = transform.position;
        saveSize = _camera.orthographicSize;
        
        Vector2 newPos = Vector3.zero;
        float newSize = saveSize / 1.5f;

        for (int i = 0; i < unitsPositions.Count; i++)
        {
            newPos += unitsPositions[i];
        }

        newPos /= unitsPositions.Count;
        

        transform.DOMove(new Vector3(newPos.x, newPos.y, transform.position.z), duration);
        _camera.DOOrthoSize(newSize, duration);
    }

    
    // RETURN TO ITS ORIGINAL POSITION
    public void ExitCameraBattle()
    {
        transform.DOMove(savePos, 0.2f);
        _camera.DOOrthoSize(saveSize, 0.2f);
    }
}
