using System;
using UnityEngine;

namespace ProjectWork
{
    /// <summary>
    /// This class represents a prisoner NPC that can be interacted with. It will be used as item in TutorialTaskManager's checklist
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Prisoner : InteractableObject
    {
        public static event Action<Prisoner> OnDialogueFinished = delegate { };

        private Collider npcCollider;
        [SerializeField] private Window connectedWindow;

        private void Awake()
        {
            npcCollider = GetComponent<Collider>();

            DialogueManager.OnDialogueStarted += DisableCollider;  //Disable the collider when the dialogue starts, so the player can't interact with the NPC while peeking
            DialogueManager.OnDialogueFinished += EnableColliderAndInvokeEvent;  //Instead of subscribing directly TutorialTaskChecker to DialogueManager event, we can use this event to trigger the task completion
            
            WindowPeekController.OnPeekStarted += EnableCollider;
            WindowPeekController.OnPeekEnded += DisableCollider;

            TutorialTaskChecker.OnDayPassed += HandleDeactivation;
        }

        protected override void Start()
        {
            DisableCollider();
        }

        private void OnDestroy()
        {
            DialogueManager.OnDialogueStarted -= DisableCollider;
            DialogueManager.OnDialogueFinished -= EnableColliderAndInvokeEvent;
            
            WindowPeekController.OnPeekStarted -= EnableCollider;
            WindowPeekController.OnPeekEnded -= DisableCollider;

            TutorialTaskChecker.OnDayPassed -= HandleDeactivation;
        }

        private void HandleDeactivation(bool areDaysPassed)
        {
            if(!areDaysPassed)
            {
                return;
            }
            gameObject.SetActive(false);
        }

        private void EnableCollider(Window peekingWindow, float peekingDistance) => npcCollider.enabled = peekingWindow == connectedWindow;

        private void DisableCollider() => npcCollider.enabled = false;

        private void EnableColliderAndInvokeEvent()
        {
            npcCollider.enabled = true;
            OnDialogueFinished?.Invoke(this);
        }
    }
}