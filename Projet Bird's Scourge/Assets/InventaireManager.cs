using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventaireManager : MonoBehaviour
{
    public static InventaireManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);
    }
}
