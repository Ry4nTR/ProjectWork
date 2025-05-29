using UnityEngine;
using ProjectWork;

public class CrankInteractable : InteractableObject
{

    [Header("UI Settings")]
    [SerializeField] private DoorStatusUI doorStatusUI;

    [Header("Button Settings")]
    [SerializeField] private UnlockCrankButton unlockButton;

    [Header("Stuck Object")]
    [SerializeField] private GameObject stuckObject; // Reference to the object that appears when blocked

    [Header("Card Requirement")]
    [SerializeField] private BlackScreenData noCardBlackScreenData; // Message when player doesn't have card

    public Transform hangarDoor;
    public float maxDoorMove = 5f;
    public Vector3 doorClosedPosition;
    public Vector3 doorOpenPosition;
    public float rotationToCloseRatio = 1f;
    public float blockAtCompletion = 0.3f;
    public InteractableObject unlockObject;
    public float returnSpeed = 50f;

    public bool IsBlocked => isBlocked;


    private bool isBlocked = false;
    private bool hasBeenUnlocked = false;
    private bool isFullyClosed = false; // New flag for closed position
    private bool isSystemActivated = false; // New flag for system activation

    // Public properties to check states
    public bool IsSystemActivated => isSystemActivated;
    public bool HasBeenUnlocked => hasBeenUnlocked;
    private Camera cam;
    private bool isInteracting = false;
    private float totalRotation = 0f;
    private Vector3 previousMouseDirection;
    private Quaternion originalRotation;
    [SerializeField] private BlackScreenData doorBlockedBlackScreenData;

    protected override void Start()
    {
        cam = Camera.main;
        doorOpenPosition = hangarDoor.localPosition;
        originalRotation = transform.rotation;

        // Update interaction prompt based on card status
        UpdateInteractionPrompt();

        // Subscribe to card pickup event to update prompt
        Card.OnCardPickedUp += UpdateInteractionPrompt;
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        Card.OnCardPickedUp -= UpdateInteractionPrompt;
    }

    private void UpdateInteractionPrompt()
    {
        if (!Card.HasCard)
        {
            SetInteractionPrompt("Need Access Card");
        }
        else if (!isSystemActivated)
        {
            SetInteractionPrompt("System Not Activated");
        }
        else
        {
            SetInteractionPrompt("Turn Crank");
        }
    }

    void Update()
    {
        HandleInput();

        // Return to original position when not interacting and not fully closed
        if (!isInteracting && totalRotation > 0f && !isBlocked && !isFullyClosed)
        {
            float returnAmount = returnSpeed * Time.deltaTime;
            float newRotation = Mathf.Max(0f, totalRotation - returnAmount);
            float rotationDelta = totalRotation - newRotation;
            totalRotation = newRotation;

            transform.Rotate(Vector3.up, rotationDelta, Space.Self);
            UpdateDoorPosition();
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f))
            {
                if (hit.transform == transform)
                {
                    // Check if player has the card before allowing interaction
                    if (!Card.HasCard)
                    {
                        Debug.Log("You need an access card to operate this mechanism!");

                        // Show "no card" message
                        if (BlackScreenTextController.Instance != null && noCardBlackScreenData != null)
                        {
                            BlackScreenTextController.Instance.ActivateBlackScreen(noCardBlackScreenData);
                        }

                        return; // Don't start interaction without card
                    }

                    // Check if system is activated
                    if (!isSystemActivated)
                    {
                        Debug.Log("The crank system needs to be activated first. Use the activation button.");

                        // Show "system not activated" message (you can create a new BlackScreenData for this)
                        if (BlackScreenTextController.Instance != null && noCardBlackScreenData != null)
                        {
                            BlackScreenTextController.Instance.ActivateBlackScreen(noCardBlackScreenData);
                        }

                        return; // Don't start interaction if system not activated
                    }

                    isInteracting = true;
                    previousMouseDirection = GetMouseDirection();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isInteracting = false;
        }

        if (isInteracting && !isBlocked && !isFullyClosed)
        {
            Vector3 currentMouseDirection = GetMouseDirection();
            float angleDelta = Vector3.SignedAngle(previousMouseDirection, currentMouseDirection, transform.up);

            // Only process movement if rotating in correct direction (closing) or if trying to open but have progress
            if (angleDelta > 0 || totalRotation > 0)
            {
                // Only allow rotation in the correct direction (closing)
                if (angleDelta > 0)
                {
                    // Rotate the crank counter-clockwise (door closing direction)
                    transform.Rotate(Vector3.up, -angleDelta, Space.Self);
                    totalRotation += angleDelta;
                    totalRotation = Mathf.Clamp(totalRotation, 0, rotationToCloseRatio * maxDoorMove);
                    UpdateDoorPosition();
                }

                previousMouseDirection = currentMouseDirection;

                if (!isBlocked && !hasBeenUnlocked && GetCompletion() >= blockAtCompletion)
                {
                    isBlocked = true;

                    // Enable the stuck object
                    if (stuckObject != null)
                    {
                        stuckObject.SetActive(true);
                    }

                    // Enable the button interaction
                    if (unlockButton != null)
                    {
                        unlockButton.EnableForUnlock();
                    }

                    // Update UI
                    if (doorStatusUI != null)
                    {
                        doorStatusUI.SetBlockedStatus();
                    }

                    Debug.Log("Door blocked! Needs interaction.");

                    if (BlackScreenTextController.Instance != null && doorBlockedBlackScreenData != null)
                    {
                        BlackScreenTextController.Instance.ActivateBlackScreen(doorBlockedBlackScreenData);
                    }
                }
            }
        }
    }

    void UpdateDoorPosition()
    {
        float completion = GetCompletion();
        hangarDoor.localPosition = Vector3.Lerp(doorOpenPosition, doorClosedPosition, completion);

        // Check if we've reached the closed position
        isFullyClosed = completion >= 0.99f;

        if (isFullyClosed && doorStatusUI != null)
        {
            doorStatusUI.SetCompletedStatus();
        }
    }

    float GetCompletion()
    {
        return totalRotation / (rotationToCloseRatio * maxDoorMove);
    }

    Vector3 GetMouseDirection()
    {
        Vector3 crankScreenPos = cam.WorldToScreenPoint(transform.position);
        Vector3 dir = Input.mousePosition - crankScreenPos;
        return dir.normalized;
    }

    public void UnlockCrank()
    {
        isBlocked = false;
        hasBeenUnlocked = true;

        // Disable the stuck object
        if (stuckObject != null)
        {
            stuckObject.SetActive(false);
        }

        // Return UI to normal after unlocking
        if (doorStatusUI != null)
        {
            doorStatusUI.ResetToNormal();
        }
    }

    // New method to activate the crank system
    public void ActivateSystem()
    {
        isSystemActivated = true;
        UpdateInteractionPrompt();
        Debug.Log("Crank system activated! You can now operate the door mechanism.");
    }

    public void ResetDoor()
    {
        isFullyClosed = false;
        isBlocked = false;
        hasBeenUnlocked = false;
        isSystemActivated = false; // Reset system activation
        totalRotation = 0f;
        transform.rotation = originalRotation;
        UpdateDoorPosition();
        UpdateInteractionPrompt(); // Update prompt when resetting
    }
}