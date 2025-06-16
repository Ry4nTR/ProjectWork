using System;
using UnityEngine;

namespace ProjectWork
{
    [RequireComponent(typeof(Collider))]
    public class Prisoner : InteractableObject
    {
        public static event Action<Prisoner> OnDialogueFinished = delegate { };

        [Header("Window Settings")]
        [SerializeField] private WindowTrigger connectedWindow;
        [SerializeField] private bool forcePeekExit = true;

        private Collider npcCollider;
        private WindowPeekController peekController;

        private void Awake()
        {
            npcCollider = GetComponent<Collider>();
            peekController = FindObjectOfType<WindowPeekController>();
        }

        private void OnEnable()
        {
            DialogueManager.OnDialogueStarted += DisableCollider;
            DialogueManager.OnDialogueFinished += HandleDialogueFinished;
            WindowPeekController.OnPeekStarted += HandlePeekStarted;
            WindowPeekController.OnPeekEnded += HandlePeekEnded;
        }

        private void OnDisable()
        {
            DialogueManager.OnDialogueStarted -= DisableCollider;
            DialogueManager.OnDialogueFinished -= HandleDialogueFinished;
            WindowPeekController.OnPeekStarted -= HandlePeekStarted;
            WindowPeekController.OnPeekEnded -= HandlePeekEnded;
        }

        protected override void Start()
        {
            DisableCollider();
        }

        private void HandlePeekStarted(WindowTrigger window, float distance)
        {
            npcCollider.enabled = (window == connectedWindow);
        }

        private void HandlePeekEnded()
        {
            DisableCollider();
        }

        private void DisableCollider()
        {
            npcCollider.enabled = false;
        }

        private void HandleDialogueFinished(InteractableObject character)
        {
            npcCollider.enabled = true;

            if (!forcePeekExit) return;

            if (connectedWindow != null && connectedWindow.isPeeking)
            {
                connectedWindow.ForceEndPeek();
            }

            if (peekController != null && peekController.IsPeeking)
            {
                peekController.EndPeek();
            }

            if (character == this)
            {
                OnDialogueFinished?.Invoke(this);
            }
        }
    }
}