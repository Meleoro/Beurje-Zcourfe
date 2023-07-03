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
    
    [Header("Units/Ennemies")]
    private Dictionary<Vector2Int, Unit> activeUnits = new Dictionary<Vector2Int, Unit>();
    private Dictionary<Vector2Int, Ennemy> activeEnnemies = new Dictionary<Vector2Int, Ennemy>();
    private List<Unit> currentUnits = new List<Unit>();
    private List<Ennemy> currentEnnemies = new List<Ennemy>();

    [Header("Order")] 
    public List<GameObject> order;

    [Header("Mana")] 
    public int manaMax = 5;
    public int currentMana;

    [Header("Références")] 
    public UIBattleManager UIBattle;


    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UIBattle.gameObject.SetActive(true);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            NextTurn();
        }
    }


    // ----------------------- DICTIONNARIES MANAGEMENT -----------------------

    public void AddUnit(Unit newUnit)
    {
        activeUnits.Add(new Vector2Int(newUnit.currentTile.posOverlayTile.x, newUnit.currentTile.posOverlayTile.y), newUnit);
        currentUnits.Add(newUnit);
        
        CalculateOrder();
    }
    
    public void AddEnnemy(Ennemy newEnnemy)
    {
        activeEnnemies.Add(new Vector2Int(newEnnemy.currentTile.posOverlayTile.x, newEnnemy.currentTile.posOverlayTile.y), newEnnemy);
        currentEnnemies.Add(newEnnemy);
        
        CalculateOrder();
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
        
        CalculateOrder();
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
        
        CalculateOrder();
    }


    
    // ----------------------- ORDER MANAGEMENT -----------------------
    
    // Create the order of the fight in the beginning 
    public void CalculateOrder()
    {
        int wantedLength = 10;
        bool orderFound = false;
        
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
    }

    
    // Actualise the order during the fight 
    public void ActualiseOrder()
    {
        int wantedLength = 10;
        bool orderFound = false;

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
                currentUnits[i].haste += currentUnits[i].data.levels[currentUnits[i].currentLevel - 1].vitesse;
            }
        }

        for (int i = 0; i < currentEnnemies.Count; i++)
        {
            if (currentEnnemies[i].haste < 100)
            {
                currentEnnemies[i].haste += currentEnnemies[i].data.vitesse;
            }
        }
    }


    // Go to the next turn and actualise the order
    public void NextTurn()
    {
        order.RemoveAt(0);
        
        GainMana(1);
        UIBattleManager.Instance.UpdateTurnUI();
        ActualiseOrder();
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
