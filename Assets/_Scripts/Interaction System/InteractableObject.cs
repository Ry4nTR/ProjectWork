using System;
using ProjectWork;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour, IInteractable
{
    public ObjectiveDefinition objectiveDefinition;
    public event Action<InteractableObject> OnInteractionFinished = delegate { };

    [SerializeField] protected bool canInteractAtStart = true;
    [SerializeField] private string interactionPrompt = "Interact"; // Default prompt text

    [SerializeField] private bool isUsingBlackScreen = false;
    [SerializeField] private BlackScreenData blackScreenData;
    private bool _canInteract;

    public bool CanInteract => _canInteract;
    public string InteractionPrompt => interactionPrompt; // Public getter for the prompt

    protected virtual void Start()
    {
        _canInteract = canInteractAtStart;
    }

    public virtual void Interact()
    {
        if (!_canInteract)
        {
            Debug.LogWarning("Cannot interact with this object!");
            return;
        }
        if (isUsingBlackScreen)
        {
            BlackScreenTextController.Instance.ActivateBlackScreen(blackScreenData);
            BlackScreenTextController.OnBlackScreenTextFinished += InvokeInteractionFinishedEvent;
        }
        else
        {
            InvokeInteractionFinishedEvent();
        }
    }

    protected void InvokeInteractionFinishedEvent()
    {
        if (isUsingBlackScreen)
            BlackScreenTextController.OnBlackScreenTextFinished -= InvokeInteractionFinishedEvent;

        OnInteractionFinished?.Invoke(this);
    }

    public void UnlockInteraction() => _canInteract = true;
    public void LockInteraction() => _canInteract = false;
    public void ResetInteraction() => _canInteract = canInteractAtStart;
}