using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class UICameraFinder : MonoBehaviour
{
    void Start()
    {
        GetComponent<Canvas>().worldCamera = UICameraManager.Instance.GetComponent<Camera>();
    }
}
