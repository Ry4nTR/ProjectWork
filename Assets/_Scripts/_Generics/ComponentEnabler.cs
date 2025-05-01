using UnityEngine;

namespace ProjectWork
{
    public abstract class ComponentEnabler : MonoBehaviour
    {
        protected void EnableComponent()
        {
            enabled = true;
        }
        protected void DisableComponent()
        {
            enabled = false;
        }
    }   
}