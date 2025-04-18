using System;
using UnityEngine;

public class Trash : InteractableObject
{
    public static event Action<Trash> OnTrashThrown = delegate { };

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
        OnTrashThrown?.Invoke(this);
        InvokeInteractionFinishedEvent();
    }
}