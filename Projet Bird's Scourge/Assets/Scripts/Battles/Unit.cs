using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;


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
    [HideInInspector] public int haste;

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
        MouseManager.Instance.noControl = true;
        
        if (competenceTiles.Contains(clickedEnnemy.currentTile))
        {
            if (competenceUsed.levels[competenceLevel].competenceManaCost <= BattleManager.Instance.currentMana)
            {
                List<Vector2> positions = new List<Vector2>();

                positions.Add(transform.position);
                positions.Add(clickedEnnemy.transform.position);

                CameraManager.Instance.EnterCameraBattle(positions, 0.7f);

                yield return new WaitForSeconds(1f);
                
                int attackHitRate = statsCalculator.CalculateHitRate(data.levels[currentLevel-1].agilite, competenceUsed.levels[competenceLevel].baseHitRate,clickedEnnemy.data.agilite);
                int attackDamage = statsCalculator.CalculateDamages(data.levels[currentLevel-1].force, competenceUsed.levels[competenceLevel].damageMultiplier, clickedEnnemy.data.defense);
                int attackCriticalRate = statsCalculator.CalculateCriticalRate(data.levels[currentLevel-1].chance, competenceUsed.levels[competenceLevel].criticalMultiplier, clickedEnnemy.data.chance);
                Debug.Log(attackHitRate);
                Debug.Log(attackDamage);
                Debug.Log(attackCriticalRate);
                
                if (Random.Range(0, 100) <= attackHitRate) // Si l'attaque touche
                {
                    if (Random.Range(0, 100) <= attackCriticalRate) // Si c'est un critique
                    {
                        clickedEnnemy.TakeDamages(attackDamage * 2);
                        BattleManager.Instance.LoseMana(competenceUsed.levels[competenceLevel].competenceManaCost);
                        
                        StartCoroutine(UIBattleManager.Instance.AttackUIFeel(data.attackSprite, clickedEnnemy.data.damageSprite, true,attackDamage * 2,false,true));
                    }
                    else // si ce n'est pas un critique
                    {
                        clickedEnnemy.TakeDamages(attackDamage);
                        BattleManager.Instance.LoseMana(competenceUsed.levels[competenceLevel].competenceManaCost);
            
                        StartCoroutine(UIBattleManager.Instance.AttackUIFeel(data.attackSprite, clickedEnnemy.data.damageSprite, true,attackDamage,false,false)); 
                    }
                }
                else // Si c'est un miss
                {
                    StartCoroutine(UIBattleManager.Instance.AttackUIFeel(data.attackSprite, clickedEnnemy.data.damageSprite, true,0,true,false));
                }
                UIBattleManager.Instance.UpdateTurnUI();
            }
        }
    }

    // REDUCE THE HEALTH OF THE UNIT AND VERIFY IF HE IS DEAD
    public void TakeDamages(int damages)
    {
        currentHealth -= damages;

        if (currentHealth <= 0)
        {
            BattleManager.Instance.RemoveUnit(this);
            Destroy(gameObject);
        }
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
            tilesAttack = rangeFinder.FindTilesCompetence(currentTile, data.attaqueData, attackLevel - 1);
        
        if(data.competence1Data != null)
            tilesCompetence1 = rangeFinder.FindTilesCompetence(currentTile, data.competence1Data, competence1Level - 1);
        
        if(data.competence2Data != null)
            tilesCompetence2 = rangeFinder.FindTilesCompetence(currentTile, data.competence2Data, competence2Level - 1);
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
        MouseManager.Instance.noControl = true;
        
        for(int i = 0; i < path.Count; i++)
        {
            transform.position = path[i].transform.position + new Vector3(0, 0.4f, -1);

            transform.DOScale(new Vector3(0.75f, 1.25f, 1f), 0.04f)
                .OnComplete(() => transform.DOScale(Vector3.one, 0.04f));
            
            yield return new WaitForSeconds(0.2f);
        }
        
        currentTile = path[path.Count - 1];

        MouseManager.Instance.noControl = false;
        
        FindTilesAtRange();
        FindTilesCompetences();
    }
}
