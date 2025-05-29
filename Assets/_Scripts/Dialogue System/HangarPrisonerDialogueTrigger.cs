using ProjectWork;
using UnityEngine;

public class HangarPrisonerDialogueTrigger : DialogueTrigger
{
    public override void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogues[0], dialogueText, questionBoxContainer);
    }
}
