using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Unit : MonoBehaviour
{
    [Header("GeneralDatas")] 
    public DataUnit data;
    
    [Header("CurrentDatas")]
    [HideInInspector] public Vector2Int startTile;
    [HideInInspector] public OverlayTile currentTile;
    [HideInInspector] public List<OverlayTile> currentTilesAtRange = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesAttack = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesCompetence1 = new List<OverlayTile>();
    [HideInInspector] public List<OverlayTile> tilesCompetence2 = new List<OverlayTile>();
    private bool isSelected;

    [Header("ElementsToSave")] 
    public int attackLevel;
    public int competence1Level;
    public int competence2Level;
    
    [Header("References")]
    private RangeFinder rangeFinder;


    private void Start()
    {
        rangeFinder = new RangeFinder();
        
        currentTile = MapManager.Instance.map[startTile];

        MoveToTile(currentTile.transform.position);
    }


    // FIND ALL AVAILABLE TILES AT RANGE
    public void FindTilesAtRange()
    {
        currentTilesAtRange = rangeFinder.FindTilesInRange(currentTile, data.moveRange);
        
        MouseManager.Instance.ManageOverlayTiles();
    }

    // FIND ALL THE TILES TO COLOR WHEN A COMPETENCE IS SELECTED
    public void FindTilesCompetences()
    {
        if(data.attaqueData != null) 
            tilesAttack = rangeFinder.FindTilesCompetence(currentTile, data.attaqueData, attackLevel);
        
        if(data.competence1Data != null)
            tilesCompetence1 = rangeFinder.FindTilesCompetence(currentTile, data.competence1Data, competence1Level);
        
        if(data.competence2Data != null)
            tilesCompetence2 = rangeFinder.FindTilesCompetence(currentTile, data.competence2Data, competence2Level);
    }


    // INSTANT MOVE
    public void MoveToTile(Vector2 newPos)
    {
        transform.position = newPos + new Vector2(0, 0.4f);
        
        FindTilesAtRange();
        FindTilesCompetences();
    }
    
    
    // MOVE WITH BREAKS 
    public IEnumerator MoveToTile(List<OverlayTile> path)
    {
        for(int i = 0; i < path.Count; i++)
        {
            transform.position = path[i].transform.position + new Vector3(0, 0.4f, -1);

            transform.DOScale(new Vector3(0.75f, 1.25f, 1f), 0.04f)
                .OnComplete(() => transform.DOScale(Vector3.one, 0.04f));
            
            yield return new WaitForSeconds(0.2f);
        }
        
        currentTile = path[path.Count - 1];

        FindTilesAtRange();
        FindTilesCompetences();
    }
}
