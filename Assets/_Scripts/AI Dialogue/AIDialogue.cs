// AIDialogue.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New AIDialogue", menuName = "Dialogue/AI Dialogue")]
public class AIDialogue : ScriptableObject
{
    [System.Serializable]
    public class DialogueMessage
    {
        public bool isAISpeaking; // true=AI, false=player
        [TextArea(3, 10)] public string text;
    }

    public List<DialogueMessage> messages = new List<DialogueMessage>();
}