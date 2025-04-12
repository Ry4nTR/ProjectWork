using System;
using ITSProjectWork;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour, IInteractable
{
    public event Action<InteractableObject> OnInteractionFinished = delegate { };

    [SerializeField] private bool isUsingBlackScreen = false;
    [SerializeField] private string screenMessage = "INTERACTING";

    public virtual void Interact()
    {
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
        if(isUsingBlackScreen)
            BlackScreenTextController.OnBlackScreenTextFinished -= InvokeEvent;

        OnInteractionFinished?.Invoke(this);
    }
}

