using System;
using System.Collections;
using UnityEngine;

namespace ProjectWork
{
    public class WindowPeekController : MonoBehaviour
    {
        // ------------------------- Events -------------------------
        [Tooltip("Triggered when peek starts, passing window and distance")]
        public static event Action<WindowTrigger, float> OnPeekStarted = delegate { };

        [Tooltip("Triggered when peek ends")]
        public static event Action OnPeekEnded = delegate { };

        // ----------------------- References -----------------------
        [Header("References")]
        [Tooltip("Player camera transform reference")]
        private Transform playerCamera;

        [Tooltip("Character controller for movement")]
        private MyCharacterController movementScript;

        [Tooltip("Camera manager component")]
        private CameraManager cameraManager;

        [Tooltip("Dialogue manager reference")]
        private DialogueManager dialogueManager;

        // --------------------- Peek Settings ----------------------
        [Header("Default Peek Settings")]
        [Tooltip("Default max horizontal rotation when window doesn't specify")]
        [SerializeField] private float defaultMaxYaw = 30f;

        [Tooltip("Default max vertical rotation when window doesn't specify")]
        [SerializeField] private float defaultMaxPitch = 20f;

        [Space(5)]
        [Tooltip("Mouse rotation sensitivity")]
        [SerializeField] private float rotationSpeed = 2f;

        [Tooltip("Transition speed when entering/exiting peek mode")]
        [SerializeField] private float transitionSpeed = 3f;

        [Tooltip("Speed when centering view to window")]
        [SerializeField] private float centerViewSpeed = 5f;

        [Tooltip("Threshold angle (degrees) to complete centering")]
        [SerializeField] private float centerThreshold = 0.5f;

        // ------------------- Runtime Variables --------------------
        private Vector3 originalCamPos;
        private Quaternion originalCamRot;
        private WindowTrigger currentWindow;
        private Vector2 rotationInput;
        private bool canListenInput = true;
        private bool isTransitioning = false;
        private bool shouldCenterView = false;
        private Quaternion targetCenterRotation;
        private bool exitLocked = false;

        // ------------------- Public Properties --------------------
        [Header("Status")]
        [Tooltip("Is the player currently peeking through a window?")]
        [SerializeField] private bool isPeeking;
        public bool IsPeeking { get => isPeeking; private set => isPeeking = value; }

        private void Awake()
        {
            cameraManager = GetComponentInChildren<CameraManager>();
            playerCamera = cameraManager.transform;
            movementScript = GetComponent<MyCharacterController>();

            DialogueManager.OnDialogueStarted += LockInput;
            DialogueManager.OnDialogueFinished += UnlockInput;
        }

        private void Start()
        {
            dialogueManager = DialogueManager.Instance;
            UnlockInput();
        }

        private void Update()
        {
            if (!IsPeeking || PauseHandler.IsPaused) return;

            if (shouldCenterView)
            {
                CenterView();
            }
            else
            {
                HandlePeekRotation();
            }
        }

        private void LateUpdate()
        {
            if (PauseHandler.IsPaused) return;
            HandleInput();
        }

        private void OnDestroy()
        {
            DialogueManager.OnDialogueStarted -= LockInput;
            DialogueManager.OnDialogueFinished -= UnlockInput;
        }

        /// <summary>
        /// Disables input listening (e.g., during dialogue)
        /// </summary>
        private void LockInput()
        {
            canListenInput = false;
        }

        /// <summary>
        /// Enables input listening
        /// </summary>
        private void UnlockInput()
        {
            canListenInput = true;
        }

        // -------------------- Public Methods ----------------------

        /// <summary>
        /// Starts the peek interaction at specified window
        /// </summary>
        /// <param name="window">Window to peek through</param>
        public void StartPeek(WindowTrigger window)
        {
            if (IsPeeking || isTransitioning || window == null || window.peekTarget == null) return;

            currentWindow = window;
            targetCenterRotation = window.peekTarget.rotation;
            StartCoroutine(PeekCoroutine());
        }

        /// <summary>
        /// Ends the current peek interaction
        /// </summary>
        public void EndPeek()
        {
            if (!IsPeeking || isTransitioning || !CanExitPeek()) return;
            StartCoroutine(EndPeekCoroutine());
        }

