using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpot : MonoBehaviour
{
    public Color gizmoColor;


    public void SpawnUnit(GameObject currentUnit, int currentHealth)
    {
        Unit unit = Instantiate(currentUnit, transform.position, Quaternion.identity, transform).GetComponent<Unit>();
        unit.currentHealth = currentHealth;
    }
    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
}
