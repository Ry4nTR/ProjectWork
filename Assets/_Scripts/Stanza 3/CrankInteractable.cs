using ProjectWork;
using UnityEngine;

public class CrankInteractable : MonoBehaviour
{
    public Transform hangarDoor;
    public float maxDoorMove = 5f;
    public Vector3 doorClosedPosition;
    public Vector3 doorOpenPosition;
    public float rotationToCloseRatio = 1f;
    public float blockAtCompletion = 0.3f;
    public InteractableObject unlockObject;
    public float returnSpeed = 50f;

    private bool isBlocked = false;
    private bool hasBeenUnlocked = false;
    private Camera cam;
    private bool isInteracting = false;
    private float totalRotation = 0f;
    private Vector3 previousMouseDirection;
    private Quaternion originalRotation;

    void Start()
    {
        cam = Camera.main;
        doorOpenPosition = hangarDoor.localPosition;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        HandleInput();

        // Return to original position when not interacting
        if (!isInteracting && totalRotation > 0f && !isBlocked)
        {
            float returnAmount = returnSpeed * Time.deltaTime;
            float newRotation = Mathf.Max(0f, totalRotation - returnAmount);
            float rotationDelta = totalRotation - newRotation;
            totalRotation = newRotation;

            // Rotate the crank back to original position (clockwise - door opening direction)
            transform.Rotate(Vector3.up, rotationDelta, Space.Self);

            // Update door position (opening)
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
                    isInteracting = true;
                    previousMouseDirection = GetMouseDirection();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isInteracting = false;
        }

        if (isInteracting && !isBlocked)
        {
            Vector3 currentMouseDirection = GetMouseDirection();
            float angleDelta = Vector3.SignedAngle(previousMouseDirection, currentMouseDirection, transform.up);
            previousMouseDirection = currentMouseDirection;

            if (angleDelta > 0 || totalRotation > 0)
            {
                // Rotate the crank counter-clockwise (door closing direction)
                transform.Rotate(Vector3.up, -angleDelta, Space.Self);

                totalRotation += angleDelta;
                totalRotation = Mathf.Clamp(totalRotation, 0, rotationToCloseRatio * maxDoorMove);

                UpdateDoorPosition();

                if (!isBlocked && !hasBeenUnlocked && GetCompletion() >= blockAtCompletion)
                {
                    isBlocked = true;
                    unlockObject.UnlockInteraction();
                    Debug.Log("Door blocked! Needs interaction.");
                }
            }
        }
    }

    void UpdateDoorPosition()
    {
        float completion = GetCompletion();
        // This lerp direction works for both closing and opening
        hangarDoor.localPosition = Vector3.Lerp(doorOpenPosition, doorClosedPosition, completion);
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
        Debug.Log("Door unlocked!");
    }
}