        /// <summary>
        /// Checks if peek can be exited (not during dialogue)
        /// </summary>
        public bool CanExitPeek()
        {
            return (dialogueManager == null || !dialogueManager.IsDialogueActive()) && !exitLocked;
        }

        public void LockExit()
        {
            exitLocked = true;
        }

        public void UnlockExit()
        {
            exitLocked = false;
        }

        // --------------------- Coroutines -------------------------

        /// <summary>
        /// Handles the transition into peek mode
        /// </summary>
        private IEnumerator PeekCoroutine()
        {
            isTransitioning = true;
            IsPeeking = true;

            // Save initial state
            originalCamPos = playerCamera.position;
            originalCamRot = playerCamera.rotation;

            // Disable controls
            TogglePlayerControls(false);

            // Transition to peek position
            float t = 0;
            while (t < 1f && IsPeeking)
            {
                t += Time.deltaTime * transitionSpeed;
                playerCamera.position = Vector3.Lerp(originalCamPos, currentWindow.peekTarget.position, t);
                playerCamera.rotation = Quaternion.Slerp(originalCamRot, targetCenterRotation, t);
                yield return null;
            }

            if (IsPeeking)
            {
                shouldCenterView = true;
                rotationInput = Vector2.zero;
            }

            isTransitioning = false;
            OnPeekStarted?.Invoke(currentWindow, currentWindow.PeekDistance);
        }

        /// <summary>
        /// Handles the transition out of peek mode
        /// </summary>
        private IEnumerator EndPeekCoroutine()
        {
            isTransitioning = true;
            shouldCenterView = false;

            // Transition back to original position
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

            // Restore controls and reset state
            TogglePlayerControls(true);
            IsPeeking = false;
            currentWindow?.ForceEndPeek();
            currentWindow = null;
            isTransitioning = false;
            OnPeekEnded?.Invoke();
        }

        // -------------------- Private Methods --------------------

        /// <summary>
        /// Handles input for exiting peek and shooting
        /// </summary>
        private void HandleInput()
        {
            if (!CanExitPeek()) return;

            if (canListenInput && Input.GetKeyDown(KeyCode.E) && !isTransitioning)
            {
                EndPeek();
            }

            if (Input.GetMouseButtonDown(0)) // Shoot laser
            {
                if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hit, 100f))
                {
                    if (hit.collider.CompareTag("Asteroid"))
                    {
                        hit.collider.GetComponent<Asteroid>().DestroyByPlayer();
                    }
                }
            }
        }

        /// <summary>
        /// Smoothly centers the view to the window's target rotation
        /// </summary>
        private void CenterView()
        {
            playerCamera.rotation = Quaternion.Slerp(
                playerCamera.rotation,
                targetCenterRotation,
                Time.deltaTime * centerViewSpeed
            );

            if (Quaternion.Angle(playerCamera.rotation, targetCenterRotation) < centerThreshold)
            {
                playerCamera.rotation = targetCenterRotation;
                shouldCenterView = false;
            }
        }

        /// <summary>
        /// Handles camera rotation during peek with window-specific limits
        /// </summary>
        private void HandlePeekRotation()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            rotationInput.x += mouseX * rotationSpeed;
            rotationInput.y -= mouseY * rotationSpeed;

            // Use window-specific limits if available, otherwise use defaults
            float yawLimit = currentWindow != null ? currentWindow.maxYaw : defaultMaxYaw;
            float pitchLimit = currentWindow != null ? currentWindow.maxPitch : defaultMaxPitch;

            rotationInput.x = Mathf.Clamp(rotationInput.x, -yawLimit, yawLimit);
            rotationInput.y = Mathf.Clamp(rotationInput.y, -pitchLimit, pitchLimit);

            Quaternion yaw = Quaternion.AngleAxis(rotationInput.x, Vector3.up);
            Quaternion pitch = Quaternion.AngleAxis(rotationInput.y, Vector3.right);

            playerCamera.rotation = targetCenterRotation * yaw * pitch;
        }

        /// <summary>
        /// Toggles player controls on/off during peek transitions
        /// </summary>
        /// <param name="enable">Whether to enable controls</param>
        private void TogglePlayerControls(bool enable)
        {
            if (movementScript != null) movementScript.enabled = enable;
            if (cameraManager != null) cameraManager.canRotate = enable;
        }
    }
}