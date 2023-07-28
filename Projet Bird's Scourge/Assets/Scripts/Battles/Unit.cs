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
    public int PM;

    
    [Header("ElementsToSave")] 
    public int currentHealth;
    [SerializeField] private int currentLevel;
    public int CurrentLevel{
        get { return currentLevel - 1; }
    }

    [SerializeField] private int attackLevel;
    public int AttackLevel{
        get { return attackLevel - 1; }
    }

    [SerializeField] private int competence1Level;
    public int Competence1Level{
        get { return competence1Level - 1; }
    }

    [SerializeField] private int competence2Level;
    public int Competence2Level{
        get { return competence2Level - 1; }
    }

    
    [Header("References")]
    private RangeFinder rangeFinder;
    private StatsCalculator statsCalculator;


    private void Start()
    {
        rangeFinder = new RangeFinder();
        statsCalculator = new StatsCalculator();
        
        BattleManager.Instance.AddUnitList(this);
    }

    private void Update()
    {
        if (currentTile == null)
        {
            FindCurrentTile();
            
            BattleManager.Instance.AddUnit(this, false);
            InitialiseTurn();
        }
    }


    public void ActivateOutline(Color newColor)
    {
        GetComponent<SpriteRenderer>().material.SetFloat("_DoOutline", 1);

        if(newColor != null)
        {
            GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", newColor);
        }
    }

    public void DesactivateOutline()
    {
        GetComponent<SpriteRenderer>().material.SetFloat("_DoOutline", 0);
    }


    //--------------------------ATTACK PART------------------------------

    public void LaunchAttack(Ennemy clickedEnnemy, Unit clickedUnit, List<OverlayTile> competenceTiles, DataCompetence competenceUsed, int competenceLevel)
    {
        switch (competenceUsed.levels[competenceLevel].newEffet)
        {
            case DataCompetence.Effets.none :
                StartCoroutine(AttackEnnemies(clickedEnnemy, competenceTiles, competenceUsed, competenceLevel));
                break;
            
            case DataCompetence.Effets.soin :
                StartCoroutine(UseCompetence(clickedUnit, competenceTiles, competenceUsed, competenceLevel));
                break;
        }
    }
    
    
    // VERIFY IF WE CAN ATTACK THE CLICKED ENNEMY, THEN ATTACK HIM
    public IEnumerator AttackEnnemies(Ennemy clickedEnnemy, List<OverlayTile> competenceTiles, DataCompetence competenceUsed, int competenceLevel)
    {
        MouseManager.Instance.noControl = true;
        CameraManager.Instance.canMove = false;
        
        if (competenceTiles.Contains(clickedEnnemy.currentTile))
        {
            if (competenceUsed.levels[competenceLevel].competenceManaCost <= BattleManager.Instance.currentMana)
            {
                List<Vector2> positions = new List<Vector2>();

                positions.Add(transform.position);
                positions.Add(clickedEnnemy.transform.position);

                CameraManager.Instance.EnterCameraBattle(positions, 0.7f);

                yield return new WaitForSeconds(1f);
                
                int attackHitRate = statsCalculator.CalculateHitRate(data.levels[CurrentLevel].agilite, competenceUsed.levels[competenceLevel].baseHitRate,clickedEnnemy.data.levels[clickedEnnemy.CurrentLevel].PV);
                int attackDamage = statsCalculator.CalculateDamages(data.levels[CurrentLevel].force, competenceUsed.levels[competenceLevel].damageMultiplier, clickedEnnemy.data.levels[clickedEnnemy.CurrentLevel].PV);
                int attackCriticalRate = statsCalculator.CalculateCriticalRate(data.levels[CurrentLevel].chance, competenceUsed.levels[competenceLevel].criticalMultiplier, clickedEnnemy.data.levels[clickedEnnemy.CurrentLevel].PV);
                
                if (Random.Range(0, 100) <= attackHitRate) // Si l'attaque touche
                {
                    if (Random.Range(0, 100) <= attackCriticalRate) // Si c'est un critique
                    {
                        clickedEnnemy.TakeDamages(attackDamage * 2);
                        BattleManager.Instance.LoseMana(competenceUsed.levels[competenceLevel].competenceManaCost);
                        
                        StartCoroutine(UIBattleManager.Instance.attackScript.AttackUIFeel(data.attackSprite, clickedEnnemy.data.damageSprite, true,attackDamage * 2,false,true));
                    }
                    else // si ce n'est pas un critique
                    {
                        clickedEnnemy.TakeDamages(attackDamage);
                        BattleManager.Instance.LoseMana(competenceUsed.levels[competenceLevel].competenceManaCost);
            
                        StartCoroutine(UIBattleManager.Instance.attackScript.AttackUIFeel(data.attackSprite, clickedEnnemy.data.damageSprite, true, attackDamage,false,false)); 
                    }
                }
                else // Si c'est un miss
                {
                    StartCoroutine(UIBattleManager.Instance.attackScript.AttackUIFeel(data.attackSprite, clickedEnnemy.data.damageSprite, true, 0,true,false));
                }
                
                UIBattleManager.Instance.UpdateTurnUI();
                //StartCoroutine(BattleManager.Instance.NextTurn());
            }
        }
    }
    
    
    // VERIFY IF WE CAN BUFF / HEAL / SUMMON AN ALLY
    public IEnumerator UseCompetence(Unit clickedUnit, List<OverlayTile> competenceTiles, DataCompetence competenceUsed, int competenceLevel)
    {
        MouseManager.Instance.noControl = true;
        CameraManager.Instance.canMove = false;
        
        if (competenceTiles.Contains(clickedUnit.currentTile))
        {
            if (competenceUsed.levels[competenceLevel].competenceManaCost <= BattleManager.Instance.currentMana)
            {
                List<Vector2> positions = new List<Vector2>();

                positions.Add(transform.position);
                positions.Add(clickedUnit.transform.position);

                CameraManager.Instance.EnterCameraBattle(positions, 0.7f);

                yield return new WaitForSeconds(1f);

                int addedPV = Mathf.Clamp(competenceUsed.levels[competenceLevel].healedPV, 0, clickedUnit.data.levels[clickedUnit.CurrentLevel].PV - clickedUnit.currentHealth);
                
                clickedUnit.currentHealth += addedPV;
                StartCoroutine(UIBattleManager.Instance.attackScript.HealUIFeel(data.attackSprite, clickedUnit.data.attackSprite, true, addedPV, false, false));
                
                UIBattleManager.Instance.UpdateTurnUI();
                //StartCoroutine(BattleManager.Instance.NextTurn());
            }
        }
    }

    
    // SHOW CHANCES TO HIT, DO A CRITICAL HIT AND THE DAMAGES
    public void DisplayBattleStats(Ennemy clickedEnnemy, DataCompetence competenceUsed, int competenceLevel)
    {
        int attackHitRate = statsCalculator.CalculateHitRate(data.levels[CurrentLevel].agilite, competenceUsed.levels[competenceLevel].baseHitRate,clickedEnnemy.data.levels[0].PV);
        int attackDamage = statsCalculator.CalculateDamages(data.levels[CurrentLevel].force, competenceUsed.levels[competenceLevel].damageMultiplier, clickedEnnemy.data.levels[0].PV);
        int attackCriticalRate = statsCalculator.CalculateCriticalRate(data.levels[CurrentLevel].chance, competenceUsed.levels[competenceLevel].criticalMultiplier, clickedEnnemy.data.levels[0].PV);

        UIBattleManager.Instance.OpenAttackPreview(attackDamage,attackHitRate,attackCriticalRate,this,clickedEnnemy);
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
        currentTilesAtRange = rangeFinder.FindTilesInRange(currentTile, PM);
        
        //MouseManager.Instance.ManageOverlayTiles(true);
    }

    
    // FIND ALL THE TILES TO COLOR WHEN A COMPETENCE IS SELECTED
    public void FindTilesCompetences()
    {
        if(data.attaqueData != null) 
            tilesAttack = rangeFinder.FindTilesCompetence(currentTile, data.attaqueData, AttackLevel);
        
        if(data.competence1Data != null)
            tilesCompetence1 = rangeFinder.FindTilesCompetence(currentTile, data.competence1Data, Competence1Level);
        
        if(data.competence2Data != null)
            tilesCompetence2 = rangeFinder.FindTilesCompetence(currentTile, data.competence2Data, Competence2Level);
    }


    //--------------------------MOVE PART------------------------------
    
    // INSTANT MOVE
    public void MoveToTile(Vector2 newPos)
    {
        transform.position = newPos + new Vector2(0, 0.4f);

        currentTile.isBlocked = true;
        
        FindTilesAtRange();
        FindTilesCompetences();
    }
    
    
    // MOVE WITH BREAKS 
    public IEnumerator MoveToTile(List<OverlayTile> path)
    {
        MouseManager.Instance.noControl = true;
        currentTile.isBlocked = false;
        
        for(int i = 0; i < path.Count; i++)
        {
            transform.position = path[i].transform.position + new Vector3(0, 0.4f, -1);

            transform.DOScale(new Vector3(0.75f, 1.25f, 1f), 0.04f)
                .OnComplete(() => transform.DOScale(Vector3.one, 0.04f));

            PM -= 1;
            UIBattleManager.Instance.UpdateMovePointsUI(this);
            yield return new WaitForSeconds(0.2f);
        }
        
        currentTile = path[path.Count - 1];
        currentTile.isBlocked = true;

        MouseManager.Instance.noControl = false;
        
        FindTilesAtRange();
        FindTilesCompetences();
        
        BattleManager.Instance.ActualiseUnits();
    }


    public void InitialiseTurn()
    {
        PM = data.levels[CurrentLevel].PM;
        UIBattleManager.Instance.UpdateMovePointsUI(this);
        FindTilesAtRange();
        
        MouseManager.Instance.SelectUnit(this);
        CameraManager.Instance.StartTurnUnit(this);
    }
}
