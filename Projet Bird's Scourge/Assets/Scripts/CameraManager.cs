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

    [Header("StartTurn")] 
    public float startTurnDuration;
    public Vector2 offsetPosStart;

    [Header("Movement")] 
    public bool canMove;
    public float moveSpeed;
    public float smoothMoveFactor;
    private Vector3 moveVelocity;
    
    [Header("Zoom")]
    public float zoomSpeed;
    public float smoothZoomFactor;
    public float maxZoom;
    public float minZoom;
    private float zoomVelocity;
    private float zoom;
    
    [Header("References")] 
    [HideInInspector] public Camera _camera;
    public RectTransform worldUI;

    [Header("Other")]
    [HideInInspector] public float screenWidth;
    [HideInInspector] public float screenHeight;


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

        screenHeight = _camera.pixelHeight;
        screenWidth = _camera.pixelWidth;
    }

    private void Update()
    {
        screenHeight = _camera.pixelHeight;
        screenWidth = _camera.pixelWidth;

        Move();
        Zoom();
    }

    public void Move()
    {
        if (canMove)
        {
            Vector3 newPosition = transform.position + new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeed,(Input.GetAxisRaw("Vertical")) * moveSpeed, 0);
            transform.localPosition = Vector3.SmoothDamp(transform.position,newPosition,ref moveVelocity,smoothMoveFactor);
        }
    }
    
    public void Zoom()
    {
        if (canMove)
        {
            worldUI.localScale = new Vector3(zoom,zoom,zoom)/3;
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            zoom -= scroll * zoomSpeed;
            zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
            _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, zoom, ref zoomVelocity, smoothZoomFactor);
        }
    }


    public void SelectCharacter(Unit unit, Ennemy ennemy)
    {
        canMove = false;

        Vector2 newPos = Vector2.zero;
        float newSize = 0;

        if (unit is not null)
        {
            newPos = (Vector2)unit.transform.position + offsetPosStart;
            newSize = unit.data.levels[unit.CurrentLevel].PM * 0.2f + 2.5f;
        }
        else
        {
            newPos = (Vector2)ennemy.transform.position + offsetPosStart;
            newSize = ennemy.data.levels[ennemy.CurrentLevel].PM * 0.2f + 2.5f;
        }

        transform.DOMove(new Vector3(newPos.x, newPos.y, transform.position.z), startTurnDuration).OnComplete((() => canMove = true));
        _camera.DOOrthoSize(newSize, startTurnDuration);

        zoom = newSize;
    }
    
    
    
    // MOVE THE CAMERA TO ZOOM ON ALL THE UNITS CONCERNED BY THE ATTACK
    public IEnumerator EnterCameraBattle(List<Vector2> unitsPositions, float duration, float noControlTime)
    {
        canMove = false;
        
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

        yield return new WaitForSeconds(noControlTime);
    }

    
    // RETURN TO ITS ORIGINAL POSITION
    public void ExitCameraBattle()
    {
        transform.DOMove(savePos, 0.2f);
        _camera.DOOrthoSize(saveSize, 0.2f);
        
        canMove = true;
    }
}
