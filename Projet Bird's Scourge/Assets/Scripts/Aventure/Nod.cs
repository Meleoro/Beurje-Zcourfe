using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


public class Nod : MonoBehaviour
{
    public enum NodeType
    {
        battle,
        camp
    }

    public NodeType nodeType = NodeType.battle;
    public GameObject battlePrefab;
    
    private SpriteRenderer sr;
    public List<Nod> connectedNods = new List<Nod>();
    public List<Sprite> spritList = new List<Sprite>();
    public bool isCamp;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (isCamp) sr.sprite = spritList[0];
        else
        {
            sr.sprite = spritList[Random.Range(1, 10)];
        }
    }


    public void DoNodeEffect()
    {
        switch (nodeType)
        {
            case NodeType.battle :
                StartCoroutine(LaunchBattle());
                break;
        }
    }

    private IEnumerator LaunchBattle()
    {
        battlePrefab = AventureManager.Instance.ChoseBattle();
 
        StartCoroutine(UIMapManager.Instance.StartBattleEffect());

        yield return new WaitForSeconds(1.2f);

        Instantiate(battlePrefab, new Vector3(300, 0, 0), Quaternion.identity);
        
        CameraManager.Instance.CameraBattleStart(BattleManager.Instance);
    }
}

