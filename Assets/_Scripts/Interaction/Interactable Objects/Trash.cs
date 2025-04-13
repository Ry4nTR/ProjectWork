using System;
using UnityEngine;

public class Trash : InteractableObject
{
    public event Action OnTrashThrown = delegate { };

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("TrashCan"))
        {
            Interact();
            gameObject.SetActive(false);
        }
    }

    public override void Interact()
    {
        InvokeInteractionFinishedEvent();
    }
}