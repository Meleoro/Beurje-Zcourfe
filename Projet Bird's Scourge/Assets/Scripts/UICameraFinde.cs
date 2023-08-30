using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class UICameraFinde : MonoBehaviour
{
    void Start()
    {
        GetComponent<Canvas>().worldCamera = UICameraManage.Instance.GetComponent<Camera>();
    }
}
