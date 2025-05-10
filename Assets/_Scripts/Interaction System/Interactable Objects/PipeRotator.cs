using System;
using System.Collections;
using UnityEngine;

namespace ProjectWork
{
    /// <summary>
    /// This class is used to handle pipe rotation for the enigma.
    /// Allows smooth rotation around a specified axis.
    /// </summary>
    public class PipeRotator : InteractableObject
    {
        public event Action OnPipeStartRotation = delegate { };
        public event Action OnPipeEndRotation = delegate { };

        [Header("Rotation settings")]
        [SerializeField] private GameObject objToRotate;

        [Tooltip("Degrees per second for smooth rotation")]
        [SerializeField] private float rotationSpeed = 90f;

        [Tooltip("Total degrees to rotate per interaction")]
        [SerializeField] private float rotationAngle = 90f;      

        [Tooltip("Axis to rotate around (e.g., 0 0 1 for Z-axis)")]
        [SerializeField] private Vector3 rotationAxis = Vector3.forward;
        private bool _isRotating = false;

        [Header("References")]
        private Collider meshCollider;

        private bool IsRotating 
        { 
            get => _isRotating;
            set
            {
                _isRotating = value;
                if(_isRotating)
                {
                    OnPipeStartRotation?.Invoke();
                    SetColliderActivation(false);
                }
                else
                {
                    OnPipeEndRotation?.Invoke();
                    SetColliderActivation(true);
                }                
            }
        }

        private void Awake()
        {
            meshCollider = GetComponent<Collider>();

            PipePuzzle.OnPuzzleCompleted += LockInteraction;
        }

        private void OnDestroy()
        {
            PipePuzzle.OnPuzzleCompleted -= LockInteraction;
        }

        public override void Interact()
        {
            if (!CanInteract)
                return;

            if (!IsRotating)
                StartCoroutine(RotatePipe(objToRotate));
        }

        private IEnumerator RotatePipe(GameObject gameObjectToRotate)
        {
            IsRotating = true;

            Quaternion startRotation = gameObjectToRotate.transform.rotation;
            Quaternion targetRotation = startRotation * Quaternion.AngleAxis(rotationAngle, rotationAxis.normalized);

            float elapsed = 0f;
            float duration = rotationAngle / rotationSpeed;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                gameObjectToRotate.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
                yield return null;
            }

            gameObjectToRotate.transform.rotation = targetRotation;
            IsRotating = false;
        }

        /// <summary>
        /// It is used to avoid showing UI "Rotate" prompt when rotating
        /// </summary>
        private void SetColliderActivation(bool isActive)
        {
            meshCollider.enabled = isActive;
        }
    }

}
