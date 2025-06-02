using ProjectWork;
using UnityEngine;

public class HangarPrisonerDialogueTrigger : DialogueTrigger
{
    public override void TriggerDialogue()
    {
        if (dialogues.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(dialogues[0], dialogueText, questionBoxContainer);
        }
        else
        {
            Debug.LogError("No dialogues assigned!", this);
        }
    }
}