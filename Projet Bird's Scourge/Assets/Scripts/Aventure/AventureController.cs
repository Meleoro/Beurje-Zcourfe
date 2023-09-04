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
    private AventureManager mainScript;
    


    // INITIALISE THE CONTROLLER AND RETURNS THE CURRENT NOD
    public void Initialise(List<ListSpots> currentMap)
    {
        currentNod = currentMap[0].list[(int)(currentMap[0].list.Count * 0.5f)];
        noControl = false;

        player.transform.position = currentNod.transform.position;

        mainScript = GetComponent<AventureManager>();
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
                StartCoroutine(MoveTo(hitObjects[i].collider.GetComponent<Nod>()));
            }
        }
    }
    

    // MOVE THE CAMERA AND THE PLAYER + RETURNS TRUE IF A MOVEMENT HAS BEEN MADE
    public IEnumerator MoveTo(Nod selectedNod)
    {
        if (currentNod.connectedNods.Contains(selectedNod) && selectedNod.transform.position.x > currentNod.transform.position.x)
        {
            AventureManager.Instance.currentY += 1;
            
            float distance = selectedNod.transform.position.x - currentNod.transform.position.x;

            CameraManager.Instance.transform.DOMoveX(CameraManager.Instance.transform.position.x + distance, 1);
            
            player.transform.DOMove(selectedNod.transform.position, 1);
            currentNod = selectedNod;
            
            //mainScript.scriptCreator.UpdateMap(currentNod);

            yield return new WaitForSeconds(1.3f);
            
            selectedNod.DoNodeEffect();
        }
    }
}
