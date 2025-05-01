using ProjectWork;

public class Bed : InteractableObject
{
    public static event System.Action OnBedInteracted = delegate { };

    private void Awake()
    {
        TutorialTaskChecker.OnTasksCompleted += UnlockInteraction;
    }

    private void OnDestroy()
    {
        TutorialTaskChecker.OnTasksCompleted -= UnlockInteraction;
    }

    public override void Interact()
    {
        base.Interact();
        OnBedInteracted?.Invoke();
        LockInteraction();
    }
}