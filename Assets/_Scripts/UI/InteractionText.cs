using UnityEngine;

namespace ProjectWork
{
    public class InteractionText : UITextManager
    {
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
}