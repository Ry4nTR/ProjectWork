using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Base class for all interaction UI prompts
/// </summary>
public abstract class UITextBase : MonoBehaviour
{
    protected TextMeshProUGUI textComponent;
    protected Image imageComponent;

    protected virtual void Awake()
    {
        // Find components on the same GameObject
        textComponent = GetComponent<TextMeshProUGUI>();
        imageComponent = GetComponentInChildren<Image>();

        SetActive(false);
    }

    public virtual void SetActive(bool state)
    {
        textComponent.enabled = state;
        imageComponent.enabled = state;
    }
}