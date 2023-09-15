using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleObject : MonoBehaviour
{
    [Header("Other")]
    float currentWidthRatio;
    float currentHeightRatio;
    float charaWidthOffset;
    float charaHeightOffset;

    [Header("Prefabs")] 
    public GameObject imageParentObject;
    public GameObject textObject;
    
    
    [Header("References")] 
    public TextMeshProUGUI damageNumber;
    public RectTransform attackUIParent;
    public RectTransform attackUI;
    public RectTransform currentImageParent;
    public Image currentImage;
    public Image attackFond;
    public GameObject ghostPrefab;
    public Transform ghostParentLeft;
    public Transform ghostParentRight;
    
    public enum CompetenceType
    {
        attack,
        heal,
        summon,
        buffDamage,
        buffAccuracy,
        buffCrit,
        buffDefense
    }
    
    
    
    // WHEN THE ATTACK UI HAS TO APPEAR
    public IEnumerator UniqueCharaAttack(DataUnit currentUnit, int damage, bool deadEnnemy, DataCompetence.VFXTypes VFXType, Vector2 pos)
    {
        CompetenceType currentCompetenceType = CompetenceType.attack;
        
        SetupFeel(currentUnit.damageSprite, currentUnit, pos);
        
        StartCoroutine(CharacterFeel(currentUnit, currentCompetenceType, deadEnnemy));

        StartCoroutine(TextFeel(currentCompetenceType, damage, pos));

        StartCoroutine(GhostTrail(10, 0.04f, 0.1f, currentCompetenceType));
        
        LaunchVFX(VFXType);

        yield return new WaitForSeconds(1.3f);

        StartCoroutine(EndFeel());
    }
    
    
    // WHEN THE HEAL UI HAS TO APPEAR
    public IEnumerator UniqueCharaHeal(DataUnit currentUnit, int damage, bool deadEnnemy, DataCompetence.VFXTypes VFXType, Vector2 pos)
    {
        CompetenceType currentCompetenceType = CompetenceType.heal;
        
        SetupFeel(currentUnit.attackSprite, currentUnit, pos);
        
        StartCoroutine(CharacterFeel(currentUnit, currentCompetenceType, deadEnnemy));

        StartCoroutine(TextFeel(currentCompetenceType, damage, pos));

        //StartCoroutine(GhostTrail(10, 0.04f, 0.1f, currentCompetenceType));
        
        LaunchVFX(VFXType);

        yield return new WaitForSeconds(1.3f);

        StartCoroutine(EndFeel());
    }
    
    
    // WHEN THE HEAL UI HAS TO APPEAR
    public IEnumerator UniqueCharaBuff(DataUnit currentUnit, BuffManager.BuffType buffType, int buffValue, DataCompetence.VFXTypes VFXType, Vector2 pos)
    {
        CompetenceType currentCompetenceType = CompetenceType.buffDamage;
        
        switch (buffType)
        {
            case BuffManager.BuffType.accuracy : 
                currentCompetenceType = CompetenceType.buffAccuracy;
                break;
            
            case BuffManager.BuffType.crit : 
                currentCompetenceType = CompetenceType.buffCrit;
                break;
            
            case BuffManager.BuffType.defense : 
                currentCompetenceType = CompetenceType.buffDefense;
                break;
        }

        SetupFeel(currentUnit.attackSprite, currentUnit, pos);
        
        StartCoroutine(CharacterFeel(currentUnit, currentCompetenceType, false));

        StartCoroutine(TextFeel(currentCompetenceType, buffValue, pos));

        //StartCoroutine(GhostTrail(10, 0.04f, 0.1f, currentCompetenceType));
        
        LaunchVFX(VFXType);

        yield return new WaitForSeconds(1.3f);

        StartCoroutine(EndFeel());
    }
    
    
    
    private void SetupFeel(Sprite currentSprite, DataUnit currentData, Vector2 wantedPos)
    {
        CameraManager.Instance.canMove = false;
        UIBattleManager.Instance.buttonScript.SwitchButtonInteractible(false);
        attackUI.gameObject.SetActive(true);
        
        charaWidthOffset = 800 * currentData.XPosModificator * currentWidthRatio;
        charaHeightOffset = 300 * currentData.YPosModificator * currentHeightRatio;
        
        

        currentImageParent = Instantiate(imageParentObject, wantedPos, Quaternion.identity, attackUI).GetComponent<RectTransform>();
        currentImage = currentImageParent.GetComponentInChildren<Image>();
        
        Destroy(currentImageParent.gameObject, 3);
        
        currentImage.gameObject.SetActive(true);
        currentImage.sprite = currentSprite;
        
        currentImageParent.localScale = Vector3.one * (0.8f * currentData.attackSpriteSize);

        currentImageParent.localPosition = new Vector3(wantedPos.x + charaWidthOffset, wantedPos.y + charaHeightOffset, currentImageParent.position.z);
            

        attackFond.DOFade(0.8f, 0.05f);

        float fadeImage = currentImage.material.GetFloat("_Alpha");
        DOTween.To(() => fadeImage, x => fadeImage = x, 1, 0.2f)
            .OnUpdate(() => {
                currentImage.material.SetFloat("_Alpha", fadeImage);
            });
        
        currentImage.material.SetFloat("_DissolveValue", 0);
    }


    private IEnumerator CharacterFeel(DataUnit currentData, CompetenceType currentCompetenceType, bool deathBlow)
    {
        if(currentCompetenceType == CompetenceType.attack)
            currentImage.rectTransform.DOShakePosition(0.5f, new Vector3(1, 1, 0) * (10 * currentWidthRatio), 20);
        
        currentImageParent.DOScale(Vector3.one * (1f * currentData.attackSpriteSize), 0.15f);

        if (!deathBlow)
        {
            Color wantedColor;
            
            if (CompetenceType.attack == currentCompetenceType)
                wantedColor = Color.red;

            else if (CompetenceType.heal == currentCompetenceType)
                wantedColor = Color.green;
            
            else
                wantedColor = Color.yellow;
            
        
            Color colorImage = Color.black;
            DOTween.To(() => colorImage, x => colorImage = x, wantedColor, 0.3f)
                .OnUpdate(() => {
                    currentImage.material.SetColor("_Color", colorImage);
                });
            
            yield return new WaitForSeconds(0.3f);
            
            colorImage = currentImage.material.GetColor("_Color");
            DOTween.To(() => colorImage, x => colorImage = x, Color.white, 0.3f)
                .OnUpdate(() => {
                    currentImage.material.SetColor("_Color", colorImage);
                });
        }

        else
        {
            Color wantedColor = Color.red;

            currentImage.material.SetColor("_Color", Color.white);
            Color colorAttacked = currentImage.material.GetColor("_Color");
            DOTween.To(() => colorAttacked, x => colorAttacked = x, wantedColor, 0.3f)
                .OnUpdate(() => {
                    currentImage.material.SetColor("_Color", colorAttacked);
                });

            yield return new WaitForSeconds(0.3f);
            
            float dissoveValue = currentImage.material.GetFloat("_DissolveValue");
            DOTween.To(() => dissoveValue, x => dissoveValue = x, 1, 0.3f * 3)
                .OnUpdate(() => {
                    currentImage.material.SetFloat("_DissolveValue", dissoveValue);
                });


            colorAttacked = currentImage.material.GetColor("_Color");
            DOTween.To(() => colorAttacked, x => colorAttacked = x, Color.black, 0.9f)
                .OnUpdate(() => {
                    currentImage.material.SetColor("_Color", colorAttacked);
                });
        }
    }


    private IEnumerator TextFeel(CompetenceType currentCompetenceType, int damage, Vector2 wantedPos)
    {
        damageNumber = Instantiate(textObject, wantedPos, Quaternion.identity, attackUI).GetComponent<TextMeshProUGUI>();
        Destroy(damageNumber.gameObject, 5);
        
        
        Vector3 posLeftBottomCorner = new Vector3(-attackUIParent.rect.width * 0.5f, -attackUIParent.rect.height * 0.5f, 0);
        float healModificator = 1;

        if (currentCompetenceType == CompetenceType.heal || currentCompetenceType == CompetenceType.summon)
        {
            healModificator = 0.8f;
        }
        
        Vector3 pos1 = new Vector3(wantedPos.x * currentWidthRatio, wantedPos.y * currentHeightRatio, 0);
        Vector3 pos2 = new Vector3(wantedPos.x * currentWidthRatio, wantedPos.y * currentHeightRatio, 0);
        
        damageNumber.rectTransform.localPosition = pos1;
        damageNumber.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        damageNumber.rectTransform.localScale = Vector3.one * 1;

        damageNumber.rectTransform.DOLocalMoveX(pos2.x, 0.6f).SetEase(Ease.InQuint);
        damageNumber.rectTransform.DOLocalMoveY(pos2.y, 0.6f).SetEase(Ease.InQuint);

        damageNumber.rectTransform.DOScale(Vector3.one * 1.5f, 0.6f);

        damageNumber.rectTransform.DORotate(new Vector3(0, 0, 10 * healModificator), 0.6f).SetEase(Ease.InQuint);
        
        
        if (currentCompetenceType == CompetenceType.summon)
        {
            damageNumber.text = "Summoned";
            damageNumber.color = Color.magenta;
        }
        else if (currentCompetenceType == CompetenceType.heal)
        {
            damageNumber.text = damage.ToString();
            damageNumber.color = Color.green;
        }
        else if (currentCompetenceType == CompetenceType.attack)
        {   
            damageNumber.text = damage.ToString();
            damageNumber.color = Color.red;
        }
        else
        {
            switch (currentCompetenceType)
            {
                case (CompetenceType.buffDamage) :
                    damageNumber.text = "Attack + " + damage.ToString() + "%";
                    break;
                
                case (CompetenceType.buffAccuracy) :
                    damageNumber.text = "Accuracy + " + damage.ToString() + "%";
                    break;
                
                case (CompetenceType.buffCrit) :
                    damageNumber.text = "Crit + " + damage.ToString() + "%";
                    break;
                
                case (CompetenceType.buffDefense) :
                    damageNumber.text = "Defense + " + damage.ToString() + "%";
                    break;
            }
            
            damageNumber.color = Color.yellow;
        }
        

        yield return new WaitForSeconds(0.6f);

        damageNumber.DOFade(0, 0.2f);
    }
    
    
    private IEnumerator GhostTrail(int iterations, float durationBetween, float ghostDuration, CompetenceType currentCompetenceType)
    {
        while (iterations > 0)
        {
            iterations -= 1;

            GameObject currentPrefab = null;
            Image currentGhost = null;
            
            
            currentPrefab = Instantiate(ghostPrefab, currentImage.rectTransform.position, Quaternion.identity, ghostParentLeft);

            ghostPrefab.transform.localScale = currentImageParent.localScale * 0.9f;
            currentGhost = currentPrefab.GetComponent<Image>();
            currentGhost.sprite = currentImage.sprite;
            
            
            Color wantedColor = Color.red;

            currentGhost.DOFade(1, ghostDuration).OnComplete(() => { currentGhost.DOFade(0, 0.2f); });
            currentGhost.material.SetColor("_Color", wantedColor);

            yield return new WaitForSeconds(durationBetween);
        }
    }
    
    
    private void LaunchVFX(DataCompetence.VFXTypes currentVFXType)
    {
        Vector2 wantedPos = currentImageParent.position;

        if (currentVFXType == DataCompetence.VFXTypes.bam)
        {
            UIVfxManager.Instance.DOBam(wantedPos, true);
        }
        
        else if (currentVFXType == DataCompetence.VFXTypes.slash)
        {
            UIVfxManager.Instance.DOSlash(wantedPos, true);
        }
        
        else if (currentVFXType == DataCompetence.VFXTypes.heal)
        {
            UIVfxManager.Instance.DoHeal(currentImageParent, currentImageParent, false);
        }
    }
    

    public IEnumerator EndFeel()
    {
        float fadeImage = currentImage.material.GetFloat("_Alpha");
        DOTween.To(() => fadeImage, x => fadeImage = x, 0.5f, 0.1f)
            .OnUpdate(() => {
                currentImage.material.SetFloat("_Alpha", fadeImage);
            });
        
        
        Color colorImage = currentImage.material.GetColor("_Color");
        DOTween.To(() => colorImage, x => colorImage = x, colorImage, 0.1f)
            .OnUpdate(() => {
                currentImage.material.SetColor("_Color", colorImage);
            });
        
        currentImageParent.DOScale(currentImageParent.localScale * 0.7f, 0.1f).SetEase(Ease.Linear);
        
        attackFond.DOFade(0f, 0.1f + 0.01f);
        
        
        yield return new WaitForSeconds(0.1f + 0.01f);

        
        //AttackUISetup();
        attackUI.gameObject.SetActive(false);
        MouseManager.Instance.noControl = false;

        if (BattleManager.Instance.order[0].CompareTag("Unit"))
        {
            UIBattleManager.Instance.buttonScript.SwitchButtonInteractible(true);
        }
        else if (BattleManager.Instance.order[0].CompareTag("Ennemy"))
        {
            UIBattleManager.Instance.buttonScript.SwitchButtonInteractible(false);
        }
    }
}
