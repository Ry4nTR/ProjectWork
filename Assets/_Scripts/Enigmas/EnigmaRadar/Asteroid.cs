using UnityEngine;

namespace ProjectWork
{
    /// <summary>
    /// Controls asteroid behavior including movement, visual effects, and scoring interactions.
    /// </summary>
    public class Asteroid : MonoBehaviour
    {
        // Configuration
        [Header("Scoring Settings")]
        [Tooltip("Points added when player destroys this asteroid")]
        [SerializeField] private float hitReward = 25f;

        [Tooltip("Points deducted when asteroid reaches target")]
        [SerializeField] private float collisionPenalty = 10f;

        [Header("Difficulty Settings")]
        [Tooltip("Base movement speed of the asteroid")]
        [Range(5f, 30f)] public float speed = 3f;

        // References
        private ProgressBar progressBar;        // Reference to the progress bar for scoring
        [HideInInspector] public AsteroidSpawner spawner; // Reference to spawner that created this asteroid
        private bool wasDestroyedByPlayer = false; // Flag to track destruction source

        // Enhanced Smoke Effects Configuration
        [Header("Enhanced Smoke Settings")]
        public GameObject smokeEffectPrefab;     // Prefab for the smoke trail effect
        public GameObject explosionEffectPrefab; // Prefab for explosion effect
        public Vector3 smokePositionOffset = new Vector3(0, -0.5f, -0.5f); // Offset for smoke position
        public float smokeSizeMultiplier = 2f;  // Multiplier for smoke particle size
        public float smokeDensity = 1.5f;       // Density of smoke particles
        public float smokeTrailLength = 1f;    // Length of the smoke trail
        public bool useWorldSpace = true;       // Whether smoke uses world or local space

        // Effect instances
        private GameObject activeSmokeEffect;   // Currently active smoke effect
        private ParticleSystem smokeParticles;  // Particle system component of smoke
        private ParticleSystemRenderer smokeRenderer; // Renderer for smoke particles

        void Start()
        {
            // Find and store reference to progress bar
            progressBar = FindFirstObjectByType<ProgressBar>();
            // Initialize smoke trail effect
            InitializeEnhancedSmokeEffect();
        }

        /// <summary>
        /// Initializes and configures the smoke trail effect for the asteroid.
        /// </summary>
        void InitializeEnhancedSmokeEffect()
        {
            if (smokeEffectPrefab != null)
            {
                // Calculate spawn position with offset
                Vector3 spawnPosition = transform.position + transform.TransformDirection(smokePositionOffset);

                // Instantiate effect and parent to asteroid
                activeSmokeEffect = Instantiate(smokeEffectPrefab, spawnPosition, transform.rotation);
                activeSmokeEffect.transform.SetParent(transform);

                // Get particle system components
                smokeParticles = activeSmokeEffect.GetComponent<ParticleSystem>();
                smokeRenderer = activeSmokeEffect.GetComponent<ParticleSystemRenderer>();

                if (smokeParticles != null)
                {
                    // Configure main particle system settings
                    var main = smokeParticles.main;
                    main.simulationSpace = useWorldSpace ? ParticleSystemSimulationSpace.World : ParticleSystemSimulationSpace.Local;
                    main.startSizeMultiplier = smokeSizeMultiplier;
                    main.startLifetime = smokeTrailLength;

                    // Configure emission rate
                    var emission = smokeParticles.emission;
                    emission.rateOverTime = 20f * smokeDensity;

                    // Configure particle shape
                    var shape = smokeParticles.shape;
                    shape.angle = 25f; // Wider emission angle

                    // Configure velocity over lifetime
                    var velocity = smokeParticles.velocityOverLifetime;
                    velocity.enabled = true;
                    velocity.space = ParticleSystemSimulationSpace.Local;

                    // Randomize smoke direction slightly
                    velocity.x = new ParticleSystem.MinMaxCurve(-0.3f, 0.3f);
                    velocity.y = new ParticleSystem.MinMaxCurve(0.1f, 0.5f);
                    velocity.z = new ParticleSystem.MinMaxCurve(-0.5f, -0.2f);

                    // Add noise/turbulence to particles
                    var noise = smokeParticles.noise;
                    noise.enabled = true;
                    noise.strength = 0.5f;
                    noise.frequency = 0.5f;

                    // Configure renderer settings for better visibility
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
            // Move asteroid forward (toward target)
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            // Update smoke position continuously to follow asteroid
            if (activeSmokeEffect != null)
            {
                activeSmokeEffect.transform.position = transform.position + transform.TransformDirection(smokePositionOffset);
            }
        }

        void OnDestroy()
        {
            // Handle effects and cleanup when asteroid is destroyed
            SpawnExplosionEffect();
            CleanupSmokeEffect();
            RemoveFromSpawner();
        }

        /// <summary>
        /// Spawns explosion effect when asteroid is destroyed.
        /// </summary>
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

        /// <summary>
        /// Cleans up smoke trail effect when asteroid is destroyed.
        /// </summary>
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

        /// <summary>
        /// Removes this asteroid from spawner's active list.
        /// </summary>
        void RemoveFromSpawner()
        {
            if (spawner != null)
            {
                spawner.activeAsteroids.Remove(this);
                spawner = null;
            }
        }

        /// <summary>
        /// Called when player destroys this asteroid.
        /// </summary>
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
            // Skip if already destroyed by player
            if (wasDestroyedByPlayer) return;

            // Apply penalty if asteroid reaches target
            if (progressBar != null)
            {
                progressBar.AddProgress(-collisionPenalty);
            }
            Destroy(gameObject);
        }
    }
}