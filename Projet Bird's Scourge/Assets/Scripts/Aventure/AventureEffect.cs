using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AventureEffect : MonoBehaviour
{
    public static AventureEffect Instance;
        
    public List<SpriteRenderer> decorationsSprites;
    public List<SpriteRenderer> iconsSprites;
    public List<LineRenderer> lineSprites;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);
    }


    public IEnumerator AppearEffect()
    {
        for (int i = 0; i < iconsSprites.Count; i++)
        {
            float dissolveValue2 = 1;
            SpriteRenderer currentSprite = iconsSprites[i];

            DOTween.To(() => dissolveValue2, x => dissolveValue2 = x, 0, Random.Range(1.5f, 2.5f)).OnUpdate((() =>
                currentSprite.material.SetFloat("_DissolveValue", dissolveValue2))); 
        }
        
        
        for (int i = 0; i < lineSprites.Count; i++)
        {
            float dissolveValue3 = 1;
            LineRenderer currentSprite = lineSprites[i];

            DOTween.To(() => dissolveValue3, x => dissolveValue3 = x, 0, Random.Range(1.5f, 2.5f)).OnUpdate((() =>
                currentSprite.material.SetFloat("_DissolveValue", dissolveValue3))); 
        }

        for (int i = 0; i < decorationsSprites.Count; i++)
        {
            decorationsSprites[i].material.SetFloat("_DissolveValue", 1);
        }
        
        for (int i = 0; i < decorationsSprites.Count; i++)
        {
            float dissolveValue = 1;
            SpriteRenderer currentSprite = decorationsSprites[i];

            DOTween.To(() => dissolveValue, x => dissolveValue = x, 0, Random.Range(3f, 5f)).OnUpdate((() =>
                currentSprite.material.SetFloat("_DissolveValue", dissolveValue)));
        }

        yield return null;
    }



    public void AddIcon(SpriteRenderer newIcon)
    {
        iconsSprites.Add(newIcon);
    }
    
    public void RemoveIcon(SpriteRenderer oldIcon)
    {
        iconsSprites.Remove(oldIcon);
    }

    public void AddDecoration(SpriteRenderer newDecoration)
    {
        decorationsSprites.Add(newDecoration);
    }

    public void AddLine(LineRenderer newLineRenderer)
    {
        lineSprites.Add(newLineRenderer);
    }
    
}
