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


    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        
        else
            Destroy(gameObject);
    }


    public void AddUnit(Unit newUnit)
    {
        activeUnits.Add(new Vector2Int(newUnit.currentTile.posOverlayTile.x, newUnit.currentTile.posOverlayTile.y), newUnit);
    }
    
    public void AddEnnemy(Ennemy newUnit)
    {
        activeEnnemies.Add(new Vector2Int(newUnit.currentTile.posOverlayTile.x, newUnit.currentTile.posOverlayTile.y), newUnit);
    }
}
