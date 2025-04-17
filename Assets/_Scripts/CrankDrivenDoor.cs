using UnityEngine;

public class CrankInteractable : MonoBehaviour
{
    public Transform hangarDoor;
    public float maxDoorMove = 5f;
    public Vector3 doorClosedPosition;
    public Vector3 doorOpenPosition;
    public float rotationToCloseRatio = 1f;

    private Camera cam;
    private bool isInteracting = false;
    private float totalRotation = 0f;

    private Vector3 previousMouseDirection;

    void Start()
    {
        cam = Camera.main;
        doorOpenPosition = hangarDoor.localPosition;
    }

    void Update()
    {
        HandleInput();
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

        if (isInteracting)
        {
            Vector3 currentMouseDirection = GetMouseDirection();
            float angleDelta = Vector3.SignedAngle(previousMouseDirection, currentMouseDirection, transform.up);
            previousMouseDirection = currentMouseDirection;

            // Rotazione manopola sull'asse Y locale
            transform.Rotate(Vector3.up, angleDelta, Space.Self);

            // Accumula rotazione
            totalRotation -= angleDelta;
            totalRotation = Mathf.Clamp(totalRotation, 0, rotationToCloseRatio * maxDoorMove);

            // Percentuale completamento
            float completion = totalRotation / (rotationToCloseRatio * maxDoorMove);

            // Muovi porta
            hangarDoor.localPosition = Vector3.Lerp(doorOpenPosition, doorClosedPosition, completion);
        }
    }

    Vector3 GetMouseDirection()
    {
        Vector3 crankScreenPos = cam.WorldToScreenPoint(transform.position);
        Vector3 dir = Input.mousePosition - crankScreenPos;
        return dir.normalized;
    }
}
