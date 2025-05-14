using UnityEngine;
using ProjectWork;

public class UnlockCrankButton : InteractableObject
{
    [SerializeField] private CrankInteractable crank;
    [SerializeField] private BlackScreenData doorUnlockedBlackScreenData;

    private void Start()
    {
        // Make button non-interactable at start
        LockInteraction();
    }

    public override void Interact()
    {
        base.Interact();

        // Unlock the crank
        crank.UnlockCrank();

        // Show unlock message
        if (BlackScreenTextController.Instance != null && doorUnlockedBlackScreenData != null)
        {
            BlackScreenTextController.Instance.ActivateBlackScreen(doorUnlockedBlackScreenData);
        }

        // Disable further interactions with the button
        LockInteraction();
    }

    // Public method to enable the button when needed
    public void EnableInteraction()
    {
        UnlockInteraction();
    }
}