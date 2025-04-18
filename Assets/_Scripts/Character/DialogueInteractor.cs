using UnityEngine;

public class DialogueInteractor : MonoBehaviour
{
    [Header("UI References")]
    private TalkText interactionPrompt;

    [Header("Detection Settings")]
    [SerializeField] private float interactionRange = 5f;
    [SerializeField] private LayerMask npcLayerMask;

    private Camera playerCamera;
    private DialogueTrigger currentNPC;

    private void Awake()
    {
        interactionPrompt = FindFirstObjectByType<TalkText>(FindObjectsInactive.Include);
    }

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
            if (hit.collider.TryGetComponent<DialogueTrigger>(out var newNPC))
            {
                if (currentNPC != newNPC)
                {
                    currentNPC = newNPC;
                    SetPromptVisibility(true);
                }
                return;
            }
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