using UnityEngine;

public class Food : InteractableObject
{
    public override void Interact()
    {
        base.Interact();  // Mantiene la logica base (eventi, black screen, ecc.)
        TrashManager.Instance.SpawnTrash();  // Spawna nel punto del TrashManager
    }
}