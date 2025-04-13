using System;
using ProjectWork;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour, IInteractable
{
    public event Action<InteractableObject> OnInteractionFinished = delegate { };

    [SerializeField] private bool canInteractAtStart = true;
    [Tooltip("Is \"OnInteractionFinished\" event called instantly after the interaction?")]
    [SerializeField] private bool isInteractionInstant = false;

    [SerializeField] private bool isUsingBlackScreen = false;
    [SerializeField] private string screenMessage = "INTERACTING";
    private bool _canInteract;

    public bool CanInteract => _canInteract;

    private void Start()
    {
        _canInteract = canInteractAtStart;
    }

    public virtual void Interact()
    {
        if (!_canInteract)
            return;

        if (isUsingBlackScreen)
        {
            BlackScreenTextController.Instance.ActivateBlackScreen(screenMessage);
            BlackScreenTextController.OnBlackScreenTextFinished += InvokeEvent;
        }
        else
        {
            InvokeEvent();
        }
    }

    private void InvokeEvent()
    {
        if (isUsingBlackScreen)
            BlackScreenTextController.OnBlackScreenTextFinished -= InvokeEvent;

        OnInteractionFinished?.Invoke(this);
    }

    public void UnlockInteraction() => _canInteract = true;
    public void LockInteraction() => _canInteract = false;

    public void ResetInteraction() => _canInteract = canInteractAtStart;
}