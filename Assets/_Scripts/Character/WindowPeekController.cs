using System.Collections;
using UnityEngine;

public class WindowPeekController : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;
    public MonoBehaviour movementScript;
    public CameraManager cameraManager;

    [Header("Peek Settings")]
    [Tooltip("Massima rotazione orizzontale (gradi)")]
    public float maxYaw = 30f;
    [Tooltip("Massima rotazione verticale (gradi)")]
    public float maxPitch = 20f;
    [Tooltip("Velocità di rotazione con il mouse")]
    public float rotationSpeed = 2f;
    [Tooltip("Velocità transizione in/out peek")]
    public float transitionSpeed = 3f;
    [Tooltip("Velocità centraggio visuale")]
    public float centerViewSpeed = 5f;
    [Tooltip("Soglia per completamento centraggio")]
    public float centerThreshold = 0.5f;

    // Variabili private
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;
    private Window currentWindow;
    private Vector2 rotationInput;
    private bool isTransitioning = false;
    private bool shouldCenterView = false;
    private Quaternion targetCenterRotation;

    public bool IsPeeking { get; private set; }

    public void StartPeek(Window window)
    {
        if (IsPeeking || isTransitioning || window == null || window.peekTarget == null)
            return;

        currentWindow = window;
        targetCenterRotation = window.peekTarget.rotation;
        StartCoroutine(PeekCoroutine());
    }

    public void EndPeek()
    {
        if (!IsPeeking || isTransitioning)
            return;

        StartCoroutine(EndPeekCoroutine());
    }

    private IEnumerator PeekCoroutine()
    {
        isTransitioning = true;
        IsPeeking = true;

        // Salva stato iniziale
        originalCamPos = playerCamera.position;
        originalCamRot = playerCamera.rotation;

        // Disabilita controlli
        TogglePlayerControls(false);

        // Transizione alla posizione di peek
        float t = 0;
        while (t < 1f && IsPeeking)
        {
            t += Time.deltaTime * transitionSpeed;
            playerCamera.position = Vector3.Lerp(originalCamPos, currentWindow.peekTarget.position, t);
            playerCamera.rotation = Quaternion.Slerp(originalCamRot, targetCenterRotation, t);
            yield return null;
        }

        // Prepara centraggio visuale
        if (IsPeeking)
        {
            shouldCenterView = true;
            rotationInput = Vector2.zero;
        }

        isTransitioning = false;
    }

    private void CenterView()
    {
        playerCamera.rotation = Quaternion.Slerp(
            playerCamera.rotation,
            targetCenterRotation,
            Time.deltaTime * centerViewSpeed
        );

        // Completa il centraggio se siamo abbastanza vicini
        if (Quaternion.Angle(playerCamera.rotation, targetCenterRotation) < centerThreshold)
        {
            playerCamera.rotation = targetCenterRotation;
            shouldCenterView = false;
        }
    }

    private IEnumerator EndPeekCoroutine()
    {
        isTransitioning = true;
        shouldCenterView = false;

        // Transizione alla posizione originale
        Vector3 startPos = playerCamera.position;
        Quaternion startRot = playerCamera.rotation;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            playerCamera.position = Vector3.Lerp(startPos, originalCamPos, t);
            playerCamera.rotation = Quaternion.Slerp(startRot, originalCamRot, t);
            yield return null;
        }

        // Ripristina controlli
        TogglePlayerControls(true);

        // Reset stato
        IsPeeking = false;
        currentWindow?.ForceEndPeek();
        currentWindow = null;
        isTransitioning = false;
    }

    private void TogglePlayerControls(bool enable)
    {
        if (movementScript != null)
            movementScript.enabled = enable;

        if (cameraManager != null)
            cameraManager.canRotate = enable;
    }

    private void Update()
    {
        if (!IsPeeking) return;

        if (shouldCenterView)
        {
            CenterView();
        }
        else
        {
            HandlePeekRotation();
        }

        if (Input.GetKeyDown(KeyCode.E) && !isTransitioning)
        {
            EndPeek();
        }

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, 100f))
            {
                if (hit.collider.CompareTag("Asteroid"))
                {
                    Destroy(hit.collider.gameObject);
                    // Increase progress bar (see below)
                }
            }
        }
    }

    private void HandlePeekRotation()
    {
        // Input mouse
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Calcola rotazione
        rotationInput.x += mouseX * rotationSpeed;
        rotationInput.y -= mouseY * rotationSpeed;

        // Applica limiti
        rotationInput.x = Mathf.Clamp(rotationInput.x, -maxYaw, maxYaw);
        rotationInput.y = Mathf.Clamp(rotationInput.y, -maxPitch, maxPitch);

        // Calcola rotazioni
        Quaternion yaw = Quaternion.AngleAxis(rotationInput.x, Vector3.up);
        Quaternion pitch = Quaternion.AngleAxis(rotationInput.y, Vector3.right);

        // Applica rotazione mantenendo il target come centro
        playerCamera.rotation = targetCenterRotation * yaw * pitch;
    }
}