using System;
using ProjectWork;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour, IInteractable
{
    public ObjectiveDefinition objectiveDefinition;
    public event Action<InteractableObject> OnInteractionFinished = delegate { };

    [SerializeField] protected bool canInteractAtStart = true;
    [SerializeField] protected string interactionPrompt = "Interact"; // Made protected so derived classes can modify it

    public bool isUsingBlackScreen = false;
    [SerializeField] private BlackScreenData blackScreenData;
    private bool _canInteract;

    public bool CanInteract => _canInteract;
    public string InteractionPrompt => interactionPrompt; // Public getter for the prompt

    protected virtual void Start()
    {
        _canInteract = canInteractAtStart;
    }

    public void Interact()
    {
        if (!_canInteract || PauseHandler.IsPaused)
        {
            Debug.LogWarning("Cannot interact with this object!");
            return;
        }
        InteractChild();
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

    /// <summary>
    /// Called at the end when the interaction is triggered.
    /// </summary>
    protected virtual void InteractChild()
    {
        Debug.Log("InteractChild method not implemented for: " + gameObject.name);
        // This method should be overridden in derived classes to implement specific interaction logic
    }

    protected void InvokeInteractionFinishedEvent()
    {
        if (isUsingBlackScreen)
            BlackScreenTextController.OnBlackScreenTextFinished -= InvokeInteractionFinishedEvent;

        OnInteractionFinished?.Invoke(this);
    }

    /// <summary>
    /// Protected method to allow derived classes to set custom interaction prompts
    /// </summary>
    protected void SetInteractionPrompt(string newPrompt)
    {
        interactionPrompt = newPrompt;
    }

    public void UnlockInteraction() => _canInteract = true;
    public void LockInteraction() => _canInteract = false;
    public void ResetInteraction() => _canInteract = canInteractAtStart;
}