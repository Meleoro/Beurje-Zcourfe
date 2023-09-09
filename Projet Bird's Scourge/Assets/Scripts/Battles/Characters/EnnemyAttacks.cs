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
                    StartCoroutine(UIBattleManager.Instance.attackScript.AttackUIFeel(dataUnit, data, leftOrigin, attackDamage * 2,false,true, deadUnit, competenceUsed.VFXType)); 
                }
                
                else if (attackedEnnemy is not null)
                {
                    deadUnit = attackedEnnemy.TakeDamages(attackDamage * 2);
                    StartCoroutine(UIBattleManager.Instance.attackScript.AttackUIFeel(data, dataUnit, leftOrigin, attackDamage * 2,false,true, deadUnit, competenceUsed.VFXType)); 
                }
                
                else
                {
                    deadUnit = attackedSummon.TakeDamages(attackDamage * 2);
                    StartCoroutine(UIBattleManager.Instance.attackScript.AttackUIFeel(dataUnit, data, leftOrigin, attackDamage * 2,false,true, deadUnit, competenceUsed.VFXType)); 
                }
            }
            // si ce n'est pas un critique
            else 
            {
                bool deadUnit;
                if (attackedUnit is not null)
                {
                    deadUnit = attackedUnit.TakeDamages(attackDamage);
                    StartCoroutine(UIBattleManager.Instance.attackScript.AttackUIFeel(dataUnit, data, leftOrigin, attackDamage,false,false, deadUnit, competenceUsed.VFXType)); 
                }
                
                else if (attackedEnnemy is not null)
                {
                    deadUnit = attackedEnnemy.TakeDamages(attackDamage);
                    StartCoroutine(UIBattleManager.Instance.attackScript.AttackUIFeel(data, dataUnit, leftOrigin, attackDamage,false,false, deadUnit, competenceUsed.VFXType)); 
                }
                
                else
                {
                    deadUnit = attackedSummon.TakeDamages(attackDamage);
                    StartCoroutine(UIBattleManager.Instance.attackScript.AttackUIFeel(dataUnit, data, leftOrigin, attackDamage,false,false, deadUnit, competenceUsed.VFXType)); 
                }
            }
        }
        // Si c'est un miss
        else 
        {
            if (attackedUnit is not null)
            {
                StartCoroutine(UIBattleManager.Instance.attackScript.AttackUIFeel(dataUnit, data, leftOrigin, attackDamage,true ,false, false, competenceUsed.VFXType)); 
            }
                
            else if (attackedEnnemy is not null)
            {
                StartCoroutine(UIBattleManager.Instance.attackScript.AttackUIFeel(data, dataUnit, leftOrigin, attackDamage,true ,false, false, competenceUsed.VFXType)); 
            }
                
            else
            {
                StartCoroutine(UIBattleManager.Instance.attackScript.AttackUIFeel(dataUnit, data, leftOrigin, attackDamage,true ,false, false, competenceUsed.VFXType)); 
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

        StartCoroutine(UIBattleManager.Instance.attackScript.SummonUIFeel(
            currentCompetence.levels[0].summonedUnit.GetComponent<Ennemy>().data, data, false, currentCompetence.VFXType));
        
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

        if (currentCompetence.levels[0].newEffet == DataCompetence.Effets.buff)
        {
            if (attackedUnit is not null)
            {
                StartCoroutine(UIBattleManager.Instance.attackScript.BuffUIFeel(
                    data, attackedUnit.data, true, false, false,currentCompetence.VFXType, currentCompetence.levels[0].createdBuff));
                
                Buff currentBuff = currentCompetence.levels[0].createdBuff;
                List<Unit> aimedUnit = new List<Unit>();
                aimedUnit.Add(attackedUnit);

                BuffManager.Instance.AddBuff(currentBuff.buffType, currentBuff.buffValue, currentBuff.buffDuration, false,
                    aimedUnit, null);
            }
                
            else if (attackedEnnemy is not null)
            {
                StartCoroutine(UIBattleManager.Instance.attackScript.BuffUIFeel(
                    attackedEnnemy.data, data, false, false, false,currentCompetence.VFXType, currentCompetence.levels[0].createdBuff));
                
                Buff currentBuff = currentCompetence.levels[0].createdBuff;
                List<Ennemy> aimedEnnemy = new List<Ennemy>();
                aimedEnnemy.Add(attackedEnnemy);

                BuffManager.Instance.AddBuff(currentBuff.buffType, currentBuff.buffValue, currentBuff.buffDuration, false,
                    null, aimedEnnemy);
            }
                
            else
            {
                StartCoroutine(UIBattleManager.Instance.attackScript.BuffUIFeel(
                    data, attackedSummon.data, true, false, false,currentCompetence.VFXType, currentCompetence.levels[0].createdBuff));
                
                Buff currentBuff = currentCompetence.levels[0].createdBuff;
                List<Ennemy> aimedEnnemy = new List<Ennemy>();
                aimedEnnemy.Add(attackedSummon);

                BuffManager.Instance.AddBuff(currentBuff.buffType, currentBuff.buffValue, currentBuff.buffDuration, false,
                    null, aimedEnnemy);
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
