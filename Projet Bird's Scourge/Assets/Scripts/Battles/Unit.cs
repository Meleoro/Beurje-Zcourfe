using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Unit : MonoBehaviour
{
    [Header("GeneralDatas")] 
    public DataUnit data;
    
    [Header("CurrentDatas")]
    [HideInInspector] public OverlayTile currentTile;
    [HideInInspector] public List<OverlayTile> currentTilesAtRange = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesAttack = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesCompetence1 = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesCompetence2 = new List<OverlayTile>();
    private bool isSelected;

    [Header("ElementsToSave")] 
    public int currentHealth;
    public int currentLevel;
    public int attackLevel;
    public int competence1Level;
    public int competence2Level;
    
    [Header("References")]
    private RangeFinder rangeFinder;
    private StatsCalculator statsCalculator;


    private void Start()
    {
        rangeFinder = new RangeFinder();
        statsCalculator = new StatsCalculator();
    }

    private void Update()
    {
        if (currentTile == null)
        {
            FindCurrentTile();
            
            BattleManager.Instance.AddUnit(this);
        }
    }

    //--------------------------ATTACK PART------------------------------
    
    // VERIFY IF WE CAN ATTACK THE CLICKED ENNEMY, THEN ATTACK HIM
    public IEnumerator AttackEnnemies(Ennemy clickedEnnemy, List<OverlayTile> competenceTiles, DataCompetence competenceUsed, int competenceLevel)
    {
        if (competenceTiles.Contains(clickedEnnemy.currentTile))
        {
            List<Vector2> positions = new List<Vector2>();

            positions.Add(transform.position);
            positions.Add(clickedEnnemy.transform.position);

            CameraManager.Instance.EnterCameraBattle(positions, 0.7f);

            yield return new WaitForSeconds(1f);
            
            clickedEnnemy.TakeDamages(statsCalculator.CalculateDamages(data.levels[currentLevel].force, competenceUsed.levels[competenceLevel].damageMultiplicator, 5));
            
            StartCoroutine(UIBattleManager.Instance.AttackUIFeel(data.attackSprite, clickedEnnemy.data.damageSprite, true));
        }
    }

    // REDUCE THE HEALTH OF THE UNIT AND VERIFY IF HE IS DEAD
    public void TakeDamages(int damages)
    {
        currentHealth -= damages;

        if (currentHealth < 0) ;
    }
    
    
    //--------------------------TILES PART------------------------------

    // FIND ON WHICH TILE THE CHARACTER HAS BEEN DRAG AND DROP
    public void FindCurrentTile()
    {
        Vector2 posToCheck = new Vector2(transform.position.x, transform.position.y - 0.4f);
        RaycastHit2D[] hits = Physics2D.RaycastAll(posToCheck, Vector2.zero);;
        
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.CompareTag("Tile"))
            {
                currentTile = hits[i].collider.gameObject.GetComponent<OverlayTile>();
                
                MoveToTile(hits[i].collider.gameObject.transform.position);

                return;
            }
        }
    }
    
    
    // FIND ALL AVAILABLE TILES AT RANGE
    public void FindTilesAtRange()
    {
        currentTilesAtRange = rangeFinder.FindTilesInRange(currentTile, data.moveRange);
        
        MouseManager.Instance.ManageOverlayTiles();
    }

    
    // FIND ALL THE TILES TO COLOR WHEN A COMPETENCE IS SELECTED
    public void FindTilesCompetences()
    {
        if(data.attaqueData != null) 
            tilesAttack = rangeFinder.FindTilesCompetence(currentTile, data.attaqueData, attackLevel);
        
        if(data.competence1Data != null)
            tilesCompetence1 = rangeFinder.FindTilesCompetence(currentTile, data.competence1Data, competence1Level);
        
        if(data.competence2Data != null)
            tilesCompetence2 = rangeFinder.FindTilesCompetence(currentTile, data.competence2Data, competence2Level);
    }


    //--------------------------MOVE PART------------------------------
    
    // INSTANT MOVE
    public void MoveToTile(Vector2 newPos)
    {
        transform.position = newPos + new Vector2(0, 0.4f);
        
        FindTilesAtRange();
        FindTilesCompetences();
    }
    
    
    // MOVE WITH BREAKS 
    public IEnumerator MoveToTile(List<OverlayTile> path)
    {
        for(int i = 0; i < path.Count; i++)
        {
            transform.position = path[i].transform.position + new Vector3(0, 0.4f, -1);

            transform.DOScale(new Vector3(0.75f, 1.25f, 1f), 0.04f)
                .OnComplete(() => transform.DOScale(Vector3.one, 0.04f));
            
            yield return new WaitForSeconds(0.2f);
        }
        
        currentTile = path[path.Count - 1];

        FindTilesAtRange();
        FindTilesCompetences();
    }
}
