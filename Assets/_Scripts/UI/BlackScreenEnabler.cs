using UnityEngine;

namespace ProjectWork
{
    public class BlackScreenEnabler : ComponentEnabler
    {
        protected virtual void Awake()
        {
            BlackScreenTextController.OnBlackScreenTextStarted += DisableComponent;
            BlackScreenTextController.OnBlackScreenTextFinished += EnableComponent;
        }

        protected virtual void OnDestroy()
        {
            BlackScreenTextController.OnBlackScreenTextStarted -= DisableComponent;
            BlackScreenTextController.OnBlackScreenTextFinished -= EnableComponent;
        }
    }
}