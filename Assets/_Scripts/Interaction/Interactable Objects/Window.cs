using UnityEngine;

public class Window : InteractableObject
{
    public Transform peekTarget;
    [SerializeField] private WindowPeekController peekController;
    private bool isPeeking = false;

    private void Awake()
    {
        if (peekController == null)
        {
            Debug.LogError("WindowPeekController non trovato nella scena!");
        }
    }

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
    }

    public void ForceEndPeek()
    {
        if (isPeeking)
        {
            isPeeking = false;
            InvokeInteractionFinishedEvent();
        }
    }
}