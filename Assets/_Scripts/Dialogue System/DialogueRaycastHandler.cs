using UnityEngine;

namespace ProjectWork
{
    public class DialogueRaycastHandler : MonoBehaviour
    {
        private DialogueManager dialogueManager;
        [SerializeField] private DialogueInteractor dialogueInteractor;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float raycastDistance = 100f;

        private void Awake()
        {
            dialogueManager = GetComponent<DialogueManager>();
        }

        private void Update()
        {
            // Left Mouse Click - Skip/advance dialogue lines
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, raycastDistance))
                {
                    // Only trigger if clicking on DialogueText during active dialogue
                    if (hit.collider.TryGetComponent(out DialogueText dialogueText) && dialogueManager.IsDialogueActive())
                    {
                        dialogueManager.HandleExternalClick();
                    }
                }
            }

            // "E" Key - Start dialogue with NPC (only when NOT in dialogue)
            if (Input.GetKeyDown(KeyCode.E) && !dialogueManager.IsDialogueActive())
            {
                if (dialogueInteractor != null && dialogueInteractor.IsLookingAtNPC())
                {
                    var trigger = dialogueInteractor.GetCurrentNPCDialogueTrigger();
                    if (trigger != null)
                        trigger.TriggerDialogue();
                }
            }
        }
    }
}