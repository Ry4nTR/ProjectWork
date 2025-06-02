using ProjectWork;
using UnityEditor.Rendering;
using UnityEngine;

public class ScientistDialogueTrigger : DialogueTrigger
{
    public override void TriggerDialogue()
    {
        if (dialogues.Length > 0 && dialogues[0] != null)
        {
            DialogueManager.Instance.StartDialogue(dialogues[0], dialogueText, questionBoxContainer);
        }
        else
        {
            Debug.LogError("No valid dialogue assigned in ScientistDialogueTrigger!", this);
        }
    }
}