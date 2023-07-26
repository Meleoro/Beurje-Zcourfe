using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Ennemy : MonoBehaviour
{
    [Header("GeneralDatas")] 
    public DataUnit data;
    [Min(1)] [SerializeField] int currentLevel = 1;
    public int CurrentLevel => currentLevel - 1;

    [Header("CurrentDatas")]
    [HideInInspector] public OverlayTile currentTile;
    [HideInInspector] public List<OverlayTile> currentMoveTiles = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesAttack = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesCompetence1 = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesCompetence2 = new List<OverlayTile>();
    private bool isSelected;
    [HideInInspector] public int haste;
    public int currentHealth;
    
    [Header("References")]
    private RangeFinder rangeFinder;
    private PathFinder pathFinder;
    private StatsCalculator statsCalculator;
    

    private void Start()
    {
        rangeFinder = new RangeFinder();
        pathFinder = new PathFinder();
        statsCalculator = new StatsCalculator();

        currentHealth = data.levels[CurrentLevel].PV;
    }
    

    private void Update()
    {
        if (currentTile == null)
        {
            FindCurrentTile();
            
            BattleManager.Instance.AddEnnemy(this);
            
            FindTilesAtRange();
        }
    }


    public void ActivateOutline(Color newColor)
    {
        GetComponent<SpriteRenderer>().material.SetFloat("_DoOutline", 1);

        if (newColor != null)
        {
            GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", newColor);
        }
    }

    public void DesactivateOutline()
    {
        GetComponent<SpriteRenderer>().material.SetFloat("_DoOutline", 0);
    }



    public void DoTurn()
    {
        UIBattleManager.Instance.buttonScript.SwitchButtonInteractible(false);
        FindTilesAtRange();

        List<OverlayTile> moveTileAttackTile = new List<OverlayTile>();

        bool isSummon = false;
        
        switch (data.attaqueData.levels[0].newEffet)
        {
            case DataCompetence.Effets.none :
                moveTileAttackTile =
                    rangeFinder.FindTilesAttackEnnemy(currentMoveTiles, data.attaqueData, 0, currentTile, data.levels[CurrentLevel].shyBehavior);
                break;
            
            case DataCompetence.Effets.invocation :
                moveTileAttackTile =
                    rangeFinder.FindTilesCompetenceEnnemy(currentMoveTiles, data.attaqueData, 0, currentTile, data.levels[CurrentLevel].shyBehavior, false);
                isSummon = true;
                break;
        }

        List<OverlayTile> movePath = pathFinder.FindPath(currentTile, moveTileAttackTile[0]);

        // If the ennemy move then attack
        if (moveTileAttackTile.Count == 2)
        {
            if (!isSummon)
            {
                Unit attackedUnit = BattleManager.Instance.activeUnits[(Vector2Int)moveTileAttackTile[1].posOverlayTile];
            
                StartCoroutine(MoveToTileAttack(movePath, attackedUnit, data.attaqueData));
            }

            else
            {
                StartCoroutine(MoveToTileSummon(movePath, data.attaqueData, moveTileAttackTile[1]));
            }
        }

        // If the ennemy move
        else
        {
            StartCoroutine(MoveToTile(movePath));
        }
    }


    public IEnumerator AttackUnit(Unit attackedUnit, DataCompetence competenceUsed)
    {
        List<Vector2> positions = new List<Vector2>();

        positions.Add(transform.position);
        positions.Add(attackedUnit.transform.position);

        CameraManager.Instance.EnterCameraBattle(positions, 0.7f);

        yield return new WaitForSeconds(1f);
                
        int attackHitRate = statsCalculator.CalculateHitRate(data.levels[CurrentLevel].agilite, competenceUsed.levels[0].baseHitRate,attackedUnit.data.levels[attackedUnit.CurrentLevel].agilite);
        int attackDamage = statsCalculator.CalculateDamages(data.levels[CurrentLevel].force, competenceUsed.levels[0].damageMultiplier, attackedUnit.data.levels[attackedUnit.CurrentLevel].defense);
        int attackCriticalRate = statsCalculator.CalculateCriticalRate(data.levels[CurrentLevel].chance, competenceUsed.levels[0].criticalMultiplier, attackedUnit.data.levels[attackedUnit.CurrentLevel].chance);
                
        // Si l'attaque touche
        if (Random.Range(0, 100) <= attackHitRate) 
        {
            // Si c'est un critique
            if (Random.Range(0, 100) <= attackCriticalRate) 
            {
                attackedUnit.TakeDamages(attackDamage * 2);
                StartCoroutine(UIBattleManager.Instance.attackScript.AttackUIFeel(attackedUnit.data.damageSprite, data.attackSprite, false,attackDamage * 2,false,true));
            }
            // si ce n'est pas un critique
            else 
            {
                attackedUnit.TakeDamages(attackDamage);
                StartCoroutine(UIBattleManager.Instance.attackScript.AttackUIFeel(attackedUnit.data.damageSprite, data.attackSprite, false,attackDamage,false,false)); 
            }
        }
        // Si c'est un miss
        else 
        { 
            StartCoroutine(UIBattleManager.Instance.attackScript.AttackUIFeel(attackedUnit.data.damageSprite, data.attackSprite, false,0,true,false));
        }

        yield return new WaitForSeconds(UIBattleManager.Instance.dureeAnimAttaque);
        
        UIBattleManager.Instance.UpdateTurnUI();
        StartCoroutine(BattleManager.Instance.NextTurn());
    }


    public IEnumerator SummonUnit(DataCompetence currentCompetence, OverlayTile currentTile)
    {
        List<Vector2> positions = new List<Vector2>();

        positions.Add(transform.position);
        positions.Add(currentTile.transform.position);

        CameraManager.Instance.EnterCameraBattle(positions, 0.7f);

        StartCoroutine(UIBattleManager.Instance.attackScript.SummonUIFeel(
            currentCompetence.levels[0].summonedUnit.GetComponent<Ennemy>().data.damageSprite, data.attackSprite));
        
        yield return new WaitForSeconds(UIBattleManager.Instance.dureeAnimAttaque * 0.5f);
        
        GameObject summonedUnit = currentCompetence.levels[0].summonedUnit;
        Vector2 spawnPos = currentTile.transform.position + Vector3.up * 0.5f;

        Instantiate(summonedUnit, spawnPos, Quaternion.identity);
        
        yield return new WaitForSeconds(UIBattleManager.Instance.dureeAnimAttaque * 0.5f);
        
        UIBattleManager.Instance.UpdateTurnUI();
        StartCoroutine(BattleManager.Instance.NextTurn());
    }
    

    
    // REDUCE THE HEALTH OF THE ENNEMY AND VERIFY IF HE IS DEAD
    public void TakeDamages(int damages)
    {
        currentHealth -= damages;

        if (currentHealth < 0)
        {
            BattleManager.Instance.RemoveEnnemy(this);
            Destroy(gameObject);
        }
    }
    
    
    //--------------------------MOVE PART------------------------------

    // INSTANT MOVE
    public void MoveToTile(Vector2 newPos)
    {
        transform.position = newPos + new Vector2(0, 0.4f);

        currentTile.isBlocked = true;
    }
    
    
    // MOVE WITH BREAKS 
    public IEnumerator MoveToTile(List<OverlayTile> path)
    {
        currentTile.isBlocked = false;

        for (int i = 0; i < path.Count; i++)
        {
            transform.position = path[i].transform.position + new Vector3(0, 0.4f, -1);

            transform.DOScale(new Vector3(0.75f, 1.25f, 1f), 0.04f)
                .OnComplete(() => transform.DOScale(Vector3.one, 0.04f));
            
            yield return new WaitForSeconds(0.2f);
        }
        
        currentTile = path[path.Count - 1];
        currentTile.isBlocked = true;

        StartCoroutine(BattleManager.Instance.NextTurn());
        
        BattleManager.Instance.ActualiseEnnemies();
    }
    
    
    // MOVE WITH BREAKS + ATTACK
    public IEnumerator MoveToTileAttack(List<OverlayTile> path, Unit attackedUnit, DataCompetence competenceUsed)
    {
        currentTile.isBlocked = false;
        
        // Move part
        for(int i = 0; i < path.Count; i++)
        {
            transform.position = path[i].transform.position + new Vector3(0, 0.4f, -1);

            transform.DOScale(new Vector3(0.75f, 1.25f, 1f), 0.04f)
                .OnComplete(() => transform.DOScale(Vector3.one, 0.04f));
            
            yield return new WaitForSeconds(0.2f);
        }
        
        // Attack part
        StartCoroutine(AttackUnit(attackedUnit, competenceUsed));
        
        currentTile = path[path.Count - 1];
        currentTile.isBlocked = true;
        
        BattleManager.Instance.ActualiseEnnemies();
    }
    
    
    // MOVE WITH BREAKS + SUMMON
    public IEnumerator MoveToTileSummon(List<OverlayTile> path, DataCompetence competenceUsed, OverlayTile attackedTile)
    {
        currentTile.isBlocked = false;
        
        // Move part
        for(int i = 0; i < path.Count; i++)
        {
            transform.position = path[i].transform.position + new Vector3(0, 0.4f, -1);

            transform.DOScale(new Vector3(0.75f, 1.25f, 1f), 0.04f)
                .OnComplete(() => transform.DOScale(Vector3.one, 0.04f));
            
            yield return new WaitForSeconds(0.2f);
        }
        
        // Attack part
        StartCoroutine(SummonUnit(competenceUsed, attackedTile));
        
        currentTile = path[path.Count - 1];
        currentTile.isBlocked = true;
        
        BattleManager.Instance.ActualiseEnnemies();
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
        currentMoveTiles = rangeFinder.FindMoveTilesEnnemy(currentTile, data.levels[CurrentLevel].movePatern);
    }
}
