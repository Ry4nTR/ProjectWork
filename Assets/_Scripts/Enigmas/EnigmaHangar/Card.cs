using UnityEngine;
using ProjectWork;

public class Card : InteractableObject
{
    [Header("Card Settings")]
    [SerializeField] private string cardName = "Access Card";
    [SerializeField] private BlackScreenData cardPickupBlackScreenData;

    // Static flag to track if player has the card
    public static bool HasCard { get; private set; } = false;

    // Event to notify other systems when card is picked up
    public static System.Action OnCardPickedUp;

    protected override void Start()
    {
        base.Start();
        SetInteractionPrompt($"Pick up {cardName}");
    }

    protected override void InteractChild()
    {
        // Player picks up the card
        HasCard = true;

        Debug.Log($"Picked up {cardName}!");

        // Notify other systems
        OnCardPickedUp?.Invoke();

        // Show pickup message if configured
        if (BlackScreenTextController.Instance != null && cardPickupBlackScreenData != null)
        {
            BlackScreenTextController.Instance.ActivateBlackScreen(cardPickupBlackScreenData);
        }

        // Disable the card object after pickup
        gameObject.SetActive(false);
    }

    // Static method to reset card state (useful for game resets)
    public static void ResetCardState()
    {
        HasCard = false;
    }
}