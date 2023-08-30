using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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



    public void GenerateDecoration(Transform fond, float minX, float maxX)
    {
        List<Vector2> possiblePos = FindPossibleSpots(fond, minX, maxX);
        
        possiblePos = VerifyPossibleSpots(possiblePos);

        GenerateSprites(possiblePos);
    }
    
    
    

    // --------------- TO FIND THE POSSIBLE SPOTS --------------- 
    
    // GENERATES THE POSSIBLE POSITIONS INTO WORLD SPACE FOR THE SPOTS
    private List<Vector2> FindPossibleSpots(Transform fond, float minX, float maxX)
    {
        Vector2 maxBounds = GetBounds(fond);
        
        stockageCurrentMinY = maxBounds.y;
        stockageCurrentMaxY = maxBounds.x;

        List<Vector2> possibleSpots = GeneratePossibleSpots(minX, maxX, wantedMapLength);

        return possibleSpots;
    }


    // GENERATES THE POSSIBLE POSITIONS FOR A CERTAIN NUMBER OF ITERATIONS
    private List<Vector2> GeneratePossibleSpots(float minX, float maxX, int iterations)
    {
        List<Vector2> possibleSpots = new List<Vector2>();

        float currentX = minX;

        while (currentX < maxX)
        {
            for (int y = 0; y < rawsNbr; y++)
            {
                float posY = Mathf.Lerp(stockageCurrentMinY, stockageCurrentMaxY, ((float)y / rawsNbr) + (1f / rawsNbr) * 0.5f);
                float posX = currentX;

                float posModificator = 0.15f;
                posX += Random.Range(-posModificator, posModificator);
                posY += Random.Range(-posModificator, posModificator);
                
                possibleSpots.Add(new Vector2(posX, posY));
            }

            currentX += distanceBetweenColumns;
        }
        
        /*for (int x = 0; x < iterations; x++)
        {
            for (int y = 0; y < rawsNbr; y++)
            {
                float posY = Mathf.Lerp(stockageCurrentMinY, stockageCurrentMaxY, ((float)y / rawsNbr) + (1f / rawsNbr) * 0.5f);
                float posX = minX + distanceBetweenColumns * x;

                float posModificator = 0.15f;
                posX += Random.Range(-posModificator, posModificator);
                posY += Random.Range(-posModificator, posModificator);
                
                possibleSpots.Add(new Vector2(posX, posY));
            }

            if (x == iterations - 1)
            {
                stockageCurrentMaxY = minY + distanceBetweenColumns * x;
            }
        }*/

        return possibleSpots;
    }


    // GET THE BOUNDS OF THE SPRITE ON WHICH THE SPOTS WILL BE DISPLAYED
    private Vector2 GetBounds(Transform currentTransform)
    {
        Vector2 finalBounds;
        
        float currentHeight = currentTransform.localScale.x;
        Vector2 center = currentTransform.position;

        finalBounds = new Vector2(center.y + currentHeight + 1.7f, center.y - currentHeight - 1.7f);

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
                    SpriteRenderer currentSR = Instantiate(possibleDecorations[j].gameObject, possibleSpots[i],
                        Quaternion.identity).GetComponentInChildren<SpriteRenderer>();

                    float dissolveValue = 1;

                    DOTween.To(() => dissolveValue, x => dissolveValue = x, 0, Random.Range(3f, 5f)).OnUpdate((() =>
                        currentSR.material.SetFloat("_DissolveValue", dissolveValue))); 
                    
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

