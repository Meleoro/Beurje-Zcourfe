using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Instance")]
    private static BattleManager _instance;
    public static BattleManager Instance
    {
        get { return _instance; }
    }
    
    [Header("Start")]
    [HideInInspector] public List<Unit> currentUnits = new List<Unit>();
    [HideInInspector] public List<Ennemy> currentEnnemies = new List<Ennemy>();
    [HideInInspector] public List<Ennemy> currentSummons = new List<Ennemy>();
    private int numberCharas;

    [Header("Units/Ennemies")]
    public Dictionary<Vector2Int, Unit> activeUnits = new Dictionary<Vector2Int, Unit>();
    public Dictionary<Vector2Int, Ennemy> activeEnnemies = new Dictionary<Vector2Int, Ennemy>();
    public Dictionary<Vector2Int, Ennemy> activeSummons = new Dictionary<Vector2Int, Ennemy>();

    [Header("Order")] 
    public List<GameObject> order;
    private bool isChangingTurn;

    [Header("Mana")] 
    public int manaMax = 5;
    public int currentMana;

    [Header("Références")] 
    public UIBattleManager UIBattle;
    public UnitSpot unitSpot1;
    public UnitSpot unitSpot2;
    public UnitSpot unitSpot3;

    [Header("BossBattle")] 
    public bool isBossBattle;
    public Ennemy bossChara;
    public int indexCharaUnlocked;

    [Header("Others")] 
    private bool doOnce;


    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (MapManager.Instance.tilesAppeared)
        {
            UIBattle.gameObject.SetActive(true);
        }

        if (currentEnnemies.Count == 0 && !doOnce)
        {
            doOnce = true;
            StartCoroutine(EndBattle());
        }
    }


    private IEnumerator EndBattle()
    {
        yield return new WaitForSeconds(2f);
        
        StartCoroutine(UIMapManager.Instance.StartBattleEffect());

        yield return new WaitForSeconds(1.2f);

        CameraManager.Instance.CameraBattleEnd();
        Destroy(gameObject);
    }


    private void VerifyBossDeath(Ennemy killedEnnemy)
    {
        if (killedEnnemy == bossChara && isBossBattle)
        {
            StartCoroutine(EndBattle());
            
            UnitSaveManager.Instance.GainUnit(indexCharaUnlocked);
        }
    }
    
    


    // ----------------------- DICTIONNARIES MANAGEMENT -----------------------

    
    public void InitialiseUnitSpots(GameObject unit1, GameObject unit2, GameObject unit3)
    {
        if (unit1 != null)
        {
            unitSpot1.SpawnUnit(unit1);
        }
        
        if (unit2 != null)
        {
            unitSpot2.SpawnUnit(unit2);
        }
        
        if (unit3 != null)
        {
            unitSpot3.SpawnUnit(unit3);
        }
    }
    

    public void AddUnit(Unit newUnit, bool summoned)
    {
        activeUnits.Add(new Vector2Int(newUnit.currentTile.posOverlayTile.x, newUnit.currentTile.posOverlayTile.y), newUnit);
        
        if(!currentUnits.Contains(newUnit))
            currentUnits.Add(newUnit);
        
        if(!summoned && activeUnits.Count + activeEnnemies.Count + activeSummons.Count >= numberCharas)
            CalculateOrder();
    }

    public void AddEnnemy(Ennemy newEnnemy, bool summoned)
    {
        activeEnnemies.Add(new Vector2Int(newEnnemy.currentTile.posOverlayTile.x, newEnnemy.currentTile.posOverlayTile.y), newEnnemy);
        
        if(!currentEnnemies.Contains(newEnnemy))
            currentEnnemies.Add(newEnnemy);
        
        if(!summoned && activeUnits.Count + activeEnnemies.Count + activeSummons.Count >= numberCharas)
            CalculateOrder();
    }
    
    public void AddSummon(Ennemy newSummon, bool summoned)
    {
        activeSummons.Add(new Vector2Int(newSummon.currentTile.posOverlayTile.x, newSummon.currentTile.posOverlayTile.y), newSummon);
        
        if(!currentSummons.Contains(newSummon))
            currentSummons.Add(newSummon);
        
        if(!summoned && activeUnits.Count + activeEnnemies.Count + activeSummons.Count >= numberCharas)
            CalculateOrder();
    }
    
    
    // IS NEEDED FOR THE START OF THE GAME (en gros les frames sur les objets instatiés au start c'est le giga bordel donc il me faut ça)
    public void AddUnitList(Unit newUnit)
    {
        currentUnits.Add(newUnit);

        numberCharas += 1;
    }
    
    // IS NEEDED FOR THE START OF THE GAME 
    public void AddEnnemyList(Ennemy newEnnemy)
    {
        currentEnnemies.Add(newEnnemy);
        
        numberCharas += 1;
    }
    
    // IS NEEDED FOR THE START OF THE GAME 
    public void AddSummonList(Ennemy newSummon)
    {
        currentSummons.Add(newSummon);
        
        numberCharas += 1;
    }
    
    
    public void RemoveUnit(Unit unit)
    {
        activeUnits.Remove(new Vector2Int(unit.currentTile.posOverlayTile.x, unit.currentTile.posOverlayTile.y));
        currentUnits.Remove(unit);

        for (int i = 0; i < order.Count; i++)
        {
            if (order[i] == unit.gameObject)
            {
                order.RemoveAt(i);
            }
        }
        
        ActualiseOrder();
    }
    
    public void RemoveEnnemy(Ennemy ennemy)
    {
        activeEnnemies.Remove(new Vector2Int(ennemy.currentTile.posOverlayTile.x, ennemy.currentTile.posOverlayTile.y));
        currentEnnemies.Remove(ennemy);
        
        for (int i = 0; i < order.Count; i++)
        {
            if (order[i] == ennemy.gameObject)
            {
                order.RemoveAt(i);
            }
        }
        
        VerifyBossDeath(ennemy);
        
        ActualiseOrder();
    }
    
    public void RemoveSummon(Ennemy summon)
    {
        activeSummons.Remove(new Vector2Int(summon.currentTile.posOverlayTile.x, summon.currentTile.posOverlayTile.y));
        currentSummons.Remove(summon);
        
        for (int i = 0; i < order.Count; i++)
        {
            if (order[i] == summon.gameObject)
            {
                order.RemoveAt(i);
            }
        }
        
        ActualiseOrder();
    }


    public void ActualiseUnits()
    {
        activeUnits.Clear();
        
        for (int i = 0; i < currentUnits.Count; i++)
        {
            activeUnits.Add((Vector2Int)currentUnits[i].currentTile.posOverlayTile, currentUnits[i]);
        }
    }
    
    public void ActualiseEnnemies()
    {
        activeEnnemies.Clear();
        
        for (int i = 0; i < currentEnnemies.Count; i++)
        {
            activeEnnemies.Add((Vector2Int)currentEnnemies[i].currentTile.posOverlayTile, currentEnnemies[i]);
        }
        
        activeSummons.Clear();
        
        for (int i = 0; i < currentSummons.Count; i++)
        {
            activeSummons.Add((Vector2Int)currentSummons[i].currentTile.posOverlayTile, currentSummons[i]);
        }
    }


    
    // ----------------------- ORDER MANAGEMENT -----------------------
    
    // Create the order of the fight in the beginning 
    public void CalculateOrder()
    {
        BuffManager.Instance.InitialiseListCharacters();
        
        int wantedLength = 4;

        order = new List<GameObject>();

        // We create the whole order list (not sorted)
        while (order.Count < wantedLength)
        {
            ActualiseHastes();

            // Then we get the characters ready to be added
            List<GameObject> newElements = new List<GameObject>();

            for (int i = 0; i < currentUnits.Count; i++)
            {
                if (currentUnits[i].haste >= 100)
                {
                    newElements.Add(currentUnits[i].gameObject);
                }
            }

            for (int i = 0; i < currentEnnemies.Count; i++)
            {
                if (currentEnnemies[i].haste >= 100)
                {
                    newElements.Add(currentEnnemies[i].gameObject);
                }
            }
            
            for (int i = 0; i < currentSummons.Count; i++)
            {
                if (currentSummons[i].haste >= 100)
                {
                    newElements.Add(currentSummons[i].gameObject);
                }
            }
            
            
            // Finaly we sort and add them
            while (newElements.Count > 0)
            {
                int maxHaste = 0;
                int indexSelected = 0;

                for (int i = 0; i < newElements.Count; i++)
                {
                    if (newElements[i].CompareTag("Unit"))
                    {
                        if (newElements[i].GetComponent<Unit>().haste >= maxHaste)
                        {
                            maxHaste = newElements[i].GetComponent<Unit>().haste;
                            indexSelected = i;
                        }
                    }
                    
                    else
                    {
                        if (newElements[i].GetComponent<Ennemy>().haste > maxHaste)
                        {
                            maxHaste = newElements[i].GetComponent<Ennemy>().haste;
                            indexSelected = i;
                        }
                    }
                }
                
                if (newElements[indexSelected].CompareTag("Unit"))
                {
                    newElements[indexSelected].GetComponent<Unit>().haste = 0;
                }
                else
                {
                    newElements[indexSelected].GetComponent<Ennemy>().haste = 0;
                }

                order.Add(newElements[indexSelected]);
                newElements.Remove(newElements[indexSelected]);
            }
        }

        UIBattleManager.Instance.UpdateTurnUI();
        StartCoroutine(FirstTurn());
    }

    
    // Actualise the order during the fight 
    public void ActualiseOrder()
    {
        int wantedLength = 4;

        // We create the whole order list (not sorted)
        while (order.Count < wantedLength)
        {
            ActualiseHastes();

            // We get the characters ready to be added
            List<GameObject> newElements = new List<GameObject>();

            for (int i = 0; i < currentUnits.Count; i++)
            {
                if (currentUnits[i].haste >= 100)
                {
                    newElements.Add(currentUnits[i].gameObject);
                }
            }

            for (int i = 0; i < currentEnnemies.Count; i++)
            {
                if (currentEnnemies[i].haste >= 100)
                {
                    newElements.Add(currentEnnemies[i].gameObject);
                }
            }
            
            for (int i = 0; i < currentSummons.Count; i++)
            {
                if (currentSummons[i].haste >= 100)
                {
                    newElements.Add(currentSummons[i].gameObject);
                }
            }
            
            
            // Finaly we sort and add them
            while (newElements.Count > 0)
            {
                int maxHaste = 0;
                int indexSelected = 0;

                for (int i = 0; i < newElements.Count; i++)
                {
                    if (newElements[i].CompareTag("Unit"))
                    {
                        if (newElements[i].GetComponent<Unit>().haste >= maxHaste)
                        {
                            maxHaste = newElements[i].GetComponent<Unit>().haste;
                            indexSelected = i;
                        }
                    }
                    
                    else
                    {
                        if (newElements[i].GetComponent<Ennemy>().haste > maxHaste)
                        {
                            maxHaste = newElements[i].GetComponent<Ennemy>().haste;
                            indexSelected = i;
                        }
                    }
                }
                
                if (newElements[indexSelected].CompareTag("Unit"))
                {
                    newElements[indexSelected].GetComponent<Unit>().haste = 0;
                }
                else
                {
                    newElements[indexSelected].GetComponent<Ennemy>().haste = 0;
                }

                order.Add(newElements[indexSelected]);
                newElements.Remove(newElements[indexSelected]);
            }
        }
        
    }

    
    // Actualise the haste of all the units and ennemies
    private void ActualiseHastes()
    {
        for (int i = 0; i < currentUnits.Count; i++)
        {
            if (currentUnits[i].haste < 100)
            {
                currentUnits[i].haste += currentUnits[i].data.levels[currentUnits[i].CurrentLevel].vitesse;
            }
        }

        for (int i = 0; i < currentEnnemies.Count; i++)
        {
            if (currentEnnemies[i].haste < 100)
            {
                currentEnnemies[i].haste += currentEnnemies[i].data.levels[currentEnnemies[i].CurrentLevel].vitesse;
            }
        }
        
        for (int i = 0; i < currentSummons.Count; i++)
        {
            if (currentSummons[i].haste < 100)
            {
                currentSummons[i].haste += currentSummons[i].data.levels[currentEnnemies[i].CurrentLevel].vitesse;
            }
        }
    }



    public IEnumerator FirstTurn()
    {
        isChangingTurn = true;
        
        StartCoroutine(UIBattleManager.Instance.NewTurnAnimation());

        yield return new WaitForSeconds(UIBattleManager.Instance.dureeAnimTour);
        
        if (order[0].CompareTag("Unit"))
        {
            order[0].GetComponent<Unit>().InitialiseTurn();
            UIBattleManager.Instance.buttonScript.SwitchButtonInteractible(true);
        }
        
        else if (order[0].CompareTag("Ennemy") || order[0].CompareTag("Summon"))
        {
            StartCoroutine(order[0].GetComponent<Ennemy>().DoTurn());
            UIBattleManager.Instance.buttonScript.SwitchButtonInteractible(false);
        }

        isChangingTurn = false;
    }


    // Go to the next turn and actualise the order
    public IEnumerator NextTurn()
    {
        isChangingTurn = true;
     
        BuffManager.Instance.NextTurn();
        MouseManager.Instance.noControl = true;

        
        if (order[0].CompareTag("Unit"))
        {
            order[0].GetComponent<Unit>().EndTurn();
            GainMana(1);
        }
        
        else if (order[0].CompareTag("Ennemy") || order[0].CompareTag("Summon"))
        {
            order[0].GetComponent<Ennemy>().DesactivateOutline();
        }
        
        MouseManager.Instance.ResetSelection();
        
        order.RemoveAt(0);
        
        ActualiseOrder();
        
        UIBattleManager.Instance.UpdateTurnUISelectedUnit(MouseManager.Instance.selectedUnit);
        UIBattleManager.Instance.UpdateTurnUI();
        StartCoroutine(UIBattleManager.Instance.NewTurnAnimation());

        yield return new WaitForSeconds(UIBattleManager.Instance.dureeAnimTour);
        
        if (order[0].CompareTag("Unit"))
        {
            order[0].GetComponent<Unit>().InitialiseTurn();
            UIBattleManager.Instance.buttonScript.SwitchButtonInteractible(true);
            
            MouseManager.Instance.noControl = false;
        }
        
        else if (order[0].CompareTag("Ennemy") || order[0].CompareTag("Summon"))
        {
            StartCoroutine(order[0].GetComponent<Ennemy>().DoTurn());
            UIBattleManager.Instance.buttonScript.SwitchButtonInteractible(false);
        }

        isChangingTurn = false;
    }

    // Exist because stupid buttons can't launch coroutines
    public void NextTurnButton()
    {
        if(!isChangingTurn)
            StartCoroutine(NextTurn());
    }
    
    
    
    // ----------------------- MANA MANAGEMENT -----------------------

    public void LoseMana(int manaLost)
    {
        currentMana -= manaLost;

        if (currentMana < 0)
            currentMana = 0;
        
        UIBattleManager.Instance.UpdateManaUI();
    }

    public void GainMana(int manaGained)
    {
        currentMana += manaGained;

        if (currentMana > manaMax)
            currentMana = manaMax;
        
        UIBattleManager.Instance.UpdateManaUI();
    }
}
