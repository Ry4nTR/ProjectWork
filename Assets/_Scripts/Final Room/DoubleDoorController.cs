using ProjectWork;
using UnityEngine;

public class DoubleDoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Transform leftDoor;    // The part that moves in -Z direction
    [SerializeField] private Transform rightDoor;   // The part that moves in +Z direction
    [SerializeField] private float openDistance = 2f; // How far each door part should move
    [SerializeField] private float moveSpeed = 1.5f;   // Speed of door movement
    [SerializeField] private float activationDistance = 8f; // Player trigger distance
    [SerializeField] private float closeDelay = 2f; // Time after player exits before closing

    [Header("Player Reference")]
    [SerializeField] private Transform player;      // Reference to player transform

    private Vector3 leftDoorClosedPosition;
    private Vector3 rightDoorClosedPosition;
    private Vector3 leftDoorOpenPosition;
    private Vector3 rightDoorOpenPosition;

    private bool shouldOpen = false; // From puzzle completion
    private bool playerInRange = false;
    private float timeOutsideRange = 0f;
    private bool isOpen = false;

    private void Start()
    {
        // Store initial positions
        leftDoorClosedPosition = leftDoor.position;
        rightDoorClosedPosition = rightDoor.position;

        // Calculate open positions (moving along Z-axis)
        leftDoorOpenPosition = leftDoorClosedPosition + (Vector3.back * openDistance);
        rightDoorOpenPosition = rightDoorClosedPosition + (Vector3.forward * openDistance);

        // Subscribe to puzzle completion event
        PuzzleChecker.OnAllPuzzlesCompleted += OnAllPuzzlesCompleted;
    }

    private void OnDestroy()
    {
        PuzzleChecker.OnAllPuzzlesCompleted -= OnAllPuzzlesCompleted;
    }

    private void Update()
    {
        // Check player distance
        float currentDistance = Vector3.Distance(transform.position, player.position);
        bool wasInRange = playerInRange;
        playerInRange = currentDistance <= activationDistance;

        // Handle player entering/exiting range
        if (playerInRange)
        {
            timeOutsideRange = 0f;
            if (!wasInRange && shouldOpen)
            {
                // Player just entered range
                isOpen = true;
            }
        }
        else
        {
            // Player is outside range
            if (wasInRange)
            {
                // Player just exited range
                timeOutsideRange = 0f;
            }
            else
            {
                timeOutsideRange += Time.deltaTime;
            }
        }

        // Update door positions
        if (isOpen)
        {
            // Move doors to open positions
            leftDoor.position = Vector3.MoveTowards(leftDoor.position, leftDoorOpenPosition, moveSpeed * Time.deltaTime);
            rightDoor.position = Vector3.MoveTowards(rightDoor.position, rightDoorOpenPosition, moveSpeed * Time.deltaTime);

            // Check if we should start closing
            if (!playerInRange && timeOutsideRange >= closeDelay)
            {
                isOpen = false;
            }
        }
        else
        {
            // Move doors to closed positions
            leftDoor.position = Vector3.MoveTowards(leftDoor.position, leftDoorClosedPosition, moveSpeed * Time.deltaTime);
            rightDoor.position = Vector3.MoveTowards(rightDoor.position, rightDoorClosedPosition, moveSpeed * Time.deltaTime);

            // Check if we should open again (player returned while closing)
            if (playerInRange && shouldOpen)
            {
                isOpen = true;
                timeOutsideRange = 0f;
            }
        }
    }

    private void OnAllPuzzlesCompleted()
    {
        shouldOpen = true;
        Debug.Log("Door received puzzle completion event - ready to open when player is near");
    }

    // Visualize activation distance in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}