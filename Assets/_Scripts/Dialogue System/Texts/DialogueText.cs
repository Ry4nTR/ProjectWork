using TMPro;
using UnityEngine;

public class DialogueText : MonoBehaviour
{
    // Serialized fields for Inspector assignment
    [SerializeField] private SpriteRenderer background; // Background sprite
    [SerializeField] private TextMeshPro message; // Text component

    private Transform cam; // Main camera
    private BoxCollider boxCollider; // Interaction collider

    private void Awake()
    {
        // Ensure collider exists
        boxCollider = GetComponent<BoxCollider>() ?? gameObject.AddComponent<BoxCollider>();

        // Debug warnings if components aren't assigned
        if (background == null)
            Debug.LogWarning("Background not assigned in Inspector!", this);
        if (message == null)
            Debug.LogWarning("Message (TextMeshPro) not assigned in Inspector!", this);
    }

    private void Start()
    {
        cam = Camera.main.transform;
        gameObject.SetActive(false); // Start hidden
    }

    private void LateUpdate()
    {
        // Face camera while maintaining upright position
        if (cam != null)
            transform.LookAt(transform.position + cam.forward);
    }

    /// <summary>
    /// Configures the text bubble with specified message
    /// </summary>
    public void Setup(string text)
    {
        // Set text and update layout
        message.SetText(text);
        message.ForceMeshUpdate();
        Vector2 textSize = message.GetRenderedValues(false);

        // Size background with padding
        Vector2 padding = new Vector2(2f, 2f);
        background.size = textSize + padding;

        // Adjust collider to match background
        if (boxCollider != null)
        {
            boxCollider.size = new Vector3(background.size.x / 1.5f, background.size.y / 2.5f, 0.1f);
            boxCollider.center = new Vector3(-.30f, 0, 0);
        }

        // Position background relative to text
        if (background.transform != null)
            background.transform.localPosition = new Vector3((textSize.x / 2) - 1.1f, 0f);
    }
}