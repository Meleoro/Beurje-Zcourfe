using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class DecorationCreator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Decoration> possibleDecorations = new List<Decoration>();

    [Header("Parameters")] 
    [SerializeField] private int columnsNbr;
    [SerializeField] private int wantedMapLength;
    [SerializeField] private float distanceBetweenRaws;
    private float stockageCurrentMaxY;

    [Header("Other")] 
    private float stockageCurrentMinX;
    private float stockageCurrentMaxX;

    

    public void GenerateDecoration(Transform fond)
    {
        List<Vector2> possiblePos = FindPossibleSpots(fond);
        
        possiblePos = VerifyPossibleSpots(possiblePos);

        GenerateSprites(possiblePos);
    }
    
    
    

    // --------------- TO FIND THE POSSIBLE SPOTS --------------- 
    
    // GENERATES THE POSSIBLE POSITIONS INTO WORLD SPACE FOR THE SPOTS
    private List<Vector2> FindPossibleSpots(Transform fond)
    {
        Vector2 maxBounds = GetBounds(fond);

        stockageCurrentMinX = -maxBounds.x - 5;
        stockageCurrentMaxX = maxBounds.x + 5;

        List<Vector2> possibleSpots = GeneratePossibleSpots(maxBounds.y, wantedMapLength);

        return possibleSpots;
    }


    // GENERATES THE POSSIBLE POSITIONS FOR A CERTAIN NUMBER OF ITERATIONS
    private List<Vector2> GeneratePossibleSpots(float minY, int iterations)
    {
        List<Vector2> possibleSpots = new List<Vector2>();

        for (int y = 0; y < iterations; y++)
        {
            for (int x = 0; x < columnsNbr; x++)
            {
                float posX = Mathf.Lerp(stockageCurrentMinX, stockageCurrentMaxX, ((float)x / columnsNbr) + (1f / columnsNbr) * 0.5f);
                float posY = minY + distanceBetweenRaws * y;

                float posModificator = 0.15f;
                posX += Random.Range(-posModificator, posModificator);
                posY += Random.Range(-posModificator, posModificator);
                
                possibleSpots.Add(new Vector2(posX, posY));
            }

            if (y == iterations - 1)
            {
                stockageCurrentMaxY = minY + distanceBetweenRaws * y;
            }
        }

        return possibleSpots;
    }


    // GET THE BOUNDS OF THE SPRITE ON WHICH THE SPOTS WILL BE DISPLAYED
    private Vector2 GetBounds(Transform currentTransform)
    {
        Vector2 finalBounds;
        
        float currentWidth = currentTransform.localScale.x;
        float currentHeight = currentTransform.localScale.y;
        Vector2 center = currentTransform.position;

        finalBounds = new Vector2(center.x + currentWidth * 0.55f, center.y - currentHeight * 0.49f);

        return finalBounds;
    }
    
    
    // --------------- TO VERIFY THE POSSIBLE SPOTS --------------- 

    private List<Vector2> VerifyPossibleSpots(List<Vector2> possibleSpots)
    {
        for (int i = possibleSpots.Count - 1; i >= 0; i--)
        {
            RaycastHit2D hit = Physics2D.CircleCast(possibleSpots[i], 0.6f, Vector2.zero);

            if (hit.collider is not null)
            {
                if (hit.collider.CompareTag("Nod"))
                {
                    possibleSpots.RemoveAt(i);
                }
            }
        }
        
        return possibleSpots;
    }
    
    
    // --------------- TO GENERATE THE SPRITES --------------- 

    private void GenerateSprites(List<Vector2> possibleSpots)
    {
        for (int i = 0; i < possibleSpots.Count; i++)
        {
            for (int j = 0; j < possibleDecorations.Count; j++)
            {
                int index = Random.Range(0, 100);

                if (index < possibleDecorations[j].probaSpawn)
                {
                    Instantiate(possibleDecorations[j].gameObject, possibleSpots[i],
                        Quaternion.identity);
                    
                    break;
                }
            }
        }
    }
    
}


[Serializable]
public class Decoration
{
    public GameObject gameObject;
    [Range(0, 100)] public int probaSpawn;
}

