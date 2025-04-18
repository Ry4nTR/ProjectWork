using ProjectWork;
using UnityEngine;
using UnityEngine.EventSystems;

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

    private EventSystem eventSystem;    //Used to deselect the button when the interaction is finished

    protected override void Awake()
    {
        base.Awake();
        cam = GetComponentInChildren<CameraManager>();
        eventSystem = EventSystem.current;
    }

    void Update()
    {
        Ray ray = new(cam.transform.position, cam.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, interactDistance, interactableObjsLayer);
        
        DeselectButton();

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
                    interactable.Interact();
                    DeselectButton();
                }
                break;
            }
            else
            {
                DeselectButton();
            }
        }
    }

    private void DeselectButton()
    {
        interactionText.SetActive(false);
        eventSystem.SetSelectedGameObject(null); // Deselect the button when the interaction is finished
    }
}
