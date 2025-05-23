using System;
using UnityEngine;

namespace ProjectWork
{
    [RequireComponent(typeof(ParticleSystem))]
    [RequireComponent(typeof(Collider))]
    public class Fire : MonoBehaviour
    {
        public event Action<Fire> OnFireDead = delegate { };

        private const float MAX_INTENSITY = 1f;

        [SerializeField, Range(0f, MAX_INTENSITY)] private float _currentIntensity = 1f;
        [SerializeField, Min(1f)] private float timeToRegenerate = 5f;
        [SerializeField, Min(0)] private float regenRate = 5f;
        private float timeLastWatered = 0f;
        private float[] startIntensities = new float[0];

        private bool isLit = true;

        private ParticleSystem[] fireParticleSystems = new ParticleSystem[0];
        private Collider fireCollider = null;

        public float CurrentIntensity
        {
            get => _currentIntensity;
            private set
            {
                _currentIntensity = Mathf.Clamp(value, 0f, MAX_INTENSITY);
                UpdateVFXsEmissionRates();

                if (_currentIntensity <= 0f)
                {
                    isLit = false;
                    fireCollider.enabled = false;
                    OnFireDead?.Invoke(this);
                }
                else
                {
                    isLit = true;
                    fireCollider.enabled = true;
                }
            }
        }

        private void Awake()
        {
            fireParticleSystems = GetComponentsInChildren<ParticleSystem>(true);
            fireCollider = GetComponent<Collider>();
        }

        private void Start()
        {
            startIntensities = new float[fireParticleSystems.Length];

            for (int i = 0; i < fireParticleSystems.Length; i++)
            {
                startIntensities[i] = fireParticleSystems[i].emission.rateOverTime.constant;
            }
        }

        private void Update()
        {
            if (isLit && CurrentIntensity < MAX_INTENSITY && Time.time - timeLastWatered >= timeToRegenerate)
            {
                CurrentIntensity += regenRate * Time.deltaTime;
            }
        }

        private void UpdateVFXsEmissionRates()
        {
            for(int i = 0; i < fireParticleSystems.Length; i++)
            {
                var emission = fireParticleSystems[i].emission;
                emission.rateOverTime = startIntensities[i] * CurrentIntensity;
            }
        }

        public bool TryExtinguish(float amountToRemove)
        {
            timeLastWatered = Time.time;
            CurrentIntensity -= amountToRemove;

            return CurrentIntensity <= 0f;
        }
    }
}