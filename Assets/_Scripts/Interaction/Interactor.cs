using ProjectWork;
using UnityEngine;

/// <summary>
/// Handles player interactions with objects in the game world.
/// </summary>
public class Interactor : BlackScreenEnabler
{
    public float interactDistance = 3f;
    public GameObject interactionText;
    [SerializeField] private CameraManager cam;
    [Tooltip("Layer used by interactable objects")]
    [SerializeField] private LayerMask interactableObjsLayer;

    protected override void Awake()
    {
        base.Awake();
        cam = GetComponentInChildren<CameraManager>();
    }

    void Update()
    {
        Ray ray = new(cam.transform.position, cam.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, interactDistance, interactableObjsLayer);
        interactionText.SetActive(false);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent(out InteractableObject interactable)
                && interactable.CanInteract)
            {
                interactionText.SetActive(true);
                if (interactable is OrderFoodButton orderFoodButton)
                {
                    orderFoodButton.Button.Select();
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactionText.SetActive(false);
                    interactable.Interact();
                }
                break;
            }
            else
            {
                interactionText.SetActive(false);
            }
        }
    }
}
