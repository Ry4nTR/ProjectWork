using UnityEngine;
using System;

namespace ProjectWork
{
    public class WindowTrigger : InteractableObject
    {
        [Header("Final Decision Settings")]
        public bool isFinalDecisionWindow = false;
        public PlanetSelector[] planetSelectors;

        [Header("Peek Settings")]
        public Transform peekTarget;
        [SerializeField] private float _peekDistance = 15f;

        [Header("Rotation Limits")]
        public float maxYaw = 30f;
        public float maxPitch = 20f;

        [Header("Window Features")]
        public bool hasProgressBar = false;

        private WindowPeekController peekController;
        private Collider windowCollider;
        public bool isPeeking = false;
        private bool puzzleCompleted = false;
        private FinalDecisionController decisionController;

        public float PeekDistance => _peekDistance;

        public static event Action<WindowTrigger> OnPeekStarted;
        public static event Action OnPeekEnded;

        private void Awake()
        {
            windowCollider = GetComponent<Collider>();
            peekController = FindObjectOfType<WindowPeekController>();
            decisionController = FindObjectOfType<FinalDecisionController>();

            if (isFinalDecisionWindow && planetSelectors != null)
            {
                foreach (var planetSelector in planetSelectors)
                {
                    planetSelector.windowTrigger = this;
                    planetSelector.SetInteractionEnabled(false);
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            TutorialTaskChecker.OnDayPassed += HandleInteraction;

            if (hasProgressBar)
            {
                ProgressBar.OnSpecificPuzzleCompleted += HandlePuzzleCompleted;
            }
        }

        private void OnDestroy()
        {
            TutorialTaskChecker.OnDayPassed -= HandleInteraction;

            if (hasProgressBar)
            {
                ProgressBar.OnSpecificPuzzleCompleted -= HandlePuzzleCompleted;
            }
        }

        private void HandlePuzzleCompleted(Puzzle specificPuzzleCompleted)
        {
            if (specificPuzzleCompleted.GetType() != typeof(ProgressBar)) return;
            puzzleCompleted = true;
            LockInteraction();
        }

        protected override void InteractChild()
        {
            if (!isPeeking && !puzzleCompleted)
            {
                StartPeek();
            }
        }

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

        private void StartPeek()
        {
            if (peekController == null || puzzleCompleted) return;

            windowCollider.enabled = false;
            isPeeking = true;
            peekController.StartPeek(this);

            if (isFinalDecisionWindow && planetSelectors != null)
            {
                foreach (var planetSelector in planetSelectors)
                {
                    planetSelector.SetInteractionEnabled(true);
                }
            }

            OnPeekStarted?.Invoke(this);
        }

        public void ForceEndPeek()
        {
            if (isPeeking)
            {
                isPeeking = false;
                windowCollider.enabled = true;

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
            if (!isFinalDecisionWindow || !isPeeking || planetSelector == null || decisionController == null) return;
            decisionController.StartDecision(planetSelector);
        }
    }
}