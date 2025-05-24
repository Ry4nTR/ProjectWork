using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWork
{
    /// <summary>
    /// Base class for all interaction UI prompts
    /// </summary>
    public abstract class UITextManager : MonoBehaviour
    {
        [SerializeField] protected Image imageComponent;
        [SerializeField] protected TextMeshProUGUI textComponent;   

        protected void Start()
        {
            SetActive(false);
        }

        public void SetActive(bool state)
        {
            if(textComponent == null)
            {
                textComponent = GetComponentInChildren<TextMeshProUGUI>(true);
            }
            textComponent.enabled = state;

            if (imageComponent == null)
            {
                imageComponent = GetComponentInChildren<Image>(true);
            }
            imageComponent.enabled = state;
        }
    }
}