using UnityEngine;

namespace ProjectWork
{
    [RequireComponent(typeof(Collider))]
    public class HangarPrisoner : InteractableObject
    {
        private Collider npcCollider;

        private void Awake()
        {
            npcCollider = GetComponent<Collider>();
            DialogueManager.OnDialogueStarted += DisableCollider;
            DialogueManager.OnDialogueFinished += EnableCollider;
        }

        private void OnDestroy()
        {
            DialogueManager.OnDialogueStarted -= DisableCollider;
            DialogueManager.OnDialogueFinished -= EnableCollider;
        }

        private void EnableCollider() => npcCollider.enabled = true;
        private void DisableCollider() => npcCollider.enabled = false;

        protected override void InteractChild()
        {
            // Get the DialogueTrigger component and trigger dialogue
            var dialogueTrigger = GetComponent<HangarPrisonerDialogueTrigger>();
            if (dialogueTrigger != null)
            {
                dialogueTrigger.TriggerDialogue();
            }
            else
            {
                Debug.LogError("No HangarPrisonerDialogueTrigger component found!", this);
            }
        }
    }
}