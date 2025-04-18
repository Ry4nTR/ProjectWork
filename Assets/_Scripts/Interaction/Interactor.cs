using ProjectWork;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles player interactions with objects in the game world.
/// </summary>
public class Interactor : BlackScreenEnabler
{
    private InteractionText interactionText;
    private EventSystem eventSystem;    //Used to deselect the button when the interaction is finished

    [SerializeField] private CameraManager cam;
    [SerializeField] private float interactDistance = 3f;
    
    [Tooltip("Layer used by interactable objects")]
    [SerializeField] private LayerMask interactableObjsLayer;

    protected override void Awake()
    {
        base.Awake();
        cam = GetComponentInChildren<CameraManager>();
        interactionText = FindFirstObjectByType<InteractionText>(FindObjectsInactive.Include);
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
                Debug.Log("Interactor: Hit: " + hit.collider.name);
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
