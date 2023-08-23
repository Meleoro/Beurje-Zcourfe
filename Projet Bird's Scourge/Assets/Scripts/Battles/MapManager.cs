using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [Header("Instance")]
    private static MapManager _instance;
    public static MapManager Instance
    {
        get { return _instance;  }
    }


    [Header("Map Infos")] 
    public Dictionary<Vector2Int, OverlayTile> map;

    [Header("Feel")] 
    public GameObject placeholderSprite;
    private List<GameObject> placeholders = new List<GameObject>();
    [HideInInspector] public bool tilesAppeared;


    [Header("Références")] 
    public GameObject overlayTile;
    public Transform overlayTilesContainer;
    private Tilemap _tilemap;
    
    
    
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        
        else
            Destroy(gameObject);
        
        _tilemap = GetComponentInChildren<Tilemap>();
        
        InitialiseMap();
        StartCoroutine(StartEffect());
    }
    

    // CREE ET PLACE LES OVERLAYTILES (ELLES SERVENT DE COLLIDER MAIS AUSSI D'INDICATEUR EN JEU)
    public void InitialiseMap()
    {
        BoundsInt bounds = _tilemap.cellBounds;

        map = new Dictionary<Vector2Int, OverlayTile>();

        // We go through every tile of the tilemap
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                for (int z = bounds.zMin; z < bounds.zMax; z++)
                {
                    Vector3Int tilePos = new Vector3Int(x, y, z);

                    // We check if the location contains a tile, then instantiate and setup an overlay tile
                    if (_tilemap.HasTile(tilePos))
                    {
                        GameObject newOverlayTile = Instantiate(overlayTile, overlayTilesContainer);
                        Vector3 posNewOverlayTile = _tilemap.GetCellCenterWorld(tilePos);

                        newOverlayTile.GetComponent<OverlayTile>().posOverlayTile = tilePos;

                        newOverlayTile.transform.position = new Vector3(posNewOverlayTile.x, posNewOverlayTile.y,
                            posNewOverlayTile.z + 1);
                        
                        map.Add(new Vector2Int(tilePos.x, tilePos.y), newOverlayTile.GetComponent<OverlayTile>());
                    }
                }
            }
        }
    }


    public IEnumerator StartEffect()
    {
        tilesAppeared = false;
        
        BoundsInt bounds = _tilemap.cellBounds;

        _tilemap.GetComponent<TilemapRenderer>().enabled = false;
        
        // We go through every tile of the tilemap
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                for (int z = bounds.zMin; z < bounds.zMax; z++)
                {
                    Vector3Int tilePos = new Vector3Int(x, y, z);
                    
                    if (_tilemap.HasTile(tilePos))
                    {
                        Vector3 currentPos = _tilemap.GetCellCenterWorld(tilePos);
                        Sprite currentSprite = _tilemap.GetSprite(tilePos);

                        GameObject currentGO = Instantiate(placeholderSprite, currentPos + Vector3.up * 1, Quaternion.identity);
                        currentGO.GetComponent<SpriteRenderer>().sprite = currentSprite;
                        currentGO.GetComponent<SpriteRenderer>().sortingOrder = -y - x;
                        
                        placeholders.Add(currentGO);

                        currentGO.transform.DOMoveY(currentGO.transform.position.y - 1, 1);

                        yield return new WaitForSeconds(0.01f);
                    }
                }
            }
        }
        
        yield return new WaitForSeconds(1f);

        _tilemap.GetComponent<TilemapRenderer>().enabled = true;


        for (int i = 0; i < placeholders.Count; i++)
        {
            Destroy(placeholders[i]);
        }

        tilesAppeared = true;
    }
}
