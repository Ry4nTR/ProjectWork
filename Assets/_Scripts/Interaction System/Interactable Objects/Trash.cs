using System;
using UnityEngine;

public class Trash : InteractableObject
{
    public static event Action<Trash> OnTrashThrown = delegate { };

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("TrashCan"))
        {
            Interact();
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
    }

    protected override void InteractChild()
    {
        OnTrashThrown?.Invoke(this);
    }
}