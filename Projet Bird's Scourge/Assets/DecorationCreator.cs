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
    [SerializeField] private Transform startX;

    [Header("Parameters")] 
    [SerializeField] private int rawsNbr;
    [SerializeField] private int wantedMapLength;
    [SerializeField] private float distanceBetweenColumns; 

    [Header("Other")] 
    private float stockageCurrentMinY;
    private float stockageCurrentMaxY;
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

        stockageCurrentMinX = maxBounds.x;
        stockageCurrentMinY = maxBounds.y;
        stockageCurrentMaxY = -maxBounds.y;

        List<Vector2> possibleSpots = GeneratePossibleSpots(stockageCurrentMinX, wantedMapLength);

        return possibleSpots;
    }


    // GENERATES THE POSSIBLE POSITIONS FOR A CERTAIN NUMBER OF ITERATIONS
    private List<Vector2> GeneratePossibleSpots(float minY, int iterations)
    {
        List<Vector2> possibleSpots = new List<Vector2>();

        for (int x = 0; x < iterations; x++)
        {
            for (int y = 0; y < rawsNbr; y++)
            {
                float posY = Mathf.Lerp(stockageCurrentMinY, stockageCurrentMaxY, ((float)y / rawsNbr) + (1f / rawsNbr) * 0.5f);
                float posX = minY + distanceBetweenColumns * x;

                float posModificator = 0.15f;
                posX += Random.Range(-posModificator, posModificator);
                posY += Random.Range(-posModificator, posModificator);
                
                possibleSpots.Add(new Vector2(posX, posY));
            }

            if (x == iterations - 1)
            {
                stockageCurrentMaxY = minY + distanceBetweenColumns * x;
            }
        }

        return possibleSpots;
    }


    // GET THE BOUNDS OF THE SPRITE ON WHICH THE SPOTS WILL BE DISPLAYED
    private Vector2 GetBounds(Transform currentTransform)
    {
        Vector2 finalBounds;
        
        float currentHeight = currentTransform.localScale.x;
        Vector2 center = currentTransform.position;

        finalBounds = new Vector2(startX.position.x - 2, center.y - currentHeight - 1.7f);

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

