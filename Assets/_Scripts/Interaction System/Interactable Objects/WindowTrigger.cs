using UnityEngine;
using System;

namespace ProjectWork
{
    public class WindowTrigger : InteractableObject
    {
        public Transform peekTarget;
        private WindowPeekController peekController;
        [SerializeField] private float _peekDistance = 15f;
        private bool isPeeking = false;
        private Collider windowCollider;
        public bool hasProgressBar = false;
        private bool puzzleCompleted = false; // New flag to track completion

        public float PeekDistance => _peekDistance;

        // Events for spawning control
        public static event Action<WindowTrigger> OnPeekStarted;
        public static event Action OnPeekEnded;

        private void Awake()
        {
            windowCollider = GetComponent<Collider>();
            peekController = FindFirstObjectByType<WindowPeekController>();
        }

        protected override void Start()
        {
            base.Start();
            TutorialTaskChecker.OnDayPassed += HandleInteraction;

            // Subscribe to puzzle completion event
            if (hasProgressBar)
            {
                ProgressBar.OnPuzzleCompleted += HandlePuzzleCompleted;
            }
        }

        private void OnDestroy()
        {
            TutorialTaskChecker.OnDayPassed -= HandleInteraction;

            // Unsubscribe from puzzle completion event
            if (hasProgressBar)
            {
                ProgressBar.OnPuzzleCompleted -= HandlePuzzleCompleted;
            }
        }

        // New method to handle puzzle completion
        private void HandlePuzzleCompleted()
        {
            puzzleCompleted = true;
            LockInteraction(); // Permanently disable interaction
        }

        protected override void InteractChild()
        {
            if (!isPeeking && !puzzleCompleted) // Check puzzleCompleted flag
            {
                StartPeek();
            }
        }

        private void HandleInteraction(bool areDaysFinished)
        {
            if (!areDaysFinished)
            {
                return;
            }
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
            if (peekController == null || puzzleCompleted) return; // Additional check

            windowCollider.enabled = false;
            isPeeking = true;
            peekController.StartPeek(this);
            OnPeekStarted?.Invoke(this);
        }

        public void ForceEndPeek()
        {
            if (isPeeking)
            {
                isPeeking = false;
                windowCollider.enabled = true;
                OnPeekEnded?.Invoke();
                InvokeInteractionFinishedEvent();
            }
        }
    }
}