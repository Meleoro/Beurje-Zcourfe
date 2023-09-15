using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnnemyAttacks : MonoBehaviour
{
    private Ennemy originalScript;
    
    private int currentLevel;
    private StatsCalculator statsCalculator;

    private void Start()
    {
        statsCalculator = new StatsCalculator();
        originalScript = GetComponent<Ennemy>();
    }


    // ATTACK ONE PLAYER'S UNIT
    public IEnumerator AttackUnit(Unit attackedUnit, Ennemy attackedEnnemy, Ennemy attackedSummon, DataCompetence competenceUsed)
    {
        DataUnit data = originalScript.data;
        currentLevel = originalScript.CurrentLevel;

        DataUnit dataUnit;
        OverlayTile currentTile;
        int unitLevel;
        bool leftOrigin;

        if (attackedUnit is not null)
        {
            currentTile = attackedUnit.currentTile;
            dataUnit = attackedUnit.data;
            unitLevel = attackedUnit.CurrentLevel;
            leftOrigin = false;
        }
        else if (attackedEnnemy is not null)
        {
            currentTile = attackedEnnemy.currentTile;
            dataUnit = attackedEnnemy.data;
            unitLevel = attackedEnnemy.CurrentLevel;
            leftOrigin = true;
        }
        else
        {
            currentTile = attackedSummon.currentTile;
            dataUnit = attackedSummon.data;
            unitLevel = attackedSummon.CurrentLevel;
            leftOrigin = false;
        }
        
        IntroCompetence(currentTile);

        yield return new WaitForSeconds(1.5f);
                
        int attackHitRate = statsCalculator.CalculateHitRate(data.levels[currentLevel].agilite, competenceUsed.levels[0].baseHitRate,dataUnit.levels[unitLevel].agilite);
        int attackDamage = statsCalculator.CalculateDamages(data.levels[currentLevel].force, competenceUsed.levels[0].damageMultiplier, dataUnit.levels[unitLevel].defense);
        int attackCriticalRate = statsCalculator.CalculateCriticalRate(data.levels[currentLevel].chance, competenceUsed.levels[0].criticalMultiplier, dataUnit.levels[unitLevel].chance);
        
        List<DataUnit> currentEnnemiesR = new List<DataUnit>();
        currentEnnemiesR.Add(data);
        
        List<DataUnit> currentUnits = new List<DataUnit>();
        List<DataUnit> currentSummons = new List<DataUnit>();
        List<DataUnit> currentEnnemiesL = new List<DataUnit>();
        
        if(attackedUnit is not null)
            currentUnits.Add(attackedUnit.data);
        
        if(attackedSummon is not null)
            currentSummons.Add(attackedSummon.data);
        
        if(attackedEnnemy is not null)
            currentEnnemiesL.Add(attackedEnnemy.data);
                
        // Si l'attaque touche
        if (Random.Range(0, 100) <= attackHitRate) 
        {
            // Si c'est un critique
            if (Random.Range(0, 100) <= attackCriticalRate) 
            {
                bool deadUnit;
                if (attackedUnit is not null)
                {
                    deadUnit = attackedUnit.TakeDamages(attackDamage * 2);
                    UIBattleManager.Instance.attackScript.LaunchAttack(currentUnits, currentEnnemiesR, UIBattleAttack.CompetenceType.attackCrit, false, deadUnit, attackDamage * 2, competenceUsed.VFXType, null);
                }
                
                else if (attackedEnnemy is not null)
                {
                    deadUnit = attackedEnnemy.TakeDamages(attackDamage * 2);
                    UIBattleManager.Instance.attackScript.LaunchAttack(currentEnnemiesL, currentEnnemiesR, UIBattleAttack.CompetenceType.attackCrit, false, deadUnit, attackDamage * 2, competenceUsed.VFXType, null);
                }
                
                else
                {
                    deadUnit = attackedSummon.TakeDamages(attackDamage * 2);
                    UIBattleManager.Instance.attackScript.LaunchAttack(currentSummons, currentEnnemiesR, UIBattleAttack.CompetenceType.attackCrit, false, deadUnit, attackDamage * 2, competenceUsed.VFXType, null);
                }
            }
            // si ce n'est pas un critique
            else 
            {
                bool deadUnit;
                if (attackedUnit is not null)
                {
                    deadUnit = attackedUnit.TakeDamages(attackDamage);
                    UIBattleManager.Instance.attackScript.LaunchAttack(currentUnits, currentEnnemiesR, UIBattleAttack.CompetenceType.attack, false, deadUnit, attackDamage, competenceUsed.VFXType, null);
                }
                
                else if (attackedEnnemy is not null)
                {
                    deadUnit = attackedEnnemy.TakeDamages(attackDamage);
                    UIBattleManager.Instance.attackScript.LaunchAttack(currentUnits, currentEnnemiesR, UIBattleAttack.CompetenceType.attack, false, deadUnit, attackDamage, competenceUsed.VFXType, null);
                }
                
                else
                {
                    deadUnit = attackedSummon.TakeDamages(attackDamage);
                    UIBattleManager.Instance.attackScript.LaunchAttack(currentUnits, currentEnnemiesR, UIBattleAttack.CompetenceType.attack, false, deadUnit, attackDamage, competenceUsed.VFXType, null);
                }
            }
        }
        // Si c'est un miss
        else 
        {
            if (attackedUnit is not null)
            {
                UIBattleManager.Instance.attackScript.LaunchAttack(currentUnits, currentEnnemiesR, UIBattleAttack.CompetenceType.miss, false, false, attackDamage, competenceUsed.VFXType, null);
            }
                
            else if (attackedEnnemy is not null)
            {
                UIBattleManager.Instance.attackScript.LaunchAttack(currentEnnemiesL, currentEnnemiesR, UIBattleAttack.CompetenceType.miss, false, false, attackDamage, competenceUsed.VFXType, null);
            }
                
            else
            {
                UIBattleManager.Instance.attackScript.LaunchAttack(currentSummons, currentEnnemiesR, UIBattleAttack.CompetenceType.miss, false, false, attackDamage, competenceUsed.VFXType, null);
            }
        }

        yield return new WaitForSeconds(UIBattleManager.Instance.dureeAnimAttaque);
        
        EndCompetence(currentTile);
    }


    // SUMMONS ANOTHER ENNEMY
    public IEnumerator SummonUnit(DataCompetence currentCompetence, OverlayTile currentCompetenceTile)
    {
        DataUnit data = originalScript.data;
        currentLevel = originalScript.CurrentLevel;
        
        IntroCompetence(currentCompetenceTile);
        
        yield return new WaitForSeconds(1.5f);
        
        List<DataUnit> leftDatas = new List<DataUnit>();
        leftDatas.Add(currentCompetence.levels[0].summonedUnit.GetComponent<Ennemy>().data);

        List<DataUnit> rightDatas = new List<DataUnit>();
        rightDatas.Add(data);
        
        UIBattleManager.Instance.attackScript.LaunchAttack(leftDatas, rightDatas, UIBattleAttack.CompetenceType.summon, false, false, 0, currentCompetence.VFXType, null);
        
        yield return new WaitForSeconds(UIBattleManager.Instance.dureeAnimAttaque * 0.5f);
        
        GameObject summonedEnnemy = currentCompetence.levels[0].summonedUnit;
        Vector2 spawnPos = currentCompetenceTile.transform.position + Vector3.up * 0.5f;

        GameObject currentEnnemy = Instantiate(summonedEnnemy, spawnPos, Quaternion.identity);
        currentEnnemy.GetComponent<Ennemy>().isSummoned = true;
        
        yield return new WaitForSeconds(UIBattleManager.Instance.dureeAnimAttaque * 0.5f);
        
        EndCompetence(currentCompetenceTile);
    }
    
    
    // BUFF OR HEAL ANOTHER CHARACTER
    public IEnumerator BuffUnit(DataCompetence currentCompetence, Unit attackedUnit, Ennemy attackedEnnemy, Ennemy attackedSummon)
    {
        DataUnit data = originalScript.data;
        currentLevel = originalScript.CurrentLevel;

        DataUnit dataUnit;
        OverlayTile currentTile;
        int unitLevel;
        bool leftOrigin;

        if (attackedUnit is not null)
        {
            currentTile = attackedUnit.currentTile;
            dataUnit = attackedUnit.data;
            unitLevel = attackedUnit.CurrentLevel;
            leftOrigin = false;
        }
        else if (attackedEnnemy is not null)
        {
            currentTile = attackedEnnemy.currentTile;
            dataUnit = attackedEnnemy.data;
            unitLevel = attackedEnnemy.CurrentLevel;
            leftOrigin = true;
        }
        else
        {
            currentTile = attackedSummon.currentTile;
            dataUnit = attackedSummon.data;
            unitLevel = attackedSummon.CurrentLevel;
            leftOrigin = false;
        }
        
        IntroCompetence(currentTile);
        
        yield return new WaitForSeconds(1.5f);
        
        List<DataUnit> currentEnnemiesR = new List<DataUnit>();
        currentEnnemiesR.Add(data);
        
        List<DataUnit> currentUnits = new List<DataUnit>();
        List<DataUnit> currentSummons = new List<DataUnit>();
        List<DataUnit> currentEnnemiesL = new List<DataUnit>();
        
        if(attackedUnit is not null)
            currentUnits.Add(attackedUnit.data);
        
        if(attackedSummon is not null)
            currentSummons.Add(attackedSummon.data);
        
        if(attackedEnnemy is not null)
            currentEnnemiesL.Add(attackedEnnemy.data);
        

        if (currentCompetence.levels[0].newEffet == DataCompetence.Effets.buff)
        {
            if (attackedUnit is not null)
            {
                UIBattleManager.Instance.attackScript.LaunchAttack(currentUnits, currentEnnemiesR, UIBattleAttack.CompetenceType.buff, false, false, 0, currentCompetence.VFXType, currentCompetence.levels[0].createdBuff);
                
                Buff currentBuff = currentCompetence.levels[0].createdBuff;
                List<Unit> aimedUnit = new List<Unit>();
                aimedUnit.Add(attackedUnit);

                BuffManager.Instance.AddBuff(currentBuff.buffType, currentBuff.buffValue, currentBuff.buffDuration, false,
                    aimedUnit, null);
            }
                
            else if (attackedEnnemy is not null)
            {
                UIBattleManager.Instance.attackScript.LaunchAttack(currentEnnemiesL, currentEnnemiesR, UIBattleAttack.CompetenceType.buff, false, false, 0, currentCompetence.VFXType, currentCompetence.levels[0].createdBuff);
                
                Buff currentBuff = currentCompetence.levels[0].createdBuff;
                List<Ennemy> aimedEnnemy = new List<Ennemy>();
                aimedEnnemy.Add(attackedEnnemy);

                BuffManager.Instance.AddBuff(currentBuff.buffType, currentBuff.buffValue, currentBuff.buffDuration, false,
                    null, aimedEnnemy);
            }
                
            else
            {
                UIBattleManager.Instance.attackScript.LaunchAttack(currentSummons, currentEnnemiesR, UIBattleAttack.CompetenceType.buff, false, false, 0, currentCompetence.VFXType, currentCompetence.levels[0].createdBuff);
                
                Buff currentBuff = currentCompetence.levels[0].createdBuff;
                List<Ennemy> aimedEnnemy = new List<Ennemy>();
                aimedEnnemy.Add(attackedSummon);

                BuffManager.Instance.AddBuff(currentBuff.buffType, currentBuff.buffValue, currentBuff.buffDuration, false,
                    null, aimedEnnemy);
            }
        }
        
        else if (currentCompetence.levels[0].newEffet == DataCompetence.Effets.soin)
        {
            if (attackedUnit is not null)
            {
                UIBattleManager.Instance.attackScript.LaunchAttack(currentUnits, currentEnnemiesR, UIBattleAttack.CompetenceType.heal, false, false, currentCompetence.levels[0].healedPV, currentCompetence.VFXType, null);

                attackedUnit.currentHealth += currentCompetence.levels[0].healedPV;

                if (attackedUnit.currentHealth > attackedUnit.data.levels[attackedUnit.CurrentLevel].PV)
                    attackedUnit.currentHealth = attackedUnit.data.levels[attackedUnit.CurrentLevel].PV;
            }
                
            else if (attackedEnnemy is not null)
            {
                UIBattleManager.Instance.attackScript.LaunchAttack(currentEnnemiesL, currentEnnemiesR, UIBattleAttack.CompetenceType.heal, false, false, currentCompetence.levels[0].healedPV, currentCompetence.VFXType, null);
                
                attackedEnnemy.currentHealth += currentCompetence.levels[0].healedPV;

                if (attackedEnnemy.currentHealth > attackedEnnemy.data.levels[attackedEnnemy.CurrentLevel].PV)
                    attackedEnnemy.currentHealth = attackedEnnemy.data.levels[attackedEnnemy.CurrentLevel].PV;
            }
                
            else
            {
                UIBattleManager.Instance.attackScript.LaunchAttack(currentSummons, currentEnnemiesR, UIBattleAttack.CompetenceType.heal, false, false, currentCompetence.levels[0].healedPV, currentCompetence.VFXType, null);
                
                attackedSummon.currentHealth += currentCompetence.levels[0].healedPV;

                if (attackedSummon.currentHealth > attackedSummon.data.levels[attackedSummon.CurrentLevel].PV)
                    attackedSummon.currentHealth = attackedSummon.data.levels[attackedSummon.CurrentLevel].PV;
            }
        }
        
        
        yield return new WaitForSeconds(UIBattleManager.Instance.dureeAnimAttaque);
        
        EndCompetence(currentTile);
    }
    
    

    public void IntroCompetence(OverlayTile currentCompetenceTile)
    {
        // Camera
        List<Vector2> positions = new List<Vector2>();
        
        positions.Add(transform.position);
        positions.Add(currentCompetenceTile.transform.position);
        
        StartCoroutine(CameraManager.Instance.EnterCameraBattle(positions, 0.7f, 3f));
        
        // Tile
        currentCompetenceTile.LaunchFlicker(0.5f, MouseManager.Instance.tilesAttackColorOver);
    }

    public void EndCompetence(OverlayTile currentCompetenceTile)
    {
        currentCompetenceTile.StopFlicker();
        
        UIBattleManager.Instance.UpdateTurnUI();
        StartCoroutine(BattleManager.Instance.NextTurn());
    }
}
