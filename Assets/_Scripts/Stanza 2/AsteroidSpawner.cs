using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 2f;
    [Range(0.1f, 5f)] public float minSpeed = 1f;
    [Range(1f, 10f)] public float maxSpeed = 3f;
    [Range(0f, 1f)] public float spawnChance = 0.7f; // 70% chance to spawn

    private List<GameObject> activeAsteroids = new List<GameObject>();
    private bool isSpawning = false;

    public void StopAndClearAsteroids()
    {
        StopAllCoroutines();
        isSpawning = false;

        foreach (var asteroid in activeAsteroids)
        {
            if (asteroid != null)
            {
                Destroy(asteroid);
            }
        }
        activeAsteroids.Clear();
    }

    private void StartSpawning()
    {
        isSpawning = true;
        StartCoroutine(SpawnAsteroids());
    }

    IEnumerator SpawnAsteroids()
    {
        while (isSpawning)
        {
            // Random chance to spawn (optional)
            if (Random.value <= spawnChance)
            {
                // Select random spawn point
                Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                GameObject asteroid = Instantiate(asteroidPrefab, randomPoint.position, randomPoint.rotation);

                // Set random speed
                Asteroid asteroidScript = asteroid.GetComponent<Asteroid>();
                if (asteroidScript != null)
                {
                    asteroidScript.speed = Random.Range(minSpeed, maxSpeed);
                }

                activeAsteroids.Add(asteroid);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void OnEnable()
    {
        Window.OnPeekStarted += StartSpawning;
        Window.OnPeekEnded += StopAndClearAsteroids;
    }

    private void OnDisable()
    {
        Window.OnPeekStarted -= StartSpawning;
        Window.OnPeekEnded -= StopAndClearAsteroids;
    }

    // Cleanup asteroids when they're destroyed elsewhere
    public void RemoveAsteroidFromList(GameObject asteroid)
    {
        if (activeAsteroids.Contains(asteroid))
        {
            activeAsteroids.Remove(asteroid);
        }
    }
}