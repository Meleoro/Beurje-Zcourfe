using System;
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
            sr.sprite = spritList[Random.Range(1, 7)];
        }
    }


    public void DoNodeEffect()
    {
        switch (nodeType)
        {
            case NodeType.battle :
                LaunchBattle();
                break;
        }
    }

    private void LaunchBattle()
    {
        Debug.Log(12);
    }
}

