using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AventureCreator : MonoBehaviour
{
    [Header("General")]
    public List<ListSpots> spots = new List<ListSpots>();
    [SerializeField] private int wantedSpotsLength;

    [Header("Parametres")] 
    [SerializeField] private int columnsNbr;
    [Range(20, 100)] [SerializeField] private int distanceBetweenRaws;
    [SerializeField] private int stepsBetweenCamp;
    [SerializeField] private int maxElementsPerRaw;
    
    [Header("Références")]
    public Image fond;
    public Image test;
    public Transform parentTest;
    
    public void GenerateMap()
    {
        // First we find the bounds and generate the possible spots
        List<Vector2> possibleSpots = FindPossibleSpots();

        for (int i = 0; i < possibleSpots.Count; i++)
        {
            Image newSpot = Instantiate(test, possibleSpots[i], Quaternion.identity, parentTest);
            newSpot.rectTransform.localPosition = possibleSpots[i];
        }

        // Then we select which of these spots we will use
        List<Vector2> chosenSpots = ChoseSpots(possibleSpots);

        // Finally we generate the points and the lines on the map
    }

    
    
    
    // --------------- TO FIND THE POSSIBLE SPOTS --------------- 
    
    private List<Vector2> FindPossibleSpots()
    {
        Vector2 maxBounds = GetBounds(fond.rectTransform);

        Vector2 boundsX = new Vector2(-maxBounds.x, maxBounds.x);
        Vector2 boundsY = new Vector2(-maxBounds.y, maxBounds.y);
        
        Debug.Log(boundsX);
        
        
        List<Vector2> possibleSpots = new List<Vector2>();

        for (int y = 0; y < wantedSpotsLength; y++)
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

    private Vector2 GetBounds(RectTransform currentTransform)
    {
        Vector2 finalBounds;
        
        float currentWidth = currentTransform.rect.width;
        float currentHeight = currentTransform.rect.height;
        Vector2 center = currentTransform.localPosition;

        finalBounds = new Vector2(center.x + currentWidth * 0.5f, center.y + currentHeight * 0.5f);
        
        return finalBounds;
    }
    
    
    
    // --------------- TO CHOSE WHICH SPOTS TO USE---------------

    private List<Vector2> ChoseSpots(List<Vector2> possibleSpots)
    {
        int counterCamp = 0;
        int currentRow = 0;
        int currentColumn = 0;
        int currentElementsInRaw = 0;

        for (int i = 0; i < possibleSpots.Count; i++)
        {
            if (currentColumn < columnsNbr)
            {
                
            }
            else
            {
                currentColumn = 0;
                currentRow += 1;
                counterCamp -= 1;
            }
        }
        
        return null;
    }
}


[Serializable]
public class ListSpots
{
    public List<Spot> list = new List<Spot>();
}
