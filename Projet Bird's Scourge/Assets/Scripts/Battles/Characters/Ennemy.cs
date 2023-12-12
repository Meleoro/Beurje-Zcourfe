using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Ennemy : MonoBehaviour
{
    [Header("GeneralDatas")] 
    public DataUnit data;
    [Min(1)] [SerializeField] int currentLevel = 1;
    public bool isUnitSummon;
    public bool isBoss;
    public int CurrentLevel => currentLevel - 1;
    [HideInInspector] public bool isSummoned;

    [Header("CurrentDatas")]
    [HideInInspector] public OverlayTile currentTile;
    [HideInInspector] public List<OverlayTile> currentMoveTiles = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesAttack = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesCompetence1 = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesCompetence2 = new List<OverlayTile>();
    private bool isSelected;
    [HideInInspector] public int haste;
    public int currentHealth;
    private bool outlineMustFlicker;
    private bool outlineFlickerLauched;
    
    [Header("References")]
    private RangeFinder rangeFinder;
    private PathFinder pathFinder;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer spriteRendererMaterialOutline;
    private EnnemyAttacks attackScript;
    

    private void Start()
    {
        rangeFinder = new RangeFinder();
        pathFinder = new PathFinder();

        attackScript = GetComponent<EnnemyAttacks>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = data.levels[CurrentLevel].PV;
        
        
        if(!isUnitSummon)
            BattleManager.Instance.AddEnnemyList(this);
            
        else
            BattleManager.Instance.AddSummonList(this);
        
        spriteRenderer.enabled = false;

        
        if (isBoss)
            spriteRendererMaterialOutline = GetComponentInChildren<SpriteRenderer>();
        
        else
            spriteRendererMaterialOutline = GetComponent<SpriteRenderer>();
    }


    // MAKE APPEAR THE ENNEMY
    public void Initialise()
    {
        FindCurrentTile();
        FindTilesAtRange();
            
        if(!isUnitSummon)
            BattleManager.Instance.AddEnnemy(this, isSummoned);
        
        else
            BattleManager.Instance.AddSummon(this, isSummoned);

        spriteRenderer.enabled = true;

        spriteRenderer.DOFade(0, 0);
        spriteRenderer.DOFade(1, 0.08f);

        transform.position = transform.position + Vector3.up * 2;
        transform.DOMoveY(transform.position.y - 2, 0.25f);

        transform.localScale = new Vector3(1, 0.2f, 1);
        transform.DOScaleY(1, 0.25f);
    }
    


    // EXECUTE THE ENNEMY'S TURN
    public IEnumerator DoTurn()
    {
        UIBattleManager.Instance.buttonScript.SwitchButtonInteractible(false);
        FindTilesAtRange();
        
        MouseManager.Instance.SelectEnnemy(this);

        yield return new WaitForSeconds(1);
    
        List<OverlayTile> moveTileAttackTile = new List<OverlayTile>();

        bool isCompetence = false;
        
        switch (data.attaqueData.levels[0].newEffet)
        {
            case DataCompetence.Effets.none :
                moveTileAttackTile =
                    rangeFinder.FindTilesAttackEnnemy(currentMoveTiles, data.attaqueData, 0, currentTile, data.levels[CurrentLevel].shyBehavior, isUnitSummon);
                break;
            
            case DataCompetence.Effets.invocation :
                moveTileAttackTile =
                    rangeFinder.FindTilesCompetenceEnnemy(currentMoveTiles, data.attaqueData, 0, currentTile, data.levels[CurrentLevel].shyBehavior, false);
                isCompetence = true;
                break;
            
            case DataCompetence.Effets.soin :
                moveTileAttackTile =
                    rangeFinder.FindTilesCompetenceEnnemy(currentMoveTiles, data.attaqueData, 0, currentTile, data.levels[CurrentLevel].shyBehavior, true);
                isCompetence = true;
                break;
            
            case DataCompetence.Effets.buff :
                moveTileAttackTile =
                    rangeFinder.FindTilesCompetenceEnnemy(currentMoveTiles, data.attaqueData, 0, currentTile, data.levels[CurrentLevel].shyBehavior, true);
                isCompetence = true;
                break;
        }

        List<OverlayTile> movePath = pathFinder.FindPath(currentTile, moveTileAttackTile[0], data.levels[CurrentLevel].moveDiagonal);

        // If the ennemy move then attack
        if (moveTileAttackTile.Count == 2)
        {
            if (!isCompetence)
            {
                Unit attackedUnit = null;
                Ennemy attackedEnnemy = null;
                Ennemy attackedSummon = null;

                if (!isUnitSummon)
                {
                    if (BattleManager.Instance.activeUnits.ContainsKey(
                            (Vector2Int)moveTileAttackTile[1].posOverlayTile))
                    {
                        attackedUnit = BattleManager.Instance.activeUnits[(Vector2Int)moveTileAttackTile[1].posOverlayTile];
                    }
                    else
                    {
                        attackedSummon = BattleManager.Instance.activeSummons[(Vector2Int)moveTileAttackTile[1].posOverlayTile];
                    }
                }
                
                else
                    attackedEnnemy = BattleManager.Instance.activeEnnemies[(Vector2Int)moveTileAttackTile[1].posOverlayTile];
            
                StartCoroutine(MoveToTileAttack(movePath, attackedUnit, attackedEnnemy, attackedSummon, data.attaqueData));
            }

            else
            {
                Unit aimedUnit = null;
                Ennemy aimedEnnemy = null;
                Ennemy aimedSummon = null;

                if (isUnitSummon)
                {
                    if (BattleManager.Instance.activeUnits.ContainsKey(
                            (Vector2Int)moveTileAttackTile[1].posOverlayTile))
                    {
                        aimedUnit = BattleManager.Instance.activeUnits[(Vector2Int)moveTileAttackTile[1].posOverlayTile];
                    }
                    else
                    {
                        aimedSummon = BattleManager.Instance.activeSummons[(Vector2Int)moveTileAttackTile[1].posOverlayTile];
                    }
                }
                
                else
                    aimedEnnemy = BattleManager.Instance.activeEnnemies[(Vector2Int)moveTileAttackTile[1].posOverlayTile];
                
                
                StartCoroutine(MoveToTileCompetence(movePath, data.attaqueData, moveTileAttackTile[1], aimedUnit, aimedEnnemy, aimedSummon));
            }
        }

        // If the ennemy move
        else
        {
            StartCoroutine(MoveToTile(movePath));
        }
    }
    
    
    
    // REDUCE THE HEALTH OF THE ENNEMY AND VERIFY IF HE IS DEAD
    public bool TakeDamages(int damages)
    {
        currentHealth -= damages;

        if (currentHealth <= 0)
        {
            Death();

            return true;
        }

        return false;
    }

    public void Death()
    {
        DesactivateFlicker();
        
        if(!isUnitSummon)
            BattleManager.Instance.RemoveEnnemy(this);
        
        else
            BattleManager.Instance.RemoveSummon(this);
        
        Destroy(gameObject);
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
        
        FindTilesAtRange();
    }
    
    
    // MOVE WITH BREAKS + ATTACK
    public IEnumerator MoveToTileAttack(List<OverlayTile> path, Unit attackedUnit, Ennemy attackedEnnemy, Ennemy attackedSummon, DataCompetence competenceUsed)
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
        StartCoroutine(attackScript.AttackUnit(attackedUnit, attackedEnnemy, attackedSummon, competenceUsed));
        
        currentTile = path[path.Count - 1];
        currentTile.isBlocked = true;
        
        BattleManager.Instance.ActualiseEnnemies();
        
        FindTilesAtRange();
    }
    
    
    // MOVE WITH BREAKS + SUMMON
    public IEnumerator MoveToTileCompetence(List<OverlayTile> path, DataCompetence competenceUsed, OverlayTile attackedTile, Unit aimedUnit, Ennemy aimedEnnemy, Ennemy aimedSummon)
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
        switch (data.attaqueData.levels[0].newEffet)
        {
            case DataCompetence.Effets.invocation :
                StartCoroutine(attackScript.SummonUnit(competenceUsed, attackedTile));
                break;
            
            case DataCompetence.Effets.buff :
                StartCoroutine(attackScript.BuffUnit(competenceUsed, aimedUnit, aimedEnnemy, aimedSummon));
                break;
            
            case DataCompetence.Effets.soin :
                StartCoroutine(attackScript.BuffUnit(competenceUsed, aimedUnit, aimedEnnemy, aimedSummon));
                break;
        }
        
        
        currentTile = path[path.Count - 1];
        currentTile.isBlocked = true;
        
        BattleManager.Instance.ActualiseEnnemies();
        
        FindTilesAtRange();
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
    
    
    //--------------------------SELECTION PART------------------------------
    
    public void ActivateOutline(Color newColor)
    {
        spriteRendererMaterialOutline.material.SetFloat("_DoOutline", 1);

        if (newColor != null)
        {
            spriteRendererMaterialOutline.material.SetColor("_OutlineColor", newColor);
        }
    }

    public void DesactivateOutline()
    {
        spriteRendererMaterialOutline.material.SetFloat("_DoOutline", 0);
        DesactivateFlicker();
    }

    public void ActivateFlicker()
    {
        outlineMustFlicker = true;
        
        ManageFlickerOutline();
    }

    public void DesactivateFlicker()
    {
        outlineMustFlicker = false;
        
        ManageFlickerOutline();
    }

    // MANAGES WHEN AN UNIT NEEDS ITS OUTLINE TO FLICKER
    private void ManageFlickerOutline()
    {
        if (outlineMustFlicker)
        {
            if (!outlineFlickerLauched)
            {
                outlineFlickerLauched = true;
                StartCoroutine(OutlineTurn(MouseManager.Instance.tilesAttackColorOver, MouseManager.Instance.turnOutlineSpeed));
            }
        }

        else if(outlineFlickerLauched)
        {
            StopAllCoroutines();
            outlineFlickerLauched = false;

            DesactivateOutline();
        }
    }

    // OUTLINE WHICH INDICATE THAT A UNIT MUST BE SELECTED
    private IEnumerator OutlineTurn(Color outlineColor, float flickerSpeed)
    {
        float outlineValue = 0;
        DOTween.To(() => outlineValue, x => outlineValue = x, 1, 1 / flickerSpeed);
        
        if(outlineColor != null)
        {
            spriteRendererMaterialOutline.material.SetColor("_OutlineColor", outlineColor);
        }
        
        while (outlineValue < 1)
        {
            spriteRendererMaterialOutline.material.SetFloat("_DoOutline", outlineValue);
        
            yield return new WaitForSeconds(Time.deltaTime);
        }

        yield return new WaitForSeconds(1 / flickerSpeed * 0.3f);
        
        DOTween.To(() => outlineValue, x => outlineValue = x, 0, 1 / flickerSpeed);
        
        while (outlineValue > 0)
        {
            spriteRendererMaterialOutline.material.SetFloat("_DoOutline", outlineValue);
        
            yield return new WaitForSeconds(Time.deltaTime);
        }

        StartCoroutine(OutlineTurn(outlineColor, flickerSpeed));
    }
}
