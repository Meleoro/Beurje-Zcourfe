using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [HideInInspector] public int haste;
    public int PM;
    private bool isSelected;
    private bool outlineActive;
    public bool outlineTurnLauched;
    public bool mustBeSelected;
    public bool objectFlicker;

    
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
    private SpriteRenderer spriteRenderer;


    private void Start()
    {
        rangeFinder = new RangeFinder();
        statsCalculator = new StatsCalculator();

        spriteRenderer = GetComponent<SpriteRenderer>();
        
        BattleManager.Instance.AddUnitList(this);
        
        spriteRenderer.enabled = false;
    }

    private void Update()
    {
        ManageFlickerOutline(false);
    }
    
    
    // MAKE APPEAR THE UNIT
    public void Initialise()
    {
        FindCurrentTile();
        FindTilesAtRange();
            
        BattleManager.Instance.AddUnit(this, false);

        spriteRenderer.enabled = true;

        spriteRenderer.DOFade(0, 0);
        spriteRenderer.DOFade(1, 0.08f);

        transform.position = transform.position + Vector3.up * 2;
        transform.DOMoveY(transform.position.y - 2, 0.25f);

        transform.localScale = new Vector3(0.5f, 2f, 1);
        transform.DOScale( new Vector3(1f, 1f, 1), 0.25f);
    }
    
    

    //--------------------------ATTACK PART------------------------------

    public void LaunchAttack(List<Ennemy> clickedEnnemy, List<Unit> clickedUnit, List<Ennemy> clickedSummons, List<OverlayTile> competenceTiles, DataCompetence competenceUsed, int competenceLevel)
    {
        switch (competenceUsed.levels[competenceLevel].newEffet)
        {
            case DataCompetence.Effets.none :
                if(clickedEnnemy.Count > 0)
                    StartCoroutine(AttackEnnemies(clickedEnnemy, competenceTiles, competenceUsed, competenceLevel));
                break;
            

            case DataCompetence.Effets.soin :
                if(clickedUnit.Count > 0 || clickedSummons.Count >= 0)
                    StartCoroutine(UseCompetence(clickedUnit, clickedSummons, competenceTiles, competenceUsed, competenceLevel));
                break;
            
            case DataCompetence.Effets.buff :
                if(clickedUnit.Count > 0 || clickedSummons.Count >= 0)
                    StartCoroutine(UseCompetence(clickedUnit, clickedSummons, competenceTiles, competenceUsed, competenceLevel));
                break;
        }
    }
    
    
    // VERIFY IF WE CAN ATTACK THE CLICKED ENNEMY, THEN ATTACK HIM
    public IEnumerator AttackEnnemies(List<Ennemy> clickedEnnemies, List<OverlayTile> competenceTiles, DataCompetence competenceUsed, int competenceLevel)
    {
        if (competenceUsed.levels[competenceLevel].competenceManaCost <= BattleManager.Instance.currentMana)
        {
            MouseManager.Instance.noControl = true;
            
            List<Vector2> positions = new List<Vector2>();
            
            positions.Add(transform.position); 
            positions.Add(clickedEnnemies[0].transform.position);

            StartCoroutine(CameraManager.Instance.EnterCameraBattle(positions, 0.7f, 3f));
            

            List<UIBattleAttack.CompetenceType> competenceTypes = new List<UIBattleAttack.CompetenceType>();
            List<int> damages = new List<int>();
            List<bool> deadEnnemies = new List<bool>();
            
            for (int i = 0; i < clickedEnnemies.Count; i++)
            {
                #region Toutes les blessing avec un effet avant d'attaquer -----------------------------------------------------------------
                
                BenedictionManager.instance.BlessingEffect(0,this,clickedEnnemies[i],0);
                BenedictionManager.instance.BlessingEffect(1,this,clickedEnnemies[i],0);
                BenedictionManager.instance.BlessingEffect(2,this,clickedEnnemies[i],0);
                BenedictionManager.instance.BlessingEffect(6,this,clickedEnnemies[i],0);
                #endregion

                int attackHitRate = statsCalculator.CalculateHitRate(data.levels[CurrentLevel].agilite, competenceUsed.levels[competenceLevel].baseHitRate, clickedEnnemies[i].data.levels[clickedEnnemies[i].CurrentLevel].PV);
                attackHitRate += BuffManager.Instance.GetAccuracyBuff(attackHitRate,this,null);
                int attackDamage = statsCalculator.CalculateDamages(data.levels[CurrentLevel].force, competenceUsed.levels[competenceLevel].damageMultiplier, clickedEnnemies[i].data.levels[clickedEnnemies[i].CurrentLevel].PV);
                attackDamage += BuffManager.Instance.GetDamageBuff(attackDamage,this,null);
                int attackCriticalRate = statsCalculator.CalculateCriticalRate(data.levels[CurrentLevel].chance, competenceUsed.levels[competenceLevel].criticalMultiplier, clickedEnnemies[i].data.
                    levels[clickedEnnemies[i].CurrentLevel].PV);
                attackCriticalRate += BuffManager.Instance.GetDamageBuff(attackCriticalRate,this,null);
                
                
                if (Random.Range(0, 100) <= attackHitRate) // Si l'attaque touche
                {
                    if (Random.Range(0, 100) <= attackCriticalRate) // Si c'est un critique
                    {
                        deadEnnemies.Add(clickedEnnemies[i].TakeDamages(attackDamage * 2));
                        damages.Add(attackDamage * 2);
                        
                        competenceTypes.Add(UIBattleAttack.CompetenceType.attackCrit);
                    }
                    
                    else // si ce n'est pas un critique
                    {
                        deadEnnemies.Add(clickedEnnemies[i].TakeDamages(attackDamage));
                        damages.Add(attackDamage);
                        
                        competenceTypes.Add(UIBattleAttack.CompetenceType.attack);
                    }
                }
                
                else // Si c'est un miss
                {
                    deadEnnemies.Add(false);
                    damages.Add(0);
                    
                    competenceTypes.Add(UIBattleAttack.CompetenceType.miss);
                }
                
                
                #region Toutes les blessing avec un effet après avoir attaqué -----------------------------------------------------------------
                BenedictionManager.instance.BlessingEffect(3,this,clickedEnnemies[i],attackDamage);
                BenedictionManager.instance.BlessingEffect(4,this,clickedEnnemies[i],0);
                BenedictionManager.instance.BlessingEffect(5,this,clickedEnnemies[i],0);
                #endregion
                
            }
            
            yield return new WaitForSeconds(1f);
            
            BattleManager.Instance.LoseMana(competenceUsed.levels[competenceLevel].competenceManaCost);

            List<DataUnit> currentUnits = new List<DataUnit>();
            List<DataUnit> currentEnnemies = new List<DataUnit>();
            
            currentUnits.Add(data);

            for (int i = 0; i < clickedEnnemies.Count; i++)
            {
                currentEnnemies.Add(clickedEnnemies[i].data);
            }
            
            UIBattleManager.Instance.attackScript.LaunchAttack(currentUnits, currentEnnemies, competenceTypes, true, deadEnnemies, damages, competenceUsed.VFXType, null);
            
            yield return new WaitForSeconds(2f);
            
            UIBattleManager.Instance.UpdateTurnUI();
            StartCoroutine(BattleManager.Instance.NextTurn());
        }
    }
    
    
    // VERIFY IF WE CAN BUFF / HEAL / SUMMON AN ALLY
    public IEnumerator UseCompetence(List<Unit> clickedUnits, List<Ennemy> clickedSummons, List<OverlayTile> competenceTiles, DataCompetence competenceUsed, int competenceLevel)
    {
        if (competenceUsed.levels[competenceLevel].competenceManaCost <= BattleManager.Instance.currentMana)
        {
            MouseManager.Instance.noControl = true;
             
            
            List<Vector2> positions = new List<Vector2>();

            positions.Add(transform.position);
            positions.Add(clickedUnits[0].transform.position);

            StartCoroutine(CameraManager.Instance.EnterCameraBattle(positions, 0.7f, 3f));
            
            yield return new WaitForSeconds(1f);

            Buff currentBuff = competenceUsed.levels[competenceLevel].createdBuff;

            List<UIBattleAttack.CompetenceType> currentCompetenceTypes = new List<UIBattleAttack.CompetenceType>();
            List<int> healedAmounts = new List<int>();
            List<bool> deadEnnemies = new List<bool>();
            

            for (int i = 0; i < clickedUnits.Count; i++)
            {
                switch (competenceUsed.levels[competenceLevel].newEffet)
                {
                    case DataCompetence.Effets.soin :
                        int addedPV = Mathf.Clamp(competenceUsed.levels[competenceLevel].healedPV, 0, clickedUnits[i].data.levels[clickedUnits[i].CurrentLevel].PV - clickedUnits[i].currentHealth);
                        clickedUnits[i].currentHealth += addedPV;
                        currentCompetenceTypes.Add(UIBattleAttack.CompetenceType.heal);
                        healedAmounts.Add(addedPV);
                        deadEnnemies.Add(false);
                        break;
        
                    case DataCompetence.Effets.buff :
                        List<Unit> concernedUnits = new List<Unit>();
                        currentBuff = competenceUsed.levels[competenceLevel].createdBuff;
                        concernedUnits.Add(clickedUnits[i]);
                        BuffManager.Instance.AddBuff(currentBuff.buffType, currentBuff.buffValue, currentBuff.buffDuration, false, concernedUnits, null);
                        break;
                }
            }
            
            for (int i = 0; i < clickedSummons.Count; i++)
            {
                switch (competenceUsed.levels[competenceLevel].newEffet)
                {
                    case DataCompetence.Effets.soin :
                        int addedPV = Mathf.Clamp(competenceUsed.levels[competenceLevel].healedPV, 0, clickedSummons[i].data.levels[clickedSummons[i].CurrentLevel].PV - clickedSummons[i].currentHealth);
                        clickedSummons[i].currentHealth += addedPV;
                        currentCompetenceTypes.Add(UIBattleAttack.CompetenceType.heal);
                        healedAmounts.Add(addedPV);
                        deadEnnemies.Add(false);
                        break;
        
                    case DataCompetence.Effets.buff :
                        List<Ennemy> concernedSummons = new List<Ennemy>();
                        currentBuff = competenceUsed.levels[competenceLevel].createdBuff;
                        concernedSummons.Add(clickedSummons[i]);
                        BuffManager.Instance.AddBuff(currentBuff.buffType, currentBuff.buffValue, currentBuff.buffDuration, false, null, concernedSummons);
                        break;
                }
            }
                
            
            UIBattleManager.Instance.UpdateTurnUI();
            
            List<DataUnit> currentUnits = new List<DataUnit>();
            currentUnits.Add(data);
            
            List<DataUnit> currentEnnemies = new List<DataUnit>();

            for (int i = 0; i < clickedUnits.Count; i++)
            {
                currentEnnemies.Add(clickedUnits[i].data);
            }
            
            for (int i = 0; i < clickedSummons.Count; i++)
            {
                currentEnnemies.Add(clickedSummons[i].data);
            }
            
            
            switch (competenceUsed.levels[competenceLevel].newEffet)
            {
                case DataCompetence.Effets.soin :
                    UIBattleManager.Instance.attackScript.LaunchAttack(currentUnits, currentEnnemies, currentCompetenceTypes, true, deadEnnemies, healedAmounts, competenceUsed.VFXType, currentBuff);
                    break;
        
                case DataCompetence.Effets.buff :
                    UIBattleManager.Instance.attackScript.LaunchAttack(currentUnits, currentEnnemies, UIBattleAttack.CompetenceType.buff, true, false, 0, competenceUsed.VFXType, currentBuff);
                    break;
            }
        }
    }

    
    // SHOW CHANCES TO HIT, DO A CRITICAL HIT AND THE DAMAGES
    public void DisplayBattleStats(Ennemy clickedEnnemy, DataCompetence competenceUsed, int competenceLevel)
    {
        int attackHitRate = statsCalculator.CalculateHitRate(data.levels[CurrentLevel].agilite, competenceUsed.levels[competenceLevel].baseHitRate, clickedEnnemy.data.levels[clickedEnnemy.CurrentLevel].PV);
        attackHitRate += BuffManager.Instance.GetAccuracyBuff(attackHitRate,this,null);
        int attackDamage = statsCalculator.CalculateDamages(data.levels[CurrentLevel].force, competenceUsed.levels[competenceLevel].damageMultiplier, clickedEnnemy.data.levels[clickedEnnemy.CurrentLevel].PV);
        attackDamage += BuffManager.Instance.GetDamageBuff(attackDamage,this,null);
        int attackCriticalRate = statsCalculator.CalculateCriticalRate(data.levels[CurrentLevel].chance, competenceUsed.levels[competenceLevel].criticalMultiplier, clickedEnnemy.data.levels[clickedEnnemy.CurrentLevel].PV);
        attackCriticalRate += BuffManager.Instance.GetDamageBuff(attackCriticalRate,this,null);
        
        UIBattleManager.Instance.OpenAttackPreview(attackDamage,attackHitRate,attackCriticalRate,this,clickedEnnemy);
        
        BuffManager.Instance.RemoveBenedictions(this);
    }

    
    // REDUCE THE HEALTH OF THE UNIT AND VERIFY IF HE IS DEAD
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
        BattleManager.Instance.RemoveUnit(this);
        Destroy(gameObject);
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
    public void FindTilesAtRange(bool forceReset = false, bool forceChange = false)
    {
        currentTilesAtRange = rangeFinder.FindTilesInRange(currentTile, PM);
        
        MouseManager.Instance.tilesScript.ManageOverlayTiles(forceReset, forceChange);
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
        if (mustBeSelected)
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
        
            FindTilesAtRange(true, true);
            FindTilesCompetences();
        
            BattleManager.Instance.ActualiseUnits();
        }
    }

    
    //-------------------------- SELECTION PART ------------------------------

    public void InitialiseTurn()
    {
        PM = data.levels[CurrentLevel].PM;

        mustBeSelected = true;
        
        UIBattleManager.Instance.UpdateMovePointsUI(this);
        FindTilesAtRange();
        
        MouseManager.Instance.SelectUnit(this);
        //CameraManager.Instance.SelectUnit(this);
    }

    public void EndTurn()
    {
        mustBeSelected = false;
        
        DesactivateOutline();
        DeselectUnit();
    }

    public void SelectUnit()
    {
        isSelected = true;
        
        ManageFlickerOutline(false);
    }

    public void DeselectUnit()
    {
        isSelected = false;
    }
    
    public void ActivateOutline(Color newColor)
    {
        outlineActive = true;
        ManageFlickerOutline(true);
        
        spriteRenderer.material.SetFloat("_DoOutline", 1);

        if(newColor != null)
        {
            spriteRenderer.material.SetColor("_OutlineColor", newColor);
        }
    }

    public void DesactivateOutline()
    {
        spriteRenderer.material.SetFloat("_DoOutline", 0);
        
        outlineActive = false;
    }


    // MANAGES WHEN AN UNIT NEEDS ITS OUTLINE TO FLICKER
    private void ManageFlickerOutline(bool forceStop)
    {
        if (mustBeSelected && !outlineActive)
        {
            if (!isSelected && !outlineTurnLauched)
            {
                outlineTurnLauched = true;
                
                if(!objectFlicker)
                    StartCoroutine(OutlineTurn(MouseManager.Instance.unitTurnOutlineColor, MouseManager.Instance.turnOutlineSpeed));
                
                else
                    StartCoroutine(OutlineTurn(Color.white, MouseManager.Instance.turnOutlineSpeed));
            }

            else if (isSelected && outlineTurnLauched)
            {
                StopAllCoroutines();
                outlineTurnLauched = false;
            }
        }

        else if(outlineTurnLauched || forceStop)
        {
            StopAllCoroutines();
            outlineTurnLauched = false;

            if(!outlineActive)
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
            spriteRenderer.material.SetColor("_OutlineColor", outlineColor);
        }
        
        while (outlineValue < 1)
        {
            spriteRenderer.material.SetFloat("_DoOutline", outlineValue);
        
            yield return new WaitForSeconds(Time.deltaTime);
        }

        yield return new WaitForSeconds(1 / flickerSpeed * 0.3f);
        
        DOTween.To(() => outlineValue, x => outlineValue = x, 0, 1 / flickerSpeed);
        
        while (outlineValue > 0)
        {
            spriteRenderer.material.SetFloat("_DoOutline", outlineValue);
        
            yield return new WaitForSeconds(Time.deltaTime);
        }

        StartCoroutine(OutlineTurn(outlineColor, flickerSpeed));
    }

}
