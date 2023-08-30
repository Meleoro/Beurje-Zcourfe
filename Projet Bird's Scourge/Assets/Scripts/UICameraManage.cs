using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICameraManage : MonoBehaviour
{
    public static UICameraManage Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);
    }
}
