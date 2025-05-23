using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ProjectWork
{
    public class AsteroidSpawner : MonoBehaviour
    {
        // Configuration
        public GameObject asteroidPrefab;
        public Transform[] spawnPoints;
        public Transform targetPoint; // Nuovo punto di destinazione
        public float spawnInterval = 2f;
        [Range(0f, 1f)] public float spawnChance = 0.7f;
        
        // Gizmo settings
        public float gizmoRadius = 0.5f;
        public Color gizmoColor = Color.cyan;
        public Color targetGizmoColor = Color.red; // Colore per il punto di destinazione

        public List<Asteroid> activeAsteroids = new List<Asteroid>();

        private bool isSpawning = false;
        private bool isClearingAsteroids = false; // New flag to prevent spawning during cleanup



        void Update()
        {
            // Clean up any null references in the list
            activeAsteroids.RemoveAll(asteroid => asteroid == null);
        }


        // AsteroidSpawner.cs modifications (only the event handler parts)
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

        private void HandlePeekStarted(WindowTrigger window)
        {
            // Only start spawning if this is the window with progress bar
            if (window.hasProgressBar && !isSpawning)
            {
                isSpawning = true;
                StartCoroutine(SpawnAsteroids());
            }
        }

        private void StartSpawning()
        {
            if (isSpawning) return;
            isSpawning = true;
            StartCoroutine(SpawnAsteroids());
        }

        private void StopSpawning()
        {
            isSpawning = false;
            StopAllCoroutines();
        }

        private IEnumerator SpawnAsteroids()
        {
            while (isSpawning && !isClearingAsteroids) // Add check for clearing state
            {
                if (Random.value <= spawnChance && spawnPoints.Length > 0)
                {
                    Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                    GameObject asteroidObj = Instantiate(asteroidPrefab, randomPoint.position, Quaternion.identity);
                    Asteroid asteroid = asteroidObj.GetComponent<Asteroid>();
                    asteroid.spawner = this;
                    activeAsteroids.Add(asteroid);

                    if (targetPoint != null)
                    {
                        asteroidObj.transform.LookAt(targetPoint);
                    }
                }
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        public void DestroyAllActiveAsteroids()
        {
            if (isClearingAsteroids) return;

            // Stop immediately any spawning activity
            isSpawning = false;
            isClearingAsteroids = true;
            StopAllCoroutines();

            // Destroy all asteroids immediately
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

        // Draw gizmos in the editor
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

            // Draw target point
            if (targetPoint != null)
            {
                Gizmos.color = targetGizmoColor;
                Gizmos.DrawWireSphere(targetPoint.position, gizmoRadius * 1.5f);
                Gizmos.DrawWireCube(targetPoint.position, Vector3.one * gizmoRadius);
            }
        }
    }
}