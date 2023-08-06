using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AventureCreator : MonoBehaviour
{
    [Header("General")]
    public List<ListSpots> map = new List<ListSpots>();
    [SerializeField] private int wantedMapLength;

    [Header("Parametres")] 
    [SerializeField] private int columnsNbr;
    [Range(50, 150)] [SerializeField] private int distanceBetweenRaws;
    [SerializeField] private int stepsBetweenCamp;
    [Range(0, 100)] [SerializeField] private int probaSpotSpawn;
    [SerializeField] private int maxElementsPerRaw;
    
    [Header("Références")]
    public Image fond;
    public GameObject spot;
    public Transform parentSpot;
    
    
    // THE WHOLE PROCESS TO GENERATE THE MAP
    public void GenerateMap()
    {
        // First we find the bounds and generate the possible spots
        List<Vector2> possibleSpots = FindPossibleSpots();

        // Then we select which spots we will use and create the map list
        ChoseSpots(possibleSpots);
        
        // Next we generate the lines on the map
        GeneratePaths();

        // Finally we initiate every nods with their functions

    }

    
    
    
    // --------------- TO FIND THE POSSIBLE SPOTS --------------- 
    
    // GENERATES THE POSSIBLE POSITIONS INTO WORLD SPACE FOR THE SPOTS
    private List<Vector2> FindPossibleSpots()
    {
        Vector2 maxBounds = GetBounds(fond.rectTransform);

        Vector2 boundsX = new Vector2(-maxBounds.x, maxBounds.x);
        Vector2 boundsY = new Vector2(-maxBounds.y, maxBounds.y);


        List<Vector2> possibleSpots = new List<Vector2>();

        for (int y = 0; y < wantedMapLength; y++)
        {
            for (int x = 0; x < columnsNbr; x++)
            {
                float posX = Mathf.Lerp(boundsX.x, boundsX.y, ((float)x / columnsNbr) + (1f / columnsNbr) * 0.5f);
                float posY = boundsY.x + distanceBetweenRaws * y;

                float posModificator = 10f;
                posX += Random.Range(-posModificator, posModificator);
                posY += Random.Range(-posModificator, posModificator);
                
                possibleSpots.Add(new Vector2(posX, posY));
            }
        }
        
        return possibleSpots;
    }

    
    // GET THE BOUNDS OF THE SPRITE ON WHICH THE SPOTS WILL BE DISPLAYED
    private Vector2 GetBounds(RectTransform currentTransform)
    {
        Vector2 finalBounds;
        
        float currentWidth = currentTransform.rect.width;
        float currentHeight = currentTransform.rect.height;
        Vector2 center = currentTransform.localPosition;

        finalBounds = new Vector2(center.x + currentWidth * 0.35f, center.y + currentHeight * 0.4f);
        
        return finalBounds;
    }
    
    
    
    // --------------- TO CHOSE WHICH SPOTS TO USE ---------------

    // SELECTS AND RETURNS EVERY SPOTS LOCATIONS 
    private void ChoseSpots(List<Vector2> possibleSpots)
    {
        int counterCamp = 0;
        int currentElementsInRaw = 0;

        int i = 0;

        for (int y = 0; y < wantedMapLength; y++)
        {
            map.Add(new ListSpots());
            
            // We go across every column in on line
            for (int x = 0; x < columnsNbr; x++)
            {
                if (VerifySpot(x, currentElementsInRaw, counterCamp <= 1))
                {
                    Nod newSpot = Instantiate(spot, possibleSpots[i], Quaternion.identity, parentSpot).GetComponent<Nod>();
                    
                    map[y].list.Add(newSpot);
                    newSpot.GetComponent<Image>().rectTransform.localPosition = possibleSpots[i];
                    newSpot.isCamp = counterCamp <= 1;

                    currentElementsInRaw += 1;
                }

                else
                {
                    map[y].list.Add(null);
                }

                i += 1;
            }
            
            // If no element has been added
            if (currentElementsInRaw == 0)
            {
                map.RemoveAt(y);
                
                y -= 1;
                i -= columnsNbr;
            }
            
            // If we can go on
            else
            {
                if (counterCamp <= 1)
                {
                    counterCamp = stepsBetweenCamp;
                }
                else
                {
                    counterCamp -= 1;
                }
                    
                currentElementsInRaw = 0;
            }
        }
    }
    
    
    // SAYS IF AN ELEMENT CAN SPAWN WITH A BIT OF RANDOM 
    private bool VerifySpot(int currentColumn, int currentElementsInRaw, bool isCamp)
    {
        if (isCamp)
        {
            if (currentColumn == (int)(columnsNbr * 0.5f))
            {
                return true;
            }
        }
        else
        {
            if (currentElementsInRaw < maxElementsPerRaw)
            {
                int tirage = Random.Range(0, 100);

                if (tirage < probaSpotSpawn)
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    
    // --------------- TO CREATE THE MAP ---------------

    private void GeneratePaths()
    {
        // Go through the whole map
        for (int y = 0; y < map.Count - 1; y++)
        {
            for (int x = 0; x < map[y].list.Count; x++)
            {
                if (map[y].list[x] is not null)
                {
                    Nod currentNod = map[y].list[x];
                    List<Nod> connectedNodes = VerifyLink(new Vector2Int(x, y), currentNod.isCamp);

                    for (int i = 0; i < connectedNodes.Count; i++)
                    {
                        currentNod.connectedNods.Add(connectedNodes[i]);
                        connectedNodes[i].connectedNods.Add(currentNod);
                    
                        Debug.DrawLine(currentNod.GetComponent<RectTransform>().position, connectedNodes[i].GetComponent<RectTransform>().position, Color.blue, 30);
                    }
                }
            }
        }
    }

    private List<Nod> VerifyLink(Vector2Int coordinates, bool isCamp)
    {
        List<Nod> linkedNodes = new List<Nod>();
        bool nextIsCamp = false;

        if (map[coordinates.y + 1].list[(int)(columnsNbr * 0.5f)] is not null)
        {
            nextIsCamp = map[coordinates.y + 1].list[(int)(columnsNbr * 0.5f)].isCamp;
        }

        if (!nextIsCamp && !isCamp)
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
                    linkedNodes.Add(map[coordinates.y + 1].list[coordinates.x - 1]);
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
            linkedNodes.Add(map[coordinates.y + 1].list[(int)(columnsNbr * 0.5f)]);
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
    
}


[Serializable]
public class ListSpots
{
    public List<Nod> list = new List<Nod>();
}
