using UnityEngine;

namespace ProjectWork
{
    public class Asteroid : MonoBehaviour
    {
        // Configuration
        public float speed = 3f;
        private float hitReward = 25f;
        private float collisionPenalty = 10f;

        // References
        private ProgressBar progressBar;
        [HideInInspector] public AsteroidSpawner spawner;
        private bool wasDestroyedByPlayer = false;

        // Enhanced Smoke Effects
        [Header("Enhanced Smoke Settings")]
        public GameObject smokeEffectPrefab;
        public GameObject explosionEffectPrefab;
        public Vector3 smokePositionOffset = new Vector3(0, -0.5f, -0.5f); // Behind and below the asteroid
        public float smokeSizeMultiplier = 2f;
        public float smokeDensity = 1.5f;
        public float smokeTrailLength = 1f;
        public bool useWorldSpace = true;

        private GameObject activeSmokeEffect;
        private ParticleSystem smokeParticles;
        private ParticleSystemRenderer smokeRenderer;

        void Start()
        {
            progressBar = FindFirstObjectByType<ProgressBar>();
            InitializeEnhancedSmokeEffect();
        }

        void InitializeEnhancedSmokeEffect()
        {
            if (smokeEffectPrefab != null)
            {
                // Calculate spawn position with offset
                Vector3 spawnPosition = transform.position + transform.TransformDirection(smokePositionOffset);

                // Instantiate effect
                activeSmokeEffect = Instantiate(smokeEffectPrefab, spawnPosition, transform.rotation);
                activeSmokeEffect.transform.SetParent(transform);

                // Get components
                smokeParticles = activeSmokeEffect.GetComponent<ParticleSystem>();
                smokeRenderer = activeSmokeEffect.GetComponent<ParticleSystemRenderer>();

                if (smokeParticles != null)
                {
                    // Configure for realistic smoke
                    var main = smokeParticles.main;
                    main.simulationSpace = useWorldSpace ? ParticleSystemSimulationSpace.World : ParticleSystemSimulationSpace.Local;
                    main.startSizeMultiplier = smokeSizeMultiplier;
                    main.startLifetime = smokeTrailLength;

                    var emission = smokeParticles.emission;
                    emission.rateOverTime = 20f * smokeDensity;

                    var shape = smokeParticles.shape;
                    shape.angle = 25f; // Wider emission angle

                    var velocity = smokeParticles.velocityOverLifetime;
                    velocity.enabled = true;
                    velocity.space = ParticleSystemSimulationSpace.Local;

                    // Randomize smoke direction slightly
                    velocity.x = new ParticleSystem.MinMaxCurve(-0.3f, 0.3f);
                    velocity.y = new ParticleSystem.MinMaxCurve(0.1f, 0.5f);
                    velocity.z = new ParticleSystem.MinMaxCurve(-0.5f, -0.2f);

                    // Add turbulence
                    var noise = smokeParticles.noise;
                    noise.enabled = true;
                    noise.strength = 0.5f;
                    noise.frequency = 0.5f;

                    // Renderer settings for better visibility
                    if (smokeRenderer != null)
                    {
                        smokeRenderer.sortingFudge = 10;
                        smokeRenderer.maxParticleSize = 5f;
                    }
                }
            }
        }

        void Update()
        {
            // Movement forward (toward target)
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            // Update smoke position continuously
            if (activeSmokeEffect != null)
            {
                activeSmokeEffect.transform.position = transform.position + transform.TransformDirection(smokePositionOffset);
            }
        }

        void OnDestroy()
        {
            SpawnExplosionEffect();
            CleanupSmokeEffect();
            RemoveFromSpawner();
        }

        void SpawnExplosionEffect()
        {
            if (explosionEffectPrefab != null && gameObject.scene.isLoaded)
            {
                GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

                // Scale explosion based on asteroid size
                float scale = transform.localScale.magnitude;
                explosion.transform.localScale = Vector3.one * scale;
            }
        }

        void CleanupSmokeEffect()
        {
            if (activeSmokeEffect != null)
            {
                if (smokeParticles != null)
                {
                    // Fade out smoke gradually
                    var emission = smokeParticles.emission;
                    emission.rateOverTime = 0f;

                    // Detach from parent
                    activeSmokeEffect.transform.SetParent(null);

                    // Destroy after remaining particles fade
                    Destroy(activeSmokeEffect, smokeParticles.main.startLifetime.constantMax);
                }
                else
                {
                    Destroy(activeSmokeEffect);
                }
            }
        }

        void RemoveFromSpawner()
        {
            if (spawner != null)
            {
                spawner.activeAsteroids.Remove(this);
                spawner = null;
            }
        }

        public void DestroyByPlayer()
        {
            wasDestroyedByPlayer = true;
            if (progressBar != null)
            {
                progressBar.AddProgress(hitReward);
            }
            Destroy(gameObject);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (wasDestroyedByPlayer) return;

            if (progressBar != null)
            {
                progressBar.AddProgress(-collisionPenalty);
            }
            Destroy(gameObject);
        }
    }
}