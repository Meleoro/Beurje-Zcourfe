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
    private Vector3Int max;
    private Vector3Int min;


    [Header("Références")] 
    public GameObject overlayTile;
    public Transform overlayTilesContainer;
    private Tilemap _tilemap;
    
    
    
    private void Start()
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

        max = new Vector3Int(-100, -100, 0);
        min = new Vector3Int(100, 100, 0);

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

                        if (x > max.x)
                            max = new Vector3Int(x, max.y, max.z);

                        if (y > max.y)
                            max = new Vector3Int(max.x, y, max.z);

                        if (x < min.x)
                            min = new Vector3Int(x, min.y, min.z);

                        if (y < min.y)
                            min = new Vector3Int(min.x, y, min.z);
                    }
                }
            }
        }
    }


    public IEnumerator StartEffect()
    {
        tilesAppeared = false;
        MouseManager.Instance.noControl = true;
        
        _tilemap.GetComponent<TilemapRenderer>().enabled = false;

        int wantedCount = map.Count;
        int currentCount = 0;
        
        
        Vector3Int start = new Vector3Int((max.x + min.x) / 2, (max.y + min.y) / 2, 0);
        List<Vector3Int> next = new List<Vector3Int>();
        List<Vector3Int> outPos = new List<Vector3Int>();

        CameraManager.Instance.transform.position = _tilemap.GetCellCenterWorld(start) + Vector3.back * 10;

        float stockage = CameraManager.Instance._camera.orthographicSize;
        CameraManager.Instance._camera.DOOrthoSize(2, 0);
        CameraManager.Instance._camera.DOOrthoSize(stockage, 2);

        next.Add(start);

        while (currentCount < wantedCount)
        {
            List<Vector3Int> neighbors = new List<Vector3Int>();

            for (int i = 0; i < next.Count; i++)
            {
                if (_tilemap.HasTile(next[i]))
                {
                    Vector3 currentPos = _tilemap.GetCellCenterWorld(next[i]);
                    Sprite currentSprite = _tilemap.GetSprite(next[i]);

                    GameObject currentGO = Instantiate(placeholderSprite, currentPos + Vector3.up * 1, Quaternion.identity);
                    currentGO.GetComponent<SpriteRenderer>().sprite = currentSprite;
                    currentGO.GetComponent<SpriteRenderer>().sortingOrder = -next[i].y - next[i].x;
                        
                    placeholders.Add(currentGO);

                    currentGO.transform.DOMoveY(currentGO.transform.position.y - 1, 0.3f);
                    
                    outPos.Add(next[i]);
                    currentCount += 1;
                } 
                
                neighbors.AddRange(FindNeighbors(next[i]));
            }
            
            yield return new WaitForSeconds(0.06f);

            neighbors = neighbors.Distinct().ToList();
            next = new List<Vector3Int>();

            for (int i = neighbors.Count - 1; i >= 0; i--)
            {
                if (!outPos.Contains(neighbors[i]))
                {
                    next.Add(neighbors[i]);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        _tilemap.GetComponent<TilemapRenderer>().enabled = true;

        for (int i = 0; i < placeholders.Count; i++)
        {
            Destroy(placeholders[i]);
        }


        // CHARACTER PART
        for (int i = 0; i < BattleManager.Instance.currentUnits.Count; i++)
        {
            BattleManager.Instance.currentUnits[i].Initialise();
            yield return new WaitForSeconds(0.2f);
        }
        
        for (int i = 0; i < BattleManager.Instance.currentEnnemies.Count; i++)
        {
            BattleManager.Instance.currentEnnemies[i].Initialise();
            yield return new WaitForSeconds(0.2f);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        tilesAppeared = true;
        MouseManager.Instance.noControl = false;
    }
    
    
    // RENVOIE UNE LISTE DE TOUS LES VOISINS DE LA CASE EN PARAMETRES (SI CASE NON VIDE)
    private List<Vector3Int> FindNeighbors(Vector3Int currentPos)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        // Up
        Vector3Int newPos = new Vector3Int(currentPos.x, currentPos.y + 1);
        neighbors.Add(newPos);
        
        // Down
        newPos = new Vector3Int(currentPos.x, currentPos.y - 1);
        neighbors.Add(newPos);
        
        // Left
        newPos = new Vector3Int(currentPos.x + 1, currentPos.y);
        neighbors.Add(newPos);

        // Right
        newPos = new Vector3Int(currentPos.x - 1, currentPos.y);
        neighbors.Add(newPos);
        
        return neighbors;
    }
}
