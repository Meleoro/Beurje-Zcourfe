using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class CameraManager : MonoBehaviour
{
    [Header("Instance")] 
    private static CameraManager _instance;
    public static CameraManager Instance
    {
        get { return _instance; }
    }
    
    [Header("CameraStates")]
    public bool isInAdventure;
    public bool isInGlobal;

    [Header("Aventure")]
    public Vector3 savePosAdventure;
    private Vector3 originalPos;
    private Vector3 wantedPosShake;
    private float timerShake;

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
    public Light2D FXAventure;
    public Light2D FXBattle;
    public Transform cameraParent;

    [Header("Other")]
    [HideInInspector] public float screenWidth;
    [HideInInspector] public float screenHeight;


    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        
        else
            Destroy(gameObject);
        
        _camera = GetComponent<Camera>();
    }

    private void Start()
    {
        zoom = _camera.orthographicSize;

        screenHeight = _camera.pixelHeight;
        screenWidth = _camera.pixelWidth;

        /*if(cameraParent is not null)
            originalPos = cameraParent.transform.position;

        StartCoroutine(ShakeExploration());*/
    }

    private void Update()
    {
        screenHeight = _camera.scaledPixelHeight;
        screenWidth = _camera.scaledPixelWidth;

        if (canMove && !isInAdventure && !isInGlobal)
        {
            Move();
            Zoom();
        }

        if (isInAdventure || isInGlobal)
        {
            FXAventure.gameObject.SetActive(true);
            FXBattle.gameObject.SetActive(false);
        }
        else
        {
            FXAventure.gameObject.SetActive(false);
            FXBattle.gameObject.SetActive(true);
        }
    }

    public void Move()
    {
        Vector3 newPosition = transform.position + new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeed,(Input.GetAxisRaw("Vertical")) * moveSpeed, 0);
        transform.localPosition = Vector3.SmoothDamp(transform.position,newPosition,ref moveVelocity,smoothMoveFactor);
    }
    
    public void Zoom()
    {
        worldUI.localScale = new Vector3(zoom,zoom,zoom)/3;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoom -= scroll * zoomSpeed;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, zoom, ref zoomVelocity, smoothZoomFactor);
    }
    
    
    // -------------------------- GENERAL PART --------------------------------

    public IEnumerator TransitionAventure(Vector3 newPos, bool fromGlobalMap)
    {
        isInAdventure = true;
        isInGlobal = false;

        if (fromGlobalMap)
        {
            _camera.DOOrthoSize(_camera.orthographicSize + 2, 1.5f).SetEase(Ease.InSine);
        
            float currentIntensity = 1.5f;
            float wantedIntensity = 0;

            FXAventure.intensity = 1.5f;

            DOTween.To(() => currentIntensity, x => currentIntensity = x, wantedIntensity, 2).OnUpdate(() =>
            {
                FXAventure.intensity = currentIntensity;
            }); 
            
            yield return new WaitForSeconds(2.2f);
        }
        //StartCoroutine(AventureEffect.Instance.AppearEffect());

        transform.position = newPos;

        _camera.DOOrthoSize(12f, 0);
        _camera.DOOrthoSize(10.8f, 1.5f).SetEase(Ease.OutSine);

        float currentIntensity2 = 0;
        float wantedIntensity2 = 1.5f;

        FXAventure.intensity = 0;

        DOTween.To(() => currentIntensity2, x => currentIntensity2 = x, wantedIntensity2, 2).OnUpdate(() =>
        {
            FXAventure.intensity = currentIntensity2;
        }); 
    }
    
    public void EnterGlobal(Vector3 newPos, float newSize)
    {
        isInAdventure = false;
        isInGlobal = true;

        transform.position = newPos;
        _camera.orthographicSize = newSize;
    }
    


    // --------------------------  EXPLORATION PART  ------------------------------

    public void CameraBattleStart(BattleManager currentBattle)
    {
        DOTween.Kill(transform);
        
        savePosAdventure = transform.position;
        transform.position = new Vector3(currentBattle.transform.position.x, currentBattle.transform.position.y, -10);

        worldUI = WorldUIManager.Instance.GetComponent<RectTransform>();
        isInAdventure = false;

        FXAventure.gameObject.SetActive(false);
        FXBattle.gameObject.SetActive(true);
    }

    public void CameraBattleEnd()
    {
        //Destroy(BattleManager.Instance.gameObject);
        
        transform.position = savePosAdventure;
        isInAdventure = true;
        
        FXAventure.gameObject.SetActive(true);
        FXBattle.gameObject.SetActive(false);

        StartCoroutine(TransitionAventure(savePosAdventure, false));
    }


    /*public IEnumerator ShakeExploration()
    {
        if (timerShake < 0)
        {
            timerShake = 1.5f;

            DOTween.To(() => wantedPosShake.x, x => wantedPosShake.x = x, originalPos.x + Random.Range(-0.25f, 0.25f), 1.4f);
            DOTween.To(() => wantedPosShake.y, x => wantedPosShake.y = x, originalPos.y + Random.Range(-0.25f, 0.25f), 1.4f);
        }

        yield return new WaitForEndOfFrame();

        timerShake -= Time.deltaTime;
        cameraParent.transform.position = Vector3.Lerp(cameraParent.transform.position, wantedPosShake, Time.deltaTime);

        StartCoroutine(ShakeExploration());
    }*/
    
    
    
    // --------------------------  BATTLE PART  ------------------------------
    
    
    // WHEN YOU SELECT A CHARACTER IN BATTLE
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
