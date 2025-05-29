using UnityEngine;
using ProjectWork;

public class UnlockCrankButton : InteractableObject
{
    [SerializeField] private CrankInteractable crank;
    [SerializeField] private BlackScreenData doorUnlockedBlackScreenData;

    private bool systemActivated = false; // Track if system was activated

    protected override void Start()
    {
        base.Start();
        LockInteraction(); // Start locked

        // Only enable interaction if player already has card
        if (Card.HasCard)
        {
            UnlockInteraction();
        }
        else
        {
            // Subscribe to card pickup event
            Card.OnCardPickedUp += OnCardPickedUp;
        }
    }

    private void OnDestroy()
    {
        Card.OnCardPickedUp -= OnCardPickedUp;
    }

    private void OnCardPickedUp()
    {
        // Enable interaction when card is picked up
        if (!systemActivated)
        {
            UnlockInteraction();
        }
    }

    protected override void InteractChild()
    {
        if (!Card.HasCard) return;

        // First interaction - system activation
        if (!systemActivated)
        {
            crank.ActivateSystem();
            systemActivated = true;
            LockInteraction(); // Lock until crank gets stuck
        }
        // Second interaction - mid-puzzle unlock
        else if (crank.IsBlocked) // Now using the public property
        {
            crank.UnlockCrank();
            LockInteraction(); // Disable after unlocking
        }

        // Show unlock message
        if (BlackScreenTextController.Instance != null && doorUnlockedBlackScreenData != null)
        {
            BlackScreenTextController.Instance.ActivateBlackScreen(doorUnlockedBlackScreenData);
        }
    }

    // Called by CrankInteractable when it gets stuck
    public void EnableForUnlock()
    {
        if (systemActivated)
        {
            UnlockInteraction();
        }
    }
}