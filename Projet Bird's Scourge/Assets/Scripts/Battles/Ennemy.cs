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



    public void DoTurn()
    {
        UIBattleManager.Instance.SwitchButtonInteractible(false);
        FindTilesAtRange();

        List<OverlayTile> moveTileAttackTile =
            rangeFinder.FindTilesCompetenceEnnemy(currentMoveTiles, data.attaqueData, 0, currentTile, data.levels[CurrentLevel].shyBehavior);

        List<OverlayTile> movePath = pathFinder.FindPath(currentTile, moveTileAttackTile[0]);

        // If the ennemy move then attack
        if (moveTileAttackTile.Count == 2)
        {
            Unit attackedUnit = BattleManager.Instance.activeUnits[(Vector2Int)moveTileAttackTile[1].posOverlayTile];
            
            StartCoroutine(MoveToTileAttack(movePath, attackedUnit, data.attaqueData));
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
                
        if (Random.Range(0, 100) <= attackHitRate) // Si l'attaque touche
        {
            if (Random.Range(0, 100) <= attackCriticalRate) // Si c'est un critique
            {
                attackedUnit.TakeDamages(attackDamage * 2);
                BattleManager.Instance.LoseMana(competenceUsed.levels[0].competenceManaCost);
                        
                StartCoroutine(UIBattleManager.Instance.AttackUIFeel(attackedUnit.data.damageSprite, data.attackSprite, false,attackDamage * 2,false,true));
            }
            else // si ce n'est pas un critique
            {
                attackedUnit.TakeDamages(attackDamage);
                BattleManager.Instance.LoseMana(competenceUsed.levels[0].competenceManaCost);
            
                StartCoroutine(UIBattleManager.Instance.AttackUIFeel(attackedUnit.data.damageSprite, data.attackSprite, false,attackDamage,false,false)); 
            }
        }
        else // Si c'est un miss
        {
            StartCoroutine(UIBattleManager.Instance.AttackUIFeel(attackedUnit.data.damageSprite, data.attackSprite, false,0,true,false));
        }

        yield return new WaitForSeconds(UIBattleManager.Instance.dureeAnimAttaque);
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
        
        StartCoroutine(BattleManager.Instance.NextTurn());
        
        BattleManager.Instance.ActualiseEnnemies();
    }
    
    // MOVE WITH BREAKS 
    public IEnumerator MoveToTileAttack(List<OverlayTile> path, Unit attackedUnit, DataCompetence competenceUsed)
    {
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
