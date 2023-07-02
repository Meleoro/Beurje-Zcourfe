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
    
    [Header("currentInfos")]
    public List<Unit> activeUnits = new List<Unit>();
    public List<Unit> activeEnnemies = new List<Unit>();


    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        
        else
            Destroy(gameObject);
    }
}
