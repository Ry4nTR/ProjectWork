using UnityEngine;
using System;

namespace ProjectWork
{
    public class Window : InteractableObject
    {
        public Transform peekTarget;
        private WindowPeekController peekController;
        [SerializeField] private float _peekDistance = 15f;
        private bool isPeeking = false;
        private Collider windowCollider;

        public float PeekDistance => _peekDistance;

        // Events for spawning control
        public static event Action OnPeekStarted;
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
        }

        private void OnDestroy()
        {
            TutorialTaskChecker.OnDayPassed -= HandleInteraction;
        }

        public override void Interact()
        {
            if (!CanInteract) return;

            if (!isPeeking)
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
            // If the player has finished the tutorial, we can unlock the interaction
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
            if (peekController == null) return;

            windowCollider.enabled = false;

            isPeeking = true;
            peekController.StartPeek(this);
            OnPeekStarted?.Invoke(); // Trigger spawn start
        }

        public void ForceEndPeek()
        {
            if (isPeeking)
            {
                isPeeking = false;
                windowCollider.enabled = true;
                OnPeekEnded?.Invoke(); // Trigger spawn stop
                InvokeInteractionFinishedEvent();
            }
        }
    }
}