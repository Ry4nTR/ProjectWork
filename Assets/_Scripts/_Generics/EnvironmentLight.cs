using UnityEngine;

namespace ProjectWork
{
    [RequireComponent(typeof(Light))]
    public class EnvironmentLight : MonoBehaviour
    {// This class can be used to control the environment light on a ship.
        private Light myLight;

        protected void Awake()
        {
            myLight = GetComponent<Light>();
        }
    }
}