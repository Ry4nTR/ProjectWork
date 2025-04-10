using System.Collections;
using UnityEngine;

public class WindowPeekController : MonoBehaviour
{
    [Header("Riferimenti manuali")]
    public Transform playerCamera;
    public GameObject interactionUI;
    public MonoBehaviour movementScript;
    public CameraManager cameraManager; // riferimento al tuo script CameraManager


    [Header("Impostazioni")]
    public float rayDistance = 2.5f;
    public LayerMask windowLayer;
    public KeyCode interactKey = KeyCode.E;

    [Header("Limiti di rotazione")]
    public float maxYaw = 30f;    // rotazione orizzontale max (sx-dx)
    public float maxPitch = 20f;  // rotazione verticale max (su-giù)
    public float rotationSpeed = 2f;

    private bool isPeeking = false;
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;
    private Transform peekTarget;
    private GameObject currentWindow;

    private Vector2 rotationInput;
    private Vector2 currentRotation;

    private void Start()
    {
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    private void Update()
    {
        if (!isPeeking)
        {
            HandleLookForWindow();
        }

        if (Input.GetKeyDown(interactKey))
        {
            if (!isPeeking && peekTarget != null)
            {
                StartCoroutine(StartPeek());
            }
            else if (isPeeking)
            {
                StartCoroutine(EndPeek());
            }
        }

        if (isPeeking)
        {
            HandlePeekRotation();
        }
    }

    private void HandleLookForWindow()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, windowLayer))
        {
            if (hit.collider.CompareTag("Window"))
            {
                currentWindow = hit.collider.gameObject;
                peekTarget = currentWindow.transform.Find("PeekTarget");
                if (peekTarget == null) return;

                if (interactionUI != null)
                    interactionUI.SetActive(true);

                return;
            }
        }

        peekTarget = null;
        currentWindow = null;
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    private IEnumerator StartPeek()
    {
        if (cameraManager != null)
            cameraManager.canRotate = false;

        isPeeking = true;
        originalCamPos = playerCamera.position;
        originalCamRot = playerCamera.rotation;
        if (movementScript != null)
            movementScript.enabled = false;
        if (interactionUI != null)
            interactionUI.SetActive(false);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 3f;
            playerCamera.position = Vector3.Lerp(originalCamPos, peekTarget.position, t);
            playerCamera.rotation = Quaternion.Slerp(originalCamRot, peekTarget.rotation, t);
            yield return null;
        }

        currentRotation = Vector2.zero;
    }

    private IEnumerator EndPeek()
    {
        if (cameraManager != null)
            cameraManager.canRotate = true;

        if (originalCamPos == Vector3.zero)
            yield break;

        isPeeking = false;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 3f;
            playerCamera.position = Vector3.Lerp(playerCamera.position, originalCamPos, t);
            playerCamera.rotation = Quaternion.Slerp(playerCamera.rotation, originalCamRot, t);
            yield return null;
        }

        if (movementScript != null)
            movementScript.enabled = true;
    }

    private void HandlePeekRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationInput.x += mouseX * rotationSpeed;
        rotationInput.y -= mouseY * rotationSpeed;

        rotationInput.x = Mathf.Clamp(rotationInput.x, -maxYaw, maxYaw);
        rotationInput.y = Mathf.Clamp(rotationInput.y, -maxPitch, maxPitch);

        Quaternion yaw = Quaternion.AngleAxis(rotationInput.x, Vector3.up);
        Quaternion pitch = Quaternion.AngleAxis(rotationInput.y, Vector3.right);

        playerCamera.localRotation = yaw * pitch;
    }

}
