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
    public Image test;
    public Transform parentTest;
    
    public void GenerateMap()
    {
        // First we find the bounds and generate the possible spots
        List<Vector2> possibleSpots = FindPossibleSpots();

        /*for (int i = 0; i < possibleSpots.Count; i++)
        {
            Image newSpot = Instantiate(test, possibleSpots[i], Quaternion.identity, parentTest);
            newSpot.rectTransform.localPosition = possibleSpots[i];
        }*/

        // Then we select which spots we will use and create the map list
        List<Vector2Int> chosenSpots = ChoseSpots(possibleSpots);
        
        /*for (int i = 0; i < chosenSpots.Count; i++)
        {
            Image newSpot = Instantiate(test, (Vector2)chosenSpots[i], Quaternion.identity, parentTest);
            newSpot.rectTransform.localPosition = (Vector2)chosenSpots[i] * 20;
        }*/
        
        // Next we combine the two precedent lists into one map that we generate on screen
        
        

        // Finally we generate the lines on the map
    }

    
    
    
    // --------------- TO FIND THE POSSIBLE SPOTS --------------- 
    
    // GENERATES THE POSSIBLE POSITIONS INTO WORLD SPACE FOR THE SPOTS
    private List<Vector2> FindPossibleSpots()
    {
        Vector2 maxBounds = GetBounds(fond.rectTransform);

        Vector2 boundsX = new Vector2(-maxBounds.x, maxBounds.x);
        Vector2 boundsY = new Vector2(-maxBounds.y, maxBounds.y);
        
        Debug.Log(boundsX);
        
        
        List<Vector2> possibleSpots = new List<Vector2>();

        for (int y = 0; y < wantedMapLength; y++)
        {
            for (int x = 0; x < columnsNbr; x++)
            {
                float posX = Mathf.Lerp(boundsX.x, boundsX.y, ((float)x / columnsNbr) + (1f / columnsNbr) * 0.5f);
                float posY = boundsY.x + distanceBetweenRaws * y;
                
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

        finalBounds = new Vector2(center.x + currentWidth * 0.5f, center.y + currentHeight * 0.5f);
        
        return finalBounds;
    }
    
    
    
    // --------------- TO CHOSE WHICH SPOTS TO USE ---------------

    // SELECTS AND RETURNS EVERY SPOTS LOCATIONS 
    private List<Vector2Int> ChoseSpots(List<Vector2> possibleSpots)
    {
        int counterCamp = 0;
        int currentElementsInRaw = 0;

        int i = 0;

        List<Vector2Int> finalSpots = new List<Vector2Int>();

        for (int y = 0; y < wantedMapLength; y++)
        {
            map.Add(new ListSpots());
            
            // We go across every column in on line
            for (int x = 0; x < columnsNbr; x++)
            {
                if (VerifySpot(x, currentElementsInRaw, counterCamp <= 1))
                {
                    finalSpots.Add(new Vector2Int(x, y));
                    
                    map[y].list.Add(new Spot());
                    
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
        
        return finalSpots;
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

    private void GenerateMapList(List<Vector2> positions, List<Vector2Int> validSpots)
    {
        int index = 0;
        
        for (int y = 0; y < wantedMapLength; y++)
        {
            for (int x = 0; x < columnsNbr; x++)
            {
                if(validSpots.Contains(new Vector2Int(x, y)))
                {
                    validSpots.Remove(new Vector2Int(x, y));
                    
                    map[y].list.Add(new Spot());
                }

                else
                {
                    map[y].list.Add(null);
                }
                
                index += 1;
            }
        }
    }
    
}


[Serializable]
public class ListSpots
{
    public List<Spot> list = new List<Spot>();
}
