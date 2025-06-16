using System;
using System.Collections;
using UnityEngine;

namespace ProjectWork
{
    public class WindowPeekController : MonoBehaviour
    {
        // ------------------------- Events -------------------------
        public static event Action<WindowTrigger, float> OnPeekStarted = delegate { };
        public static event Action OnPeekEnded = delegate { };

        // ----------------------- References -----------------------
        [Header("References")]
        private Transform playerCamera;
        private MyCharacterController movementScript;
        private CameraManager cameraManager;
        private DialogueManager dialogueManager;

        // --------------------- Peek Settings ----------------------
        [Header("Default Peek Settings")]
        [SerializeField] private float defaultMaxYaw = 30f;
        [SerializeField] private float defaultMaxPitch = 20f;
        [Space(5)]
        [SerializeField] private float rotationSpeed = 2f;
        [SerializeField] private float transitionSpeed = 3f;
        [SerializeField] private float centerViewSpeed = 5f;
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
        private Coroutine currentPeekCoroutine;

        // ------------------- Public Properties --------------------
        [Header("Status")]
        [SerializeField] private bool isPeeking;
        public bool IsPeeking => isPeeking;

        private void Awake()
        {
            cameraManager = GetComponentInChildren<CameraManager>();
            playerCamera = cameraManager.transform;
            movementScript = GetComponent<MyCharacterController>();

            DialogueManager.OnDialogueStarted += LockInput;
            DialogueManager.OnDialogueFinished += UnlockInput;
            DialogueManager.OnDialogueFinished += ForceExitIfPeeking;
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
            DialogueManager.OnDialogueFinished -= ForceExitIfPeeking;
        }

        private void LockInput()
        {
            canListenInput = false;
        }

        private void UnlockInput(InteractableObject _ = null)
        {
            canListenInput = true;
        }

        private void ForceExitIfPeeking(InteractableObject _ = null)
        {
            if (IsPeeking)
            {
                Debug.Log("[PeekController] Force exiting peek due to dialogue end");
                EndPeek();
            }
        }

        public void StartPeek(WindowTrigger window)
        {
            if (IsPeeking || isTransitioning || window == null || window.peekTarget == null) return;

            currentWindow = window;
            targetCenterRotation = window.peekTarget.rotation;

            // Stop any existing coroutine
            if (currentPeekCoroutine != null)
            {
                StopCoroutine(currentPeekCoroutine);
            }

            currentPeekCoroutine = StartCoroutine(PeekCoroutine());
        }

        public void EndPeek()
        {
            if (!IsPeeking || isTransitioning) return;

            Debug.Log($"[PeekController] Ending peek");

            // Stop any existing coroutine
            if (currentPeekCoroutine != null)
            {
                StopCoroutine(currentPeekCoroutine);
            }

            currentPeekCoroutine = StartCoroutine(EndPeekCoroutine());
        }

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

        private IEnumerator PeekCoroutine()
        {
            isTransitioning = true;
            isPeeking = true;

            originalCamPos = playerCamera.position;
            originalCamRot = playerCamera.rotation;

            TogglePlayerControls(false);

            float t = 0;
            while (t < 1f && isPeeking)
            {
                t += Time.deltaTime * transitionSpeed;
                playerCamera.position = Vector3.Lerp(originalCamPos, currentWindow.peekTarget.position, t);
                playerCamera.rotation = Quaternion.Slerp(originalCamRot, targetCenterRotation, t);
                yield return null;
            }

            if (isPeeking)
            {
                shouldCenterView = true;
                rotationInput = Vector2.zero;
                OnPeekStarted?.Invoke(currentWindow, currentWindow.PeekDistance);
            }

            isTransitioning = false;
        }

        private IEnumerator EndPeekCoroutine()
        {
            isTransitioning = true;
            shouldCenterView = false;
            isPeeking = false;

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

            TogglePlayerControls(true);
            currentWindow?.ForceEndPeek();
            currentWindow = null;
            isTransitioning = false;
            OnPeekEnded?.Invoke();
        }

        private void HandleInput()
        {
            if (!CanExitPeek()) return;

            if (canListenInput && Input.GetKeyDown(KeyCode.E) && !isTransitioning)
            {
                EndPeek();
            }

            if (Input.GetMouseButtonDown(0))
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

        private void HandlePeekRotation()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            rotationInput.x += mouseX * rotationSpeed;
            rotationInput.y -= mouseY * rotationSpeed;

            float yawLimit = currentWindow?.maxYaw ?? defaultMaxYaw;
            float pitchLimit = currentWindow?.maxPitch ?? defaultMaxPitch;

            rotationInput.x = Mathf.Clamp(rotationInput.x, -yawLimit, yawLimit);
            rotationInput.y = Mathf.Clamp(rotationInput.y, -pitchLimit, pitchLimit);

            playerCamera.rotation = targetCenterRotation *
                                 Quaternion.AngleAxis(rotationInput.x, Vector3.up) *
                                 Quaternion.AngleAxis(rotationInput.y, Vector3.right);
        }

        private void TogglePlayerControls(bool enable)
        {
            if (movementScript != null) movementScript.enabled = enable;
            if (cameraManager != null) cameraManager.canRotate = enable;
        }
    }
}