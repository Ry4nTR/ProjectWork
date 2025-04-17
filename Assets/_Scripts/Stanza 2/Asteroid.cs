using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float speed = 3f;
    [HideInInspector] public ProgressBar progressBar;
    [HideInInspector] public AsteroidSpawner spawner;
    private bool wasDestroyedByPlayer = false;

    void Update()
    {
        transform.Translate(-Vector3.forward * speed * Time.deltaTime);
    }

    public void DestroyByPlayer()
    {
        wasDestroyedByPlayer = true;
        if (progressBar != null)
        {
            progressBar.AddProgress(progressBar.hitReward);
        }
        if (spawner != null)
        {
            spawner.RemoveAsteroidFromList(gameObject);
        }
        Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        if (!wasDestroyedByPlayer && progressBar != null)
        {
            progressBar.AddProgress(-progressBar.missPenalty);
        }
        if (spawner != null)
        {
            spawner.RemoveAsteroidFromList(gameObject);
        }
        Destroy(gameObject, 1f);
    }
}