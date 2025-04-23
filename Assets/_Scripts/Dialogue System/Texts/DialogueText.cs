using TMPro;
using UnityEngine;

public class DialogueText : MonoBehaviour
{
    // Configuration parameters
    [Header("Text Settings")]
    [SerializeField] private Vector2 textPadding = new Vector2(0f, 0f);
    [SerializeField] private Vector2 backgroundOffset = new Vector2(0f, 0f);

    [Header("Collider Settings")]
    private Vector3 colliderSizeScale = new Vector3(1f / 2f, 1f / 2f, 0.1f);
    [SerializeField] private Vector3 colliderCenterOffset = new Vector3(0f, 0f, 0f);

    // Component references
    private SpriteRenderer background;
    private TextMeshPro message;
    private Transform cam;
    private BoxCollider boxCollider;

    private float initialWidth; // Stores the initial width from PrepareLayout

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>() ?? gameObject.AddComponent<BoxCollider>();
        background = GetComponentInChildren<SpriteRenderer>();
        message = GetComponentInChildren<TextMeshPro>();
    }

    private void Start()
    {
        cam = Camera.main.transform;
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (cam != null)
            transform.LookAt(transform.position + cam.forward);
    }

    public void PrepareLayout(string text)
    {
        message.SetText(text);
        message.ForceMeshUpdate();
        Vector2 textSize = message.GetRenderedValues(false);

        // Store the initial width
        initialWidth = textSize.x;

        // Set background size with padding
        background.size = textSize + textPadding;

        // Position background relative to text
        background.transform.localPosition = new Vector3(
            (textSize.x / 2) + backgroundOffset.x,
            backgroundOffset.y
        );

        // Set up collider
        UpdateCollider();

        // Clear text while keeping layout
        message.SetText("");
        message.ForceMeshUpdate();
    }

    public void UpdateText(string text)
    {
        message.SetText(text);
        message.ForceMeshUpdate();

        Vector2 currentTextSize = message.GetRenderedValues(false);

        // Update background size - maintain initial width but use current height
        Vector2 newBackgroundSize = new Vector2(
            initialWidth + textPadding.x,
            currentTextSize.y + textPadding.y
        );

        background.size = newBackgroundSize;

        // Update collider
        UpdateCollider();
    }

    private void UpdateCollider()
    {
        if (boxCollider != null)
        {
            boxCollider.size = new Vector3(
                background.size.x * colliderSizeScale.x,
                background.size.y * colliderSizeScale.y,
                colliderSizeScale.z
            );
            boxCollider.center = colliderCenterOffset;
        }
    }
}