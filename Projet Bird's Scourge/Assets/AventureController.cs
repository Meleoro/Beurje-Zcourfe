using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class AventureController : MonoBehaviour
{
    [Header("Other")]
    private Nod currentNod;
    private bool noControl;
    
    [Header("References")]
    [SerializeField] private GameObject player;


    private void Update()
    {
        if (!noControl)
        {
            ManageOverlayedElement();

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ManageClickedElement();
            }
        }
    }


    // INITIALISE THE CONTROLLER
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
            player.transform.DOMove(selectedNod.transform.position, 1);
            currentNod = selectedNod;
        }
    }
}
