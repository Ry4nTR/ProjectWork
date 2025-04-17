using UnityEngine;

public class UnlockCrankButton : InteractableObject
{
    [SerializeField] private CrankInteractable crank;

    public override void Interact()
    {
        base.Interact();

        // Sblocca la manopola
        crank.UnlockCrank();

        // (Opzionale) Disattiva ulteriori interazioni col pulsante
        LockInteraction();
    }
}
