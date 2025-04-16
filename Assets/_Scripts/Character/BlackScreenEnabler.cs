using ProjectWork;
using UnityEngine;

public class BlackScreenEnabler : MonoBehaviour
{
    protected virtual void Awake()
    {
        BlackScreenTextController.OnBlackScreenTextStarted += DisableComponent;
        BlackScreenTextController.OnBlackScreenTextFinished += EnableComponent;
    }

    protected void EnableComponent()
    {
        enabled = true;
    }

    protected void DisableComponent()
    {
        enabled = false;
    }

    protected virtual void OnDestroy()
    {
        BlackScreenTextController.OnBlackScreenTextStarted -= DisableComponent;
        BlackScreenTextController.OnBlackScreenTextFinished -= EnableComponent;
    }
}
