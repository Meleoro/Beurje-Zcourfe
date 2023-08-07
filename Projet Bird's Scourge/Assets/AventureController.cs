using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class AventureController : MonoBehaviour
{
    [Header("Other")]
    [HideInInspector] public bool noControl;
    [HideInInspector] public Nod currentNod;

    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private Transform _camera;
    


    // INITIALISE THE CONTROLLER AND RETURNS THE CURRENT NOD
    public void Initialise(List<ListSpots> currentMap)
    {
        currentNod = currentMap[0].list[(int)(currentMap[0].list.Count * 0.5f)];
        noControl = false;

        player.transform.position = currentNod.transform.position;
    }

    
    // MANAGE ELEMENTS ON WHICH THE MOUSE IS
    public void ManageOverlayedElement()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hitObjects = Physics2D.RaycastAll(mousePos, Vector2.zero);

        for (int i = 0; i < hitObjects.Length; i++)
        {
            if (hitObjects[i].collider.CompareTag("Nod"))
            {
                
            }
        }
    }


    // MANAGE ELEMENTS ON WHICH THE PLAYER CLICK
    public void ManageClickedElement()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hitObjects = Physics2D.RaycastAll(mousePos, Vector2.zero);

        for (int i = 0; i < hitObjects.Length; i++)
        {
            if (hitObjects[i].collider.CompareTag("Nod"))
            {
                MoveTo(hitObjects[i].collider.GetComponent<Nod>());
            }
        }
    }
    

    public void MoveTo(Nod selectedNod)
    {
        if (currentNod.connectedNods.Contains(selectedNod))
        {
            float distance = selectedNod.transform.position.y - currentNod.transform.position.y;

            _camera.DOMoveY(_camera.transform.position.y + distance, 1);
            
            player.transform.DOMove(selectedNod.transform.position, 1);
            currentNod = selectedNod;
        }
    }
}
