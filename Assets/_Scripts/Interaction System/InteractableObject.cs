using System;
using ProjectWork;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour, IInteractable
{
    public event Action<InteractableObject> OnInteractionFinished = delegate { };

    [SerializeField] private bool canInteractAtStart = true;

    [SerializeField] private bool isUsingBlackScreen = false;
    [SerializeField] private BlackScreenData blackScreenData;
    private bool _canInteract;

    public bool CanInteract => _canInteract;

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