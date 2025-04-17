using UnityEngine;
using System;

public class Window : InteractableObject
{
    public Transform peekTarget;
    [SerializeField] private WindowPeekController peekController;
    private bool isPeeking = false;

    // Events for spawning control
    public static event Action OnPeekStarted;
    public static event Action OnPeekEnded;

    public override void Interact()
    {
        if (!CanInteract) return;

        if (!isPeeking)
        {
            StartPeek();
        }
    }

    private void StartPeek()
    {
        if (peekController == null) return;

        isPeeking = true;
        peekController.StartPeek(this);
        OnPeekStarted?.Invoke(); // Trigger spawn start
    }

    public void ForceEndPeek()
    {
        if (isPeeking)
        {
            isPeeking = false;
            OnPeekEnded?.Invoke(); // Trigger spawn stop
            InvokeInteractionFinishedEvent();
        }
    }
}