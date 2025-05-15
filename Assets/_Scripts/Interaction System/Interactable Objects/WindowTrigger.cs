// WindowTrigger.cs modifications
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
        public bool hasProgressBar = false; // Add this to identify windows with progress bars

        public float PeekDistance => _peekDistance;

        // Events for spawning control
        public static event Action<WindowTrigger> OnPeekStarted; // Modified to pass window reference
        public static event Action OnPeekEnded;


        public AIDialogue aIDialogue;


        private void Awake()
        {
            windowCollider = GetComponent<Collider>();
            peekController = FindFirstObjectByType<WindowPeekController>();
        }

        protected override void Start()
        {
            AIDialogueManager.Instance.StartDialogue(aIDialogue);
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
            OnPeekStarted?.Invoke(this); // Pass this window reference
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