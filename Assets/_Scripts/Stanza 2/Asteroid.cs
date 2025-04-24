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
        [HideInInspector] public AsteroidSpawner spawner; // Added spawner reference
        private bool wasDestroyedByPlayer = false;

        void Start()
        {
            progressBar = FindFirstObjectByType<ProgressBar>();
        }

        void Update()
        {
            // Movement forward (toward target)
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        void OnDestroy()
        {
            // More robust cleanup
            if (spawner != null)
            {
                spawner.activeAsteroids.Remove(this);
                // Immediate null check
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