using System;
using System.Collections;
using System.Collections.Generic;
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
    private List<Unit> activeUnitsList = new List<Unit>();
    private List<Unit> activeEnnemieslist = new List<Unit>();
    public Dictionary<Vector2Int, Unit> activeUnits = new Dictionary<Vector2Int, Unit>();
    public Dictionary<Vector2Int, Ennemy> activeEnnemies = new Dictionary<Vector2Int, Ennemy>();

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


    public void AddUnit(Unit newUnit)
    {
        activeUnits.Add(new Vector2Int(newUnit.currentTile.posOverlayTile.x, newUnit.currentTile.posOverlayTile.y), newUnit);
    }
    
    public void AddEnnemy(Ennemy newEnnemy)
    {
        activeEnnemies.Add(new Vector2Int(newEnnemy.currentTile.posOverlayTile.x, newEnnemy.currentTile.posOverlayTile.y), newEnnemy);
    }
    
    
    public void RemoveUnit(Unit unit)
    {
        activeUnits.Remove(new Vector2Int(unit.currentTile.posOverlayTile.x, unit.currentTile.posOverlayTile.y));
    }
    
    public void RemoveEnnemy(Unit ennemy)
    {
        activeEnnemies.Remove(new Vector2Int(ennemy.currentTile.posOverlayTile.x, ennemy.currentTile.posOverlayTile.y));
    }
}
