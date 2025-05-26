using System;
using System.Collections;
using UnityEngine;

namespace ProjectWork
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(MeshRenderer))]
    public class FireExtinguisher : InteractableObject
    {
        [Serializable]
        private struct BasicObject
        {
            public Collider collider;
            public MeshRenderer meshRenderer;
        }
        public static event Action OnFireExtinguisherPickedUp = delegate { };
        public event Action<float> OnRefillTimerChanged = delegate { };
        public event Action OnRefillTimerFinished = delegate { };
        public event Action OnRefillTimerStopped = delegate { };

        [Header("Fire Extinguisher Settings")]
        [SerializeField, Min(.1f)] private float timeToRefill = 2f;
        private float _currentTimeToRefill = 0f;

        [Header("References")]
        [SerializeField] private BasicObject glassObj;
        private BasicObject fireExtObj;

        public float CurrentTimeToRefill
        {
            get => _currentTimeToRefill;
            private set
            {
                _currentTimeToRefill = value;
                OnRefillTimerChanged?.Invoke(_currentTimeToRefill/timeToRefill);

                if(_currentTimeToRefill <= 0f)
                {
                    _currentTimeToRefill = 0f;
                    OnRefillTimerFinished?.Invoke();
                }
            }
        }

        private void Awake()
        {
            fireExtObj.collider = GetComponent<Collider>();
            fireExtObj.meshRenderer = GetComponent<MeshRenderer>();

            FirePuzzle.OnPuzzleCompleted += AbortOperations;
        }

        private void OnDestroy()
        {
            FirePuzzle.OnPuzzleCompleted -= AbortOperations;
        }

        protected override void InteractChild()
        {
            OnFireExtinguisherPickedUp?.Invoke();
            StartCoroutine(StartRefilling());
        }

        private void AbortOperations()
        {
            StopAllCoroutines();
            SetObjectActivation(false);
            LockInteraction();
            OnRefillTimerStopped?.Invoke();
        }

        private void SetObjectActivation(bool isActive)
        {
            SetBasicObjectActivation(ref fireExtObj, isActive);
            SetBasicObjectActivation(ref glassObj, isActive);
        }

        private void SetBasicObjectActivation(ref BasicObject basicObject, bool isActive)
        {
            basicObject.collider.enabled = isActive;
            basicObject.meshRenderer.enabled = isActive;
        }

        private IEnumerator StartRefilling()
        {
            SetObjectActivation(false);
            CurrentTimeToRefill = timeToRefill;
            while (CurrentTimeToRefill > 0f)
            {
                CurrentTimeToRefill -= Time.deltaTime;
                yield return null;
            }
            SetObjectActivation(true);
        }
    }
}