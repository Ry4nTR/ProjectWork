using UnityEngine;

namespace ProjectWork
{
    public class Asteroid : MonoBehaviour
    {
        // Configuration
        public float minSpeed = 1f;
        public float maxSpeed = 3f;
        public float hitReward = 10f;
        public float missPenalty = 5f;

        // Runtime data
        private float speed;
        [SerializeField] private ProgressBar progressBar;
        private bool wasDestroyedByPlayer = false;

        void Start()
        {
            speed = Random.Range(minSpeed, maxSpeed);
        }

        void Update()
        {
            transform.Translate(-Vector3.forward * speed * Time.deltaTime);
        }

        public void DestroyByPlayer()
        {
            wasDestroyedByPlayer = true;
            if (progressBar != null) progressBar.AddProgress(hitReward);
            Destroy(gameObject);
        }

        void OnBecameInvisible()
        {
            if (!wasDestroyedByPlayer && progressBar != null)
            {
                progressBar.AddProgress(-missPenalty);
            }
            Destroy(gameObject, 1f);
        }
    }
}