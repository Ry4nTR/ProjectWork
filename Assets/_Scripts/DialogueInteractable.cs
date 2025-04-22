using UnityEngine;

public class DialogueInteractable : InteractableObject
{
    [SerializeField] private DialogueTrigger dialogueTrigger;

    private bool hasCompleted = false;

    private void OnEnable()
    {
        DialogueManager.OnDialogueEnded += OnDialogueFinished;
    }

    private void OnDisable()
    {
        DialogueManager.OnDialogueEnded -= OnDialogueFinished;
    }

    public override void Interact()
    {
        dialogueTrigger.TriggerDialogue();
    }

    private void OnDialogueFinished(DialogueTrigger finishedTrigger)
    {
        if (!hasCompleted && finishedTrigger == dialogueTrigger)
        {
            hasCompleted = true;
            InvokeInteractionFinishedEvent();
        }
    }

    public override void ResetInteraction()
    {
        hasCompleted = false;
    }
}
