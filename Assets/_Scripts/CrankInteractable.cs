using ProjectWork;
using UnityEngine;

public class CrankInteractable : MonoBehaviour
{
    public Transform hangarDoor;               // Reference to the door that will be moved
    public float maxDoorMove = 5f;             // Maximum movement distance for the door
    public Vector3 doorClosedPosition;         // Position when door is fully closed
    public Vector3 doorOpenPosition;           // Position when door is fully open
    public float rotationToCloseRatio = 1f;    // Ratio of crank rotation to door movement
    public float blockAtCompletion = 0.3f;     // Percentage of completion at which to block the crank
    public InteractableObject unlockObject;    // Object that needs to be interacted with to unlock the crank

    private bool isBlocked = false;            // Whether the crank is currently blocked
    private bool hasBeenUnlocked = false;      // Whether the crank has been unlocked at least once
    private Camera cam;                        // Main camera reference
    private bool isInteracting = false;        // Whether the player is currently interacting with the crank
    private float totalRotation = 0f;          // Total accumulated rotation of the crank
    private Vector3 previousMouseDirection;    // Previous mouse direction for rotation calculation


    /// <summary>
    /// Initializes references and sets the initial open position of the door.
    /// </summary>
    void Start()
    {
        cam = Camera.main;
        doorOpenPosition = hangarDoor.localPosition;
    }

    /// <summary>
    /// Handles input and interaction updates each frame.
    /// </summary>
    void Update()
    {
        HandleInput();
    }

    /// <summary>
    /// Manages mouse input for interacting with the crank.
    /// </summary>
    void HandleInput()
    {
        // Start interaction when left mouse button is pressed on the crank
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f))
            {
                if (hit.transform == transform)
                {
                    isInteracting = true;
                    previousMouseDirection = GetMouseDirection();
                }
            }
        }

        // End interaction when left mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            isInteracting = false;
        }

        // Handle crank rotation while interacting and not blocked
        if (isInteracting && !isBlocked)
        {
            Vector3 currentMouseDirection = GetMouseDirection();
            float angleDelta = -Vector3.SignedAngle(previousMouseDirection, currentMouseDirection, transform.up);
            previousMouseDirection = currentMouseDirection;

            // Rotate the crank
            transform.Rotate(Vector3.up, -angleDelta, Space.Self);

            // Update and clamp total rotation
            totalRotation += angleDelta;
            totalRotation = Mathf.Clamp(totalRotation, 0, rotationToCloseRatio * maxDoorMove);

            // Calculate completion percentage
            float completion = totalRotation / (rotationToCloseRatio * maxDoorMove);

            // Check if crank should be blocked
            if (!isBlocked && !hasBeenUnlocked && completion >= blockAtCompletion)
            {
                isBlocked = true;
                unlockObject.UnlockInteraction();
                Debug.Log("Porta bloccata! Serve interazione.");
                BlackScreenTextController.Instance.ActivateBlackScreen("Qualcosa blocca la porta...");
            }

            // Update door position based on completion
            hangarDoor.localPosition = Vector3.Lerp(doorOpenPosition, doorClosedPosition, completion);
        }
    }

    /// <summary>
    /// Calculates the normalized direction from the crank to the mouse position in screen space.
    /// </summary>
    Vector3 GetMouseDirection()
    {
        Vector3 crankScreenPos = cam.WorldToScreenPoint(transform.position);
        Vector3 dir = Input.mousePosition - crankScreenPos;
        return dir.normalized;
    }

    /// <summary>
    /// Unlocks the crank and prevents it from being blocked again.
    /// </summary>
    public void UnlockCrank()
    {
        isBlocked = false;
        hasBeenUnlocked = true;
        Debug.Log("Porta sbloccata!");
    }
}