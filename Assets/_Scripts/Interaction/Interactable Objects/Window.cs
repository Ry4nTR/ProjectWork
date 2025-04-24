using UnityEngine;
using System;

namespace ProjectWork
{

    public class Window : InteractableObject
{
    public Transform peekTarget;
    [SerializeField] private WindowPeekController peekController;
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
    }

        public override void Interact()
        {
            if (!CanInteract) return;

            if (!isPeeking)
            {
                StartPeek();
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
