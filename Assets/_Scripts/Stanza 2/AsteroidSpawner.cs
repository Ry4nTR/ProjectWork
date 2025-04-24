using UnityEngine;
using System.Collections;

namespace ProjectWork
{
    public class AsteroidSpawner : MonoBehaviour
    {
        // Configuration
        public GameObject asteroidPrefab;
        public Transform[] spawnPoints;
        public float spawnInterval = 2f;
        [Range(0f, 1f)] public float spawnChance = 0.7f;

        private bool isSpawning = false;

        private void OnEnable()
        {
            Window.OnPeekStarted += StartSpawning;
            Window.OnPeekEnded += StopSpawning;
        }

        private void OnDisable()
        {
            Window.OnPeekStarted -= StartSpawning;
            Window.OnPeekEnded -= StopSpawning;
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
            while (isSpawning)
            {
                if (Random.value <= spawnChance)
                {
                    Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                    Instantiate(asteroidPrefab, randomPoint.position, randomPoint.rotation);
                }
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
}