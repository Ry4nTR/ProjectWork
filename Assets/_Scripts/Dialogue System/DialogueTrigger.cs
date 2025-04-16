using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue; // The dialogue to trigger
    public DialogueText dialogueText; // Reference to the NPC's TextBubble
    public Transform questionBoxContainer; // Reference to the NPC's question box container

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue, dialogueText, questionBoxContainer); // Start the dialogue
    }
}