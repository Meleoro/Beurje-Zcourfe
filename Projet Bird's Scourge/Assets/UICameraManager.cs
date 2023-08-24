using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICameraManager : MonoBehaviour
{
    public static UICameraManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);
    }
}
