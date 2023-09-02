using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


public class Nod : MonoBehaviour
{
    public enum NodeType
    {
        none,
        battle,
        eliteBattle,
        camp,
        chest,
        events,
        shop,
        statue,
        bird,
        boss
    }

    public NodeType nodeType = NodeType.none;
    public int nodeDifficulty;
    public GameObject battlePrefab;
    
    public List<Nod> connectedNods = new List<Nod>();
    public List<Nod> previousNods = new List<Nod>();
    public List<Nod> nextNods = new List<Nod>();
    public bool isCamp;
    public bool isLast;
    public int mapY;

    [Header("References Icones")] 
    public GameObject iconBattle;
    public GameObject iconElite;
    public GameObject iconBoss;
    public GameObject iconEvent;
    public GameObject iconBenediction;
    public GameObject iconChest;
    public GameObject iconShop;
    public GameObject iconBird;
    public GameObject iconCamp;
    private SpriteRenderer currentSpriteRenderer;
    
    [Header("References")]
    public GameObject lineRenderer;
    private SpriteRenderer sr;
    private EdgeCollider2D edgeCollider;

    public void InitialiseNode(NodeType currentType, int currentDifficulty, int currentY)
    {
        nodeType = currentType;
        nodeDifficulty = currentDifficulty;

        mapY = currentY;
        
        iconBattle.SetActive(false);
        iconElite.SetActive(false);
        iconBoss.SetActive(false);
        iconEvent.SetActive(false);
        iconBenediction.SetActive(false);
        iconChest.SetActive(false);
        iconShop.SetActive(false);
        iconBird.SetActive(false);
        iconCamp.SetActive(false);
        
        ChooseNodeAppearance();
    }


    public void ChooseNodeAppearance()
    {
        if(currentSpriteRenderer is not null)
            AventureEffect.Instance.RemoveIcon(currentSpriteRenderer);
        
        switch (nodeType)
        {
            case NodeType.battle :
                iconBattle.SetActive(true);
                currentSpriteRenderer = iconBattle.GetComponent<SpriteRenderer>();
                break;
            
            case NodeType.eliteBattle :
                iconElite.SetActive(true);
                currentSpriteRenderer = iconElite.GetComponent<SpriteRenderer>();
                break;
            
            case NodeType.boss :
                iconBoss.SetActive(true);
                currentSpriteRenderer = iconBoss.GetComponent<SpriteRenderer>();
                break;
            
            case NodeType.events :
                iconEvent.SetActive(true);
                currentSpriteRenderer = iconEvent.GetComponent<SpriteRenderer>();
                break;
            
            case NodeType.statue :
                iconBenediction.SetActive(true);
                currentSpriteRenderer = iconBenediction.GetComponent<SpriteRenderer>();
                break;
            
            case NodeType.chest :
                iconChest.SetActive(true);
                currentSpriteRenderer = iconChest.GetComponent<SpriteRenderer>();
                break;
            
            case NodeType.shop :
                iconShop.SetActive(true);
                currentSpriteRenderer = iconShop.GetComponent<SpriteRenderer>();
                break;
            
            case NodeType.bird :
                iconBird.SetActive(true);
                currentSpriteRenderer = iconBird.GetComponent<SpriteRenderer>();
                break;
            
            case NodeType.camp :
                iconCamp.SetActive(true);
                currentSpriteRenderer = iconCamp.GetComponent<SpriteRenderer>();
                break;
        }
        
        
        /*float dissolveValue = 1;
        DOTween.To(() => dissolveValue, x => dissolveValue = x, 0, Random.Range(1.6f, 2.5f)).OnUpdate((() =>
            currentSpriteRenderer.material.SetFloat("_DissolveValue", dissolveValue))); */
        
        AventureEffect.Instance.AddIcon(currentSpriteRenderer);
    }


    public void DoNodeEffect()
    {
        switch (nodeType)
        {
            case NodeType.battle :
                StartCoroutine(LaunchBattle());
                break;
            case NodeType.chest :
                StartCoroutine(UIMapManager.Instance.ChestPopUp());
                break;
            case NodeType.events :
                StartCoroutine(UIMapManager.Instance.EventPopUp());
                break;
            case NodeType.camp :
                StartCoroutine(UIMapManager.Instance.CampPopUp());
                break;
            case NodeType.statue :
                StartCoroutine(UIMapManager.Instance.PopUpStatue());
                break;
            case NodeType.shop :
                StartCoroutine(UIMapManager.Instance.PopUpShop());
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
        
        /*float dissolveValue = 1;
        DOTween.To(() => dissolveValue, x => dissolveValue = x, 0, Random.Range(1.8f, 2.7f)).OnUpdate((() =>
            newLineRenderer.material.SetFloat("_DissolveValue", dissolveValue))); */
        
        AventureEffect.Instance.AddLine(newLineRenderer);
    }
    

    private IEnumerator LaunchBattle()
    {
        battlePrefab = AventureManager.Instance.ChoseBattle();
 
        StartCoroutine(UIMapManager.Instance.StartBattleEffect());

        yield return new WaitForSeconds(1.2f);

        BattleManager currentBattleManager = Instantiate(battlePrefab, new Vector3(300, 0, 0), Quaternion.identity).GetComponent<BattleManager>();
        
        currentBattleManager.InitialiseUnitSpots(AventureManager.Instance.unit1, AventureManager.Instance.unit2, AventureManager.Instance.unit3);
        
        CameraManager.Instance.CameraBattleStart(BattleManager.Instance);
    }
}

