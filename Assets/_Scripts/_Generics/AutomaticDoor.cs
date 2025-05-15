using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AutomaticDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("The transform that will be moved to open/close the door")]
    public Transform doorTransform; // Assign the actual door part in inspector

    [Tooltip("How far the door moves when opening (in local space)")]
    public Vector3 openOffset = new Vector3(0, 0, 2.5f); // Adjust based on your door type

    [Tooltip("Speed at which the door opens/closes")]
    public float moveSpeed = 2f;

    [Header("Trigger Settings")]
    [Tooltip("How close the player needs to be to trigger the door")]
    public float triggerDistance = 3f;

    [Tooltip("Should the door close automatically after player leaves?")]
    public bool autoClose = true;

    [Tooltip("Delay before auto-closing (seconds)")]
    public float closeDelay = 2f;

    // Private variables
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private float closeTimer = 0f;
    private bool isOpen = false;
    private Transform playerTransform;

    void Start()
    {
        // Initialize positions
        closedPosition = doorTransform.localPosition;
        openPosition = closedPosition + openOffset;

        // Find player (tagged as "Player" by default)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("No GameObject with 'Player' tag found. Door won't function.");
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Open/close logic
        if (distanceToPlayer <= triggerDistance)
        {
            // Player is close - open door
            OpenDoor();
            closeTimer = 0f; // Reset close timer
        }
        else if (autoClose)
        {
            // Player is far - start close timer
            closeTimer += Time.deltaTime;

            if (closeTimer >= closeDelay && isOpen)
            {
                CloseDoor();
            }
        }

        // Smooth movement
        MoveDoor();
    }

    void OpenDoor()
    {
        isOpen = true;
    }

    void CloseDoor()
    {
        isOpen = false;
    }

    void MoveDoor()
    {
        Vector3 targetPosition = isOpen ? openPosition : closedPosition;
        doorTransform.localPosition = Vector3.Lerp(
            doorTransform.localPosition,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }

    // Visualize trigger distance in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}