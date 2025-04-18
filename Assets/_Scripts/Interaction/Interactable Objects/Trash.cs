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
            Destroy(gameObject);

        }
    }

    public override void Interact()
    {
        OnTrashThrown?.Invoke();
        InvokeInteractionFinishedEvent();
    }
}