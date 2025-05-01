using UnityEngine;
using TMPro;

public class InteractionText : UITextManager
{
    protected override void Awake()
    {
        base.Awake();
        // Any specific initialization for generic interactions
    }

    public void SetInteractionText(string text)
    {
        if (textComponent != null)
        {
            textComponent.text = text;
        }
        else
        {
            Debug.LogWarning("TextComponent is null in InteractionText");
        }
    }
}