using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Unit : MonoBehaviour
{
    [Header("GeneralDatas")] 
    public DataChara data;
    
    [Header("CurrentDatas")]
    public Vector2Int startTile;
    public OverlayTile currentTile;
    public List<OverlayTile> currentTilesAtRange = new List<OverlayTile>();
    private bool isSelected;
    
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
        
        MouseManager.Instance.DisplayTilesAtRange();
    }


    // INSTANT MOVE
    public void MoveToTile(Vector2 newPos)
    {
        transform.position = newPos + new Vector2(0, 0.4f);
        
        FindTilesAtRange();
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
    }
}
