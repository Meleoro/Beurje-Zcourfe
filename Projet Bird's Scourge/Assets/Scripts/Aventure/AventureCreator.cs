using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AventureCreator : MonoBehaviour
{
    [Header("General")] 
    public AventureData data;
    private List<ListSpots> map = new List<ListSpots>();

    [Header("Update")] 
    [SerializeField] private int heightStartUpdate;    // Indicates from which number of movement we start ton update the map
    private float stockageCurrentMinY;
    private float stockageCurrentMaxY;
    private float stockageCurrentMinX;
    private float stockageCurrentMaxX;
    private int currentCounterCamp;
    private SpriteRenderer farestBackground;
    
    [Header("Références")]
    public Transform fond;
    public GameObject spot;
    public Transform parentSpot;
    private DecorationCreator decorationScript;
    private IconSelector iconScript;
    public Transform startX;
    public GameObject background;
    public GameObject backgroundEnd;

    private void Awake()
    {
        iconScript = new IconSelector();
    }


    // THE WHOLE PROCESS TO GENERATE THE MAP (RETURNS THE WHOLE MAP)
    public List<ListSpots> GenerateMap()
    {
        map = new List<ListSpots>();
        
        // First we find the bounds and generate the possible spots
        List<Vector2> possibleSpots = FindPossibleSpots();

        // Then we select which spots we will use and create the map list
        ChoseSpots(possibleSpots);
        
        // Next we generate the lines on the map
        GeneratePaths(0);

        // Finally we initiate every nods with their functions
        iconScript.ChoseIcons(map, data, 0);

        // Finally we generate the decoration of the map
        decorationScript = GetComponent<DecorationCreator>();
        decorationScript.GenerateDecoration(fond, startX.position.x - 2, startX.position.x + data.distanceBetweenColumns * (data.wantedMapLength) - 2);

        ManageBackground();

        return map;
    }



    // --------------- TO FIND THE POSSIBLE SPOTS --------------- 
    
    // GENERATES THE POSSIBLE POSITIONS INTO WORLD SPACE FOR THE SPOTS
    private List<Vector2> FindPossibleSpots()
    {
        Vector3 maxBounds = GetBounds(fond);

        stockageCurrentMinX = maxBounds.x;
        stockageCurrentMinY = maxBounds.y;
        stockageCurrentMaxY = maxBounds.z;

        List<Vector2> possibleSpots = GeneratePossibleSpots(stockageCurrentMinX, data.wantedMapLength);

        return possibleSpots;
    }


    // GENERATES THE POSSIBLE POSITIONS FOR A CERTAIN NUMBER OF ITERATIONS
    private List<Vector2> GeneratePossibleSpots(float minX, int interations)
    {
        List<Vector2> possibleSpots = new List<Vector2>();
        
        
        for (int x = 0; x < interations; x++)
        {
            for (int y = 0; y < data.rawsNbr; y++)
            {
                float posY = Mathf.Lerp(stockageCurrentMinY, stockageCurrentMaxY, ((float)y / data.rawsNbr) + (1f / data.rawsNbr) * 0.5f);
                
                float posX = minX + data.distanceBetweenColumns * x;

                float posModificator = 0.1f;
                posX += Random.Range(-posModificator, posModificator);
                posY += Random.Range(-posModificator, posModificator);
                
                possibleSpots.Add(new Vector2(posX, posY));
            }

            if (x == interations - 1)
            {
                stockageCurrentMinX = minX + data.distanceBetweenColumns * x;
            }
        }

        return possibleSpots;
    }


    // GET THE BOUNDS OF THE SPRITE ON WHICH THE SPOTS WILL BE DISPLAYED
    private Vector3 GetBounds(Transform currentTransform)
    {
        Vector3 finalBounds;
        
        float currentHeight = currentTransform.localScale.x;
        Vector2 center = currentTransform.position;

        finalBounds = new Vector3(startX.position.x, center.y - currentHeight * 1.1f, center.y + currentHeight * 1.1f);

        return finalBounds;
    }



    // --------------- TO CHOSE WHICH SPOTS TO USE ---------------

    // SELECTS AND RETURNS EVERY SPOTS LOCATIONS 
    private void ChoseSpots(List<Vector2> possibleSpots)
    {
        int currentElementsInRaw = 0;

        int columnNbr = (int)(possibleSpots.Count / data.rawsNbr);

        int i = 0;

        for (int y = 0; y < columnNbr; y++)
        {
            map.Add(new ListSpots());
            int reelY = map.Count - 1;

            // We go across every column in on line
            for (int x = 0; x < data.rawsNbr; x++)
            {
                if (VerifySpot(new Vector2Int(x, reelY), currentElementsInRaw, currentCounterCamp <= 1))
                {
                    Nod newSpot = Instantiate(spot, possibleSpots[i], Quaternion.identity, parentSpot).GetComponent<Nod>();
                    
                    map[reelY].list.Add(newSpot);
                    newSpot.transform.position = possibleSpots[i];
                    newSpot.isCamp = currentCounterCamp <= 1;

                    currentElementsInRaw += 1;

                    if (reelY == data.wantedMapLength - 1)
                    {
                        newSpot.isLast = true;
                    }
                }

                else
                {
                    map[reelY].list.Add(null);
                }

                i += 1;
            }
            
            // We check if there is no dead-end 
            bool canContinue = true;

            if (reelY - 1 >= 0)
            {
                for (int k = 0; k < map[reelY - 1].list.Count; k++)
                {
                    if (map[reelY - 1].list[k] is not null)
                    {
                        List<Nod> possibleLinks = VerifyLink(new Vector2Int(k, reelY - 1), map[reelY - 1].list[k].isCamp, map[reelY - 1].list[k].isLast);

                        if (possibleLinks.Count == 0)
                        {
                            canContinue = false;
                        }
                    }
                }
            }

            // If there is a dead-end or another problem
            if (!canContinue)
            {
                for (int k = 0; k < map[reelY].list.Count; k++)
                {
                    if (map[reelY].list[k] is not null)
                    {
                        Destroy(map[reelY].list[k].gameObject);
                    }
                }
                
                map.RemoveAt(reelY);
                
                y -= 1;
                i -= data.rawsNbr;
                currentElementsInRaw = 0;
            }
            
            // If we can go on
            else
            {
                if (currentCounterCamp <= 1)
                {
                    currentCounterCamp = data.stepsBetweenCamp;
                }
                else
                {
                    currentCounterCamp -= 1;
                }
                    
                currentElementsInRaw = 0;
            }
        }
    }
    
    
    // SAYS IF AN ELEMENT CAN SPAWN WITH A BIT OF RANDOM 
    private bool VerifySpot(Vector2Int coordinates, int currentElementsInRaw, bool isCamp)
    {
        if (isCamp)
        {
            if (coordinates.x == (int)(data.rawsNbr * 0.5f))
            {
                return true;
            }
        }
        else if (coordinates.y == data.wantedMapLength - 1)
        {
            if (coordinates.x == (int)(data.rawsNbr * 0.5f))
            {
                return true;
            }
        }
        else
        {
            if (currentElementsInRaw < data.maxElementsPerColumn)
            {
                bool hasPrecedent = false;
                
                if (coordinates.x - 1 >= 0)
                {
                    if (map[coordinates.y - 1].list[coordinates.x - 1] is not null)
                    {
                        hasPrecedent = true;
                    }
                }

                if (map[coordinates.y - 1].list[coordinates.x])
                {
                    hasPrecedent = true;
                }

                if (coordinates.x + 1 < data.maxElementsPerColumn)
                {
                    if (map[coordinates.y - 1].list[coordinates.x + 1] is not null)
                    {
                        hasPrecedent = true;
                    }
                }
                
                if (hasPrecedent)
                {
                    int tirage = Random.Range(0, 100);

                    if (tirage < data.probaSpotSpawn)
                    {
                        return true;
                    }
                }
            }
        }
        
        return false;
    }
    
    
    
    // --------------- TO CREATE THE MAP ---------------

    private void GeneratePaths(int startRaw)
    {
        // Go through the whole map
        for (int y = startRaw; y < map.Count - 1; y++)
        {
            for (int x = 0; x < map[y].list.Count; x++)
            {
                if (map[y].list[x] is not null)
                {
                    Nod currentNod = map[y].list[x];
                    List<Nod> connectedNodes = VerifyLink(new Vector2Int(x, y), currentNod.isCamp, currentNod.isLast);

                    for (int i = 0; i < connectedNodes.Count; i++)
                    {
                        currentNod.connectedNods.Add(connectedNodes[i]);
                        connectedNodes[i].connectedNods.Add(currentNod);
                        
                        //AddLine(currentNod.GetComponent<LineRenderer>(), currentNod.transform.position, connectedNodes[i].transform.position);
                        currentNod.ActualiseNeighbors(connectedNodes[i].transform.position, connectedNodes[i].transform.localPosition - currentNod.transform.localPosition);
                    }
                }
            }
        }
    }

    private void AddLine(LineRenderer currentLineRenderer, Vector3 pos1, Vector3 pos2)
    {
        currentLineRenderer.positionCount += 2;
        
        currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 2, pos1);
        currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, pos2);
    }

    private List<Nod> VerifyLink(Vector2Int coordinates, bool isCamp, bool isLast)
    {
        List<Nod> linkedNodes = new List<Nod>();
        bool nextIsCamp = false;
        bool nextIsLast = false;

        if (map[coordinates.y + 1].list[(int)(data.rawsNbr * 0.5f)] is not null)
        {
            nextIsCamp = map[coordinates.y + 1].list[(int)(data.rawsNbr * 0.5f)].isCamp;
            nextIsLast = map[coordinates.y + 1].list[(int)(data.rawsNbr * 0.5f)].isLast;
        }

        
        if (!nextIsCamp && !isCamp && !nextIsLast)
        {
            if (map[coordinates.y + 1].list[coordinates.x] is not null)
            {
                linkedNodes.Add(map[coordinates.y + 1].list[coordinates.x]);
            }

            if (coordinates.x + 1 < map[coordinates.y + 1].list.Count)
            {
                if (map[coordinates.y + 1].list[coordinates.x + 1] is not null)
                {
                    linkedNodes.Add(map[coordinates.y + 1].list[coordinates.x + 1]);
                }
            }

            if (coordinates.x - 1 >= 0)
            {
                if (map[coordinates.y + 1].list[coordinates.x - 1] is not null)
                {
                    if (map[coordinates.y + 1].list[coordinates.x] is not null)
                    {
                        if (map[coordinates.y + 1].list[coordinates.x].connectedNods.Count == 0)
                        {
                            linkedNodes.Add(map[coordinates.y + 1].list[coordinates.x - 1]);
                        }
                    }
                    else
                    {
                        linkedNodes.Add(map[coordinates.y + 1].list[coordinates.x - 1]);
                    }
                }
            }

            if (coordinates.y + 2 < map.Count)
            {
                if (map[coordinates.y + 2].list[coordinates.x] is not null && map[coordinates.y + 1].list[coordinates.x] is null)
                {
                    linkedNodes.Add(map[coordinates.y + 2].list[coordinates.x]);
                }
            }
        }

        else if (!isCamp)
        {
            linkedNodes.Add(map[coordinates.y + 1].list[(int)(data.rawsNbr * 0.5f)]);
        }

        else
        {
            for (int i = 0; i < map[coordinates.y + 1].list.Count; i++)
            {
                if (map[coordinates.y + 1].list[i] is not null)
                {
                    linkedNodes.Add(map[coordinates.y + 1].list[i]);
                }
            }
        }
        

        return linkedNodes;
    }



    // --------------- TO UPDATE THE MAP ---------------

    /*public void UpdateMap(Nod currentNod)
    {
        int playerHeight = 0;
        
        // We find the current height of the player
        for (int y = 0; y < map.Count; y++)
        {
            for (int x = 0; x < map[y].list.Count; x++)
            {
                if (map[y].list[x] == currentNod)
                {
                    playerHeight = y;
                    break;
                }
            }
            
            if(playerHeight != 0)
                break;
        }

        // If we are far enough to start to update
        if (playerHeight >= heightStartUpdate)
        {
            RemoveRaw();
            AddRaw();
        }
    }


    private void AddRaw()
    {
        // First we generate the positions for our new raw
        List<Vector2> possiblePos = GeneratePossibleSpots(stockageCurrentMinX + distanceBetweenColumns, 1);
        
        // Next we check on which positions we create nods
        ChoseSpots(possiblePos);

        // Then we add the line renderers of these elements
        GeneratePaths(map.Count - 2);
        
        ChoseIcons(map.Count - 2);

        ManageBackground();
        
        
        decorationScript.GenerateDecoration(fond, stockageCurrentMinX - distanceBetweenColumns, stockageCurrentMinX);
    }

    private void RemoveRaw()
    {
        for (int i = 0; i < map[0].list.Count; i++)
        {
            if(map[0].list[i] is not null) 
                Destroy(map[0].list[i].gameObject);
        }
        
        map.RemoveAt(0);
    }*/



    private void ManageBackground()
    {
        bool isOkay = false;

        while (!isOkay)
        {
            if (farestBackground is null)
                farestBackground = fond.GetComponent<SpriteRenderer>();
        
            float pixelWidth = farestBackground.bounds.size.x * 0.5f;
        
            if (farestBackground.transform.position.x + pixelWidth < stockageCurrentMinX)
            {
                AddBackground(farestBackground);
            }

            else
            {
                isOkay = true;
                
                EndBackground(stockageCurrentMinX - 1.2f);
            }
        }
    }

    private void AddBackground(SpriteRenderer currentBackground)
    {
        float pixelWidth = currentBackground.bounds.size.x * 0.5f;
        Vector3 posBackground = currentBackground.transform.position;

        farestBackground = Instantiate(background, posBackground + new Vector3(pixelWidth, 0, 0), Quaternion.Euler(0, 0, 90)).GetComponent<SpriteRenderer>();
    }

    private void EndBackground(float XPos)
    {
        Vector3 posBackground = new Vector3(XPos, backgroundEnd.transform.position.y, backgroundEnd.transform.position.z);

        backgroundEnd.transform.position = posBackground;
    }
}


[Serializable]
public class ListSpots
{
    public List<Nod> list = new List<Nod>();
}


[Serializable]
public class NodeTypeClass
{
    public Nod.NodeType nodType;
    [Range(0, 100)] public int percentageSpawn;
    [Range(0, 100)] public int startSpawn;
    
    [Range(-5, 5)] public int difficultyValue = 0;
}
