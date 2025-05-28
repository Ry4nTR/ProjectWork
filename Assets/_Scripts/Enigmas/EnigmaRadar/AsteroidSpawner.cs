using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ProjectWork
{
    /// <summary>
    /// Manages spawning and tracking of asteroids in the game.
    /// </summary>
    public class AsteroidSpawner : MonoBehaviour
    {
        // Configuration
        public GameObject asteroidPrefab;        // Prefab for asteroids to spawn
        public Transform[] spawnPoints;         // Array of possible spawn locations
        public Transform targetPoint;           // Target point for asteroids to move toward
        public float spawnInterval = 2f;        // Time between spawn attempts
        [Range(0f, 1f)] public float spawnChance = 0.7f; // Probability of spawning each interval

        // Gizmo settings for editor visualization
        public float gizmoRadius = 0.5f;        // Size of spawn point gizmos
        public Color gizmoColor = Color.cyan;   // Color for spawn point gizmos
        public Color targetGizmoColor = Color.red; // Color for target point gizmo

        // Active asteroid tracking
        public List<Asteroid> activeAsteroids = new List<Asteroid>();

        // State flags
        private bool isSpawning = false;        // Whether spawner is currently active
        private bool isClearingAsteroids = false; // Flag to prevent spawning during cleanup

        void Update()
        {
            // Clean up any null references in the active asteroids list
            activeAsteroids.RemoveAll(asteroid => asteroid == null);
        }

        // Event subscription management
        private void OnEnable()
        {
            WindowTrigger.OnPeekStarted += HandlePeekStarted;
            WindowTrigger.OnPeekEnded += StopSpawning;
        }

        private void OnDisable()
        {
            WindowTrigger.OnPeekStarted -= HandlePeekStarted;
            WindowTrigger.OnPeekEnded -= StopSpawning;
        }

        /// <summary>
        /// Handles window peek start event to begin spawning.
        /// </summary>
        private void HandlePeekStarted(WindowTrigger window)
        {
            // Only start spawning if this is the window with progress bar
            if (window.hasProgressBar && !isSpawning)
            {
                isSpawning = true;
                StartCoroutine(SpawnAsteroids());
            }
        }

        /// <summary>
        /// Starts the asteroid spawning process.
        /// </summary>
        private void StartSpawning()
        {
            if (isSpawning) return;
            isSpawning = true;
            StartCoroutine(SpawnAsteroids());
        }

        /// <summary>
        /// Stops the asteroid spawning process.
        /// </summary>
        private void StopSpawning()
        {
            isSpawning = false;
            StopAllCoroutines();
        }

        /// <summary>
        /// Coroutine that handles periodic asteroid spawning.
        /// </summary>
        private IEnumerator SpawnAsteroids()
        {
            while (isSpawning && !isClearingAsteroids)
            {
                // Random chance to spawn each interval
                if (Random.value <= spawnChance && spawnPoints.Length > 0)
                {
                    // Select random spawn point
                    Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                    // Create asteroid instance
                    GameObject asteroidObj = Instantiate(asteroidPrefab, randomPoint.position, Quaternion.identity);
                    Asteroid asteroid = asteroidObj.GetComponent<Asteroid>();
                    asteroid.spawner = this;
                    activeAsteroids.Add(asteroid);

                    // Orient toward target if specified
                    if (targetPoint != null)
                    {
                        asteroidObj.transform.LookAt(targetPoint);
                    }
                }
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        /// <summary>
        /// Immediately destroys all active asteroids.
        /// </summary>
        public void DestroyAllActiveAsteroids()
        {
            if (isClearingAsteroids) return;

            // Stop spawning and set cleanup flag
            isSpawning = false;
            isClearingAsteroids = true;
            StopAllCoroutines();

            // Destroy all active asteroids
            for (int i = activeAsteroids.Count - 1; i >= 0; i--)
            {
                if (activeAsteroids[i] != null)
                {
                    Destroy(activeAsteroids[i].gameObject);
                }
            }
            activeAsteroids.Clear();

            Debug.Log("All asteroids destroyed immediately");
            isClearingAsteroids = false;
        }

        // Editor visualization
        private void OnDrawGizmos()
        {
            // Draw spawn points
            Gizmos.color = gizmoColor;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, gizmoRadius);
                    // Draw line showing direction to target
                    if (targetPoint != null)
                    {
                        Gizmos.DrawLine(spawnPoint.position, targetPoint.position);
                    }
                }
            }

            // Draw target point with special marker
            if (targetPoint != null)
            {
                Gizmos.color = targetGizmoColor;
                Gizmos.DrawWireSphere(targetPoint.position, gizmoRadius * 1.5f);
                Gizmos.DrawWireCube(targetPoint.position, Vector3.one * gizmoRadius);
            }
        }
    }
}