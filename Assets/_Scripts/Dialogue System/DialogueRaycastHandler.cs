using UnityEngine;

public class DialogueRaycastHandler : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueInteractor dialogueInteractor;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float raycastDistance = 100f;

    private void Awake()
    {
        dialogueManager = GetComponent<DialogueManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, raycastDistance))
            {
                // Se stai guardando il DialogueText
                DialogueText dialogueText = hit.collider.GetComponent<DialogueText>();
                if (dialogueText != null && dialogueManager.IsDialogueActive())
                {
                    dialogueManager.HandleExternalClick();
                    return;
                }

                // Se stai guardando un NPC
                if (!dialogueManager.IsDialogueActive() && dialogueInteractor != null && dialogueInteractor.IsLookingAtNPC())
                {
                    var trigger = dialogueInteractor.GetCurrentNPCDialogueTrigger();
                    if (trigger != null)
                        trigger.TriggerDialogue();
                }
            }
        }
    }
}
