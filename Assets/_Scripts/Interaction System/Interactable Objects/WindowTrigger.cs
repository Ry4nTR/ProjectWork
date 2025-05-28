using UnityEngine;
using System;

namespace ProjectWork
{
    public class WindowTrigger : InteractableObject
    {
        [Header("Final Decision Settings")]
        [Tooltip("Is this the final decision window?")]
        public bool isFinalDecisionWindow = false;

        [Tooltip("Planet selectors to enable during peek")]
        public PlanetSelector[] planetSelectors;

        // --------------------- Peek Settings ----------------------
        [Header("Peek Settings")]
        [Tooltip("Transform representing the camera's peek position")]
        public Transform peekTarget;

        [Tooltip("View distance when peeking through this window")]
        [SerializeField] private float _peekDistance = 15f;

        [Space(10)]
        [Header("Rotation Limits")]
        [Tooltip("Max horizontal rotation (degrees) for this window")]
        public float maxYaw = 30f;

        [Tooltip("Max vertical rotation (degrees) for this window")]
        public float maxPitch = 20f;

        [Space(10)]
        [Header("Window Features")]
        [Tooltip("Does this window have an associated puzzle?")]
        public bool hasProgressBar = false;

        // --------------------- Runtime State ----------------------
        private WindowPeekController peekController;
        private Collider windowCollider;
        private bool isPeeking = false;
        private bool puzzleCompleted = false;
        private FinalDecisionController decisionController;

        public float PeekDistance => _peekDistance;

        // ----------------------- Events ---------------------------
        [Tooltip("Triggered when peek starts on this window")]
        public static event Action<WindowTrigger> OnPeekStarted;

        [Tooltip("Triggered when peek ends on this window")]
        public static event Action OnPeekEnded;

        private void Awake()
        {
            windowCollider = GetComponent<Collider>();
            peekController = FindFirstObjectByType<WindowPeekController>();
            decisionController = FindAnyObjectByType<FinalDecisionController>();

            // Initialize planet selectors if this is a final decision window
            if (isFinalDecisionWindow && planetSelectors != null)
            {
                foreach (var planetSelector in planetSelectors)
                {
                    planetSelector.windowTrigger = this;
                    planetSelector.SetInteractionEnabled(false); // Start disabled
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            TutorialTaskChecker.OnDayPassed += HandleInteraction;

            if (hasProgressBar)
            {
                ProgressBar.OnPuzzleCompleted += HandlePuzzleCompleted;
            }
        }

        private void OnDestroy()
        {
            TutorialTaskChecker.OnDayPassed -= HandleInteraction;

            if (hasProgressBar)
            {
                ProgressBar.OnPuzzleCompleted -= HandlePuzzleCompleted;
            }
        }

        /// <summary>
        /// Handles puzzle completion - permanently disables interaction
        /// </summary>
        private void HandlePuzzleCompleted()
        {
            puzzleCompleted = true;
            LockInteraction();
        }

        /// <summary>
        /// Custom interaction implementation for windows
        /// </summary>
        protected override void InteractChild()
        {
            if (!isPeeking && !puzzleCompleted)
            {
                StartPeek();
            }
        }

        /// <summary>
        /// Handles day-based interaction availability
        /// </summary>
        private void HandleInteraction(bool areDaysFinished)
        {
            if (!areDaysFinished) return;

            if (canInteractAtStart)
            {
                LockInteraction();
            }
            else
            {
                UnlockInteraction();
            }
        }

        /// <summary>
        /// Initiates the peek interaction
        /// </summary>
        private void StartPeek()
        {
            if (peekController == null || puzzleCompleted) return;

            windowCollider.enabled = false;
            isPeeking = true;
            peekController.StartPeek(this);

            // Enable planet interactions if this is final decision window
            if (isFinalDecisionWindow && planetSelectors != null)
            {
                foreach (var planetSelector in planetSelectors)
                {
                    planetSelector.SetInteractionEnabled(true);
                }
            }

            OnPeekStarted?.Invoke(this);
        }

        /// <summary>
        /// Forces peek to end (called by controller)
        /// </summary>
        public void ForceEndPeek()
        {
            if (isPeeking)
            {
                isPeeking = false;
                windowCollider.enabled = true;

                // Disable planet interactions
                if (isFinalDecisionWindow && planetSelectors != null)
                {
                    foreach (var planetSelector in planetSelectors)
                    {
                        planetSelector.SetInteractionEnabled(false);
                    }
                }

                OnPeekEnded?.Invoke();
                InvokeInteractionFinishedEvent();
            }
        }

        public void SelectPlanet(PlanetSelector planetSelector)
        {
            if (!isFinalDecisionWindow || !isPeeking)
            {
                Debug.LogWarning($"Cannot select planet - isFinalDecisionWindow: {isFinalDecisionWindow}, isPeeking: {isPeeking}");
                return;
            }

            if (planetSelector == null)
            {
                Debug.LogError("Planet selector is null!");
                return;
            }

            if (decisionController == null)
            {
                Debug.LogError("FinalDecisionController is null! Make sure it's in the scene.");
                return;
            }

            decisionController.StartDecision(planetSelector);
        }
    }
}