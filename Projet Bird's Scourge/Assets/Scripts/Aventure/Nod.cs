using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


public class Nod : MonoBehaviour
{
    public enum NodeType
    {
        battle,
        camp,
        chest
    }

    public NodeType nodeType = NodeType.battle;
    public GameObject battlePrefab;
    
    public List<Nod> connectedNods = new List<Nod>();
    public bool isCamp;
    
    [Header("References")]
    public List<Sprite> spritList = new List<Sprite>();
    public GameObject lineRenderer;
    private SpriteRenderer sr;
    private EdgeCollider2D edgeCollider;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (isCamp) sr.sprite = spritList[0];
        else
        {
            sr.sprite = spritList[Random.Range(1, 10)];
        }
    }


    public void DoNodeEffect()
    {
        switch (nodeType)
        {
            case NodeType.battle :
                //StartCoroutine(LaunchBattle());
                break;
            case NodeType.chest :
                StartCoroutine(UIMapManager.Instance.ChestPopUp());
                break;
        }
    }


    public void ActualiseNeighbors(Vector2 pos2, Vector2 colliderPos)
    { 
        // Collider part
        if(edgeCollider is null)
            edgeCollider = GetComponent<EdgeCollider2D>();
        
        List<Vector2> currentPoints = edgeCollider.points.ToList();

        currentPoints.Add(Vector2.zero);
        currentPoints.Add(colliderPos / transform.localScale.x);

        edgeCollider.points = currentPoints.ToArray();
        
        
        // Line Renderer part
        LineRenderer newLineRenderer = Instantiate(lineRenderer, transform.position, Quaternion.identity, transform)
            .GetComponent<LineRenderer>();

        Vector2 direction = pos2 - (Vector2)transform.position;

        Vector2 point1 = (Vector2)transform.position + direction.normalized * 1f;
        Vector2 point2 = pos2 - direction.normalized * 1f;

        newLineRenderer.positionCount = 2;
        
        newLineRenderer.SetPosition(0, point1);
        newLineRenderer.SetPosition(1, point2);
    }
    

    private IEnumerator LaunchBattle()
    {
        battlePrefab = AventureManager.Instance.ChoseBattle();
 
        StartCoroutine(UIMapManager.Instance.StartBattleEffect());

        yield return new WaitForSeconds(1.2f);

        Instantiate(battlePrefab, new Vector3(300, 0, 0), Quaternion.identity);
        
        CameraManager.Instance.CameraBattleStart(BattleManager.Instance);
    }
}

