using UnityEngine;

public class DialogueInteractor : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject interactionPrompt;

    [Header("Detection Settings")]
    [SerializeField] private float interactionRange = 5f;
    [SerializeField] private LayerMask npcLayerMask;

    private Camera playerCamera;
    private DialogueTrigger currentNPC;

    private void Start()
    {
        playerCamera = Camera.main;
        SetPromptVisibility(false);
    }

    private void Update()
    {
        DetectNPC();
    }

    private void DetectNPC()
    {
        if (playerCamera == null) return;

        // Only show debug when state changes
        bool wasLookingAtNPC = currentNPC != null;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, npcLayerMask))
        {
            if (hit.collider.TryGetComponent<DialogueTrigger>(out var newNPC) && hit.collider.CompareTag("NPC"))
            {
                if (currentNPC != newNPC)
                {
                    currentNPC = newNPC;
                    SetPromptVisibility(true);
                    Debug.Log($"Started looking at NPC: {currentNPC.name}");
                }
                return;
            }
        }

        if (wasLookingAtNPC)
        {
            Debug.Log("Stopped looking at NPC");
        }
        ClearCurrentNPC();
    }

    public bool IsLookingAtNPC() => currentNPC != null;
    public DialogueTrigger GetCurrentNPCDialogueTrigger() => currentNPC;

    private void SetPromptVisibility(bool visible)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(visible);
        }
    }

    private void ClearCurrentNPC()
    {
        if (currentNPC != null)
        {
            currentNPC = null;
            SetPromptVisibility(false);
        }
    }

    // Debug visualization
    private void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactionRange);
        }
    }
}