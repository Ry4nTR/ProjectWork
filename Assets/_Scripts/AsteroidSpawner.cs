using System.Collections;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public Transform[] spawnPoints;
    public ProgressBar progressBar;

    void Start()
    {
        StartCoroutine(SpawnAsteroids());
    }

    IEnumerator SpawnAsteroids()
    {
        foreach (var point in spawnPoints)
        {
            GameObject asteroid = Instantiate(asteroidPrefab, point.position, point.rotation);
            asteroid.GetComponent<Asteroid>().Init(progressBar); // Pass progress bar reference
            yield return new WaitForSeconds(2f);
        }
    }
}

// Add to Asteroid prefab:
public class Asteroid : MonoBehaviour
{
    private ProgressBar progressBar;
    private bool wasHit = false;

    public void Init(ProgressBar pb)
    {
        progressBar = pb;
    }

    void OnBecameInvisible()
    {
        if (!wasHit && progressBar != null)
        {
            progressBar.AsteroidEscaped();
        }
        Destroy(gameObject, 1f); // Cleanup
    }

    void OnDestroy()
    {
        if (wasHit) progressBar.AddProgress(10f); // Reward for hit
    }
}