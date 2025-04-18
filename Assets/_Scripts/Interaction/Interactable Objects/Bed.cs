﻿using ProjectWork;

public class Bed : InteractableObject
{
    public static event System.Action OnBedInteracted = delegate { };

    private void Awake()
    {
        GameInteractionManager.OnTasksCompleted += UnlockInteraction;
    }

    private void OnDestroy()
    {
        GameInteractionManager.OnTasksCompleted -= UnlockInteraction;
    }

    public override void Interact()
    {
        base.Interact();
        OnBedInteracted?.Invoke();
        LockInteraction();
    }
}