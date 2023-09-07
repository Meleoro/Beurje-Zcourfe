using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEffects : MonoBehaviour
{
    private ObjectController originalScript;

    private void Start()
    {
        originalScript = GetComponent<ObjectController>();
    }


    public IEnumerator HealEffect(ShopItemData itemData, Unit currentUnit)
    {
        CameraManager.Instance.effectsScript.Zoom(2, currentUnit.transform.position, 1);
        
        yield return new WaitForSeconds(1f);
        
        StartCoroutine(UIBattleManager.Instance.objectsScript.UniqueCharaHeal(currentUnit.data, 5, false, DataCompetence.VFXTypes.heal));
        
        yield return new WaitForSeconds(1f);
        
        CameraManager.Instance.effectsScript.ExitZoom(0.5f);

        yield return new WaitForSeconds(0.5f);
        
        originalScript.StopUseObject();
    }

    
    public IEnumerator DamageEffect(ShopItemData itemData, Ennemy currentEnnemy)
    {
        CameraManager.Instance.effectsScript.Zoom(2, currentEnnemy.transform.position, 1);
        
        yield return new WaitForSeconds(1f);

        StartCoroutine(UIBattleManager.Instance.objectsScript.UniqueCharaAttack(currentEnnemy.data, 5, false, DataCompetence.VFXTypes.slash));
        
        yield return new WaitForSeconds(2f);
        
        CameraManager.Instance.effectsScript.ExitZoom(0.5f);

        yield return new WaitForSeconds(0.5f);
        
        originalScript.StopUseObject();
    }

    

    public IEnumerator BuffEffect(ShopItemData itemData, Unit currentUnit)
    {
        CameraManager.Instance.effectsScript.Zoom(2, currentUnit.transform.position, 1);
        
        yield return new WaitForSeconds(1f);
        
        yield return new WaitForSeconds(1f);
        
        CameraManager.Instance.effectsScript.ExitZoom(0.5f);

        yield return new WaitForSeconds(0.5f);
        
        originalScript.StopUseObject();
    }



    public IEnumerator InvocEffect(ShopItemData itemData, OverlayTile currentTile)
    {
        CameraManager.Instance.effectsScript.Zoom(2, currentTile.transform.position, 1);
        
        yield return new WaitForSeconds(1f);
        
        yield return new WaitForSeconds(1f);
        
        CameraManager.Instance.effectsScript.ExitZoom(0.5f);

        yield return new WaitForSeconds(0.5f);
        
        originalScript.StopUseObject();
    }
}
