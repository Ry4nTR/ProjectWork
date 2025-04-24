using System;
using System.Collections;
using UnityEngine;

namespace ProjectWork
{
    public class WindowPeekController : MonoBehaviour
    {
        // Events
        public static event Action<float> OnPeekStarted = delegate { };
        public static event Action OnPeekEnded = delegate { };

        // References
        [Header("References")]
        [SerializeField] private Collider npcCollider;
        private Transform playerCamera;
        private MyCharacterController movementScript;
        private CameraManager cameraManager;
        private DialogueInteractor dialogueInteractor;
        private DialogueManager dialogueManager;
        

        // Peek Settings
        [Header("Peek Settings")]
        [Tooltip("Max horizontal rotation (degrees)")] public float maxYaw = 30f;
        [Tooltip("Max vertical rotation (degrees)")] public float maxPitch = 20f;
        [Tooltip("Mouse rotation speed")] public float rotationSpeed = 2f;
        [Tooltip("Peek transition speed")] public float transitionSpeed = 3f;
        [Tooltip("View centering speed")] public float centerViewSpeed = 5f;
        [Tooltip("Center completion threshold")] public float centerThreshold = 0.5f;

        // Private variables
        private Vector3 originalCamPos;
        private Quaternion originalCamRot;
        private Window currentWindow;
        private Vector2 rotationInput;
        private bool isTransitioning = false;
        private bool shouldCenterView = false;
        private Quaternion targetCenterRotation;
        

        // Public properties
        public bool IsPeeking { get; private set; }

        private void Awake()
        {
            npcCollider.enabled = false;
            cameraManager = GetComponentInChildren<CameraManager>();
            playerCamera = cameraManager.transform;

            movementScript = GetComponent<MyCharacterController>();
            dialogueInteractor = GetComponent<DialogueInteractor>();
            dialogueManager = DialogueManager.Instance;
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

            HandleInput();
        }

        // Public methods
        public void StartPeek(Window window)
        {
            if (IsPeeking || isTransitioning || window == null || window.peekTarget == null) return;

            currentWindow = window;
            targetCenterRotation = window.peekTarget.rotation;
            StartCoroutine(PeekCoroutine());
        }

        public void EndPeek()
        {
            if (!IsPeeking || isTransitioning || !CanExitPeek()) return;
            StartCoroutine(EndPeekCoroutine());
        }

        public bool CanExitPeek()
        {
            return dialogueManager == null || !dialogueManager.IsDialogueActive();
        }

        // Coroutines
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
            npcCollider.enabled = true;
            OnPeekStarted?.Invoke(currentWindow.PeekDistance);
        }

        private IEnumerator EndPeekCoroutine()
        {
            npcCollider.enabled = false;
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

        // Private methods
        private void HandleInput()
        {
            if (dialogueManager != null && dialogueManager.IsDialogueActive()) return;

            if (Input.GetKeyDown(KeyCode.E) && !isTransitioning)
            {
                if (TryInteractWithNPC()) return;
                EndPeek();
            }

            if (Input.GetMouseButtonDown(0)) // Shoot laser
            {
                if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hit, 100f))
                {
                    Debug.Log("Window peek, Hit: " + hit.collider.name);
                    if (hit.collider.CompareTag("Asteroid"))
                    {
                        hit.collider.GetComponent<Asteroid>().DestroyByPlayer();
                    }
                }
            }
        }

        private bool TryInteractWithNPC()
        {
            if (dialogueInteractor == null || !dialogueInteractor.IsLookingAtNPC()) return false;

            DialogueTrigger npc = dialogueInteractor.GetCurrentNPCDialogueTrigger();
            if (npc == null || npc.dialogue == null || npc.dialogueText == null) return false;

            npc.TriggerDialogue();
            return true;
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

            rotationInput.x = Mathf.Clamp(rotationInput.x, -maxYaw, maxYaw);
            rotationInput.y = Mathf.Clamp(rotationInput.y, -maxPitch, maxPitch);

            Quaternion yaw = Quaternion.AngleAxis(rotationInput.x, Vector3.up);
            Quaternion pitch = Quaternion.AngleAxis(rotationInput.y, Vector3.right);

            playerCamera.rotation = targetCenterRotation * yaw * pitch;
        }

        private void TogglePlayerControls(bool enable)
        {
            if (movementScript != null) movementScript.enabled = enable;
            if (cameraManager != null) cameraManager.canRotate = enable;
        }
    }
}