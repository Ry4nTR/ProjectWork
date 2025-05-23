using UnityEngine;

namespace ProjectWork
{
    [RequireComponent(typeof(ParticleSystem))]
    public class  ExtinguisherFoamVFX : MonoBehaviour
    {
        private const float MIN_AMOUNT_TO_REMOVE = .1f;
        [SerializeField, Min(MIN_AMOUNT_TO_REMOVE)] private float amountOfFireToRemove;
        
        private ParticleSystem _foamPS;

        public ParticleSystem FoamPS
        {
            get
            {
                if (!_foamPS) 
                    _foamPS=GetComponent<ParticleSystem>();
                
                return _foamPS;
            } 
        }

        private void OnParticleCollision(GameObject other)
        {
            if (other.TryGetComponent(out Fire fire))
            {
                float value = amountOfFireToRemove / FoamPS.particleCount;
                if (fire.TryExtinguish(value))
                {
                    fire.gameObject.SetActive(false);
                }
            }
        }

        public void Play() => FoamPS.Play();

        public void Stop() => FoamPS.Stop();
    }
}