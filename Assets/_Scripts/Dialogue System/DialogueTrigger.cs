using UnityEngine;

namespace ProjectWork
{
    public abstract class DialogueTrigger : MonoBehaviour
    {
        public Dialogue[] dialogues; // The dialogues to trigger
        public DialogueText dialogueText; // Reference to the NPC's TextBubble
        public Transform questionBoxContainer; // Reference to the NPC's question box container

        public abstract void TriggerDialogue();
    }
}