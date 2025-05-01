using UnityEngine;

namespace ProjectWork
{
    [RequireComponent(typeof(Collider))]
    public class Prisoner : MonoBehaviour
    {
        private Collider npcCollider;
        [SerializeField] private Window connectedWindow;

        private void Awake()
        {
            npcCollider = GetComponent<Collider>();

            WindowPeekController.OnPeekStarted += EnableCollider;
            WindowPeekController.OnPeekEnded += DisableCollider;
        }

        private void Start()
        {
            DisableCollider();
        }

        private void OnDestroy()
        {
            WindowPeekController.OnPeekStarted -= EnableCollider;
            WindowPeekController.OnPeekEnded -= DisableCollider;
        }

        private void EnableCollider(Window peekingWindow, float peekingDistance) => npcCollider.enabled = peekingWindow == connectedWindow;

        private void DisableCollider() => npcCollider.enabled = false;
    }
}