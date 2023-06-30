using System;
using System.Collections;
using System.Collections.Generic;
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
}
