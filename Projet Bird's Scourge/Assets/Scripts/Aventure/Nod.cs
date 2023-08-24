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
        camp
    }

    public NodeType nodeType = NodeType.battle;
    public GameObject battlePrefab;
    
    private SpriteRenderer sr;
    private EdgeCollider2D edgeCollider;
    public List<Nod> connectedNods = new List<Nod>();
    public List<Sprite> spritList = new List<Sprite>();
    public bool isCamp;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (isCamp) sr.sprite = spritList[0];
        else
        {
            sr.sprite = spritList[Random.Range(1, 7)];
        }
    }


    public void DoNodeEffect()
    {
        switch (nodeType)
        {
            case NodeType.battle :
                StartCoroutine(LaunchBattle());
                break;
        }
    }


    public void ActualiseNeighbors(Vector2 pos2)
    {
        if(edgeCollider is null)
            edgeCollider = GetComponent<EdgeCollider2D>();
        
        List<Vector2> currentPoints = edgeCollider.points.ToList();

        currentPoints.Add(Vector2.zero);
        currentPoints.Add(pos2 / transform.localScale.x);

        edgeCollider.points = currentPoints.ToArray();
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

