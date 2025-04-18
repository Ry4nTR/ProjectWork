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
    [SerializeField] private float normalInteractDistance = 3f;
    private float currentInteractDistance;

    [Tooltip("Layer used by interactable objects")]
    [SerializeField] private LayerMask interactableObjsLayer;

    protected override void Awake()
    {
        base.Awake();
        cam = GetComponentInChildren<CameraManager>();
        interactionText = FindFirstObjectByType<InteractionText>(FindObjectsInactive.Include);
        eventSystem = EventSystem.current;

        WindowPeekController.OnPeekStarted += SetWindowInteractionDistance;
        WindowPeekController.OnPeekEnded += ResetInteractionDistance;
    }

    private void Start() => currentInteractDistance = normalInteractDistance;

    private void Update()
    {
        Ray ray = new(cam.transform.position, cam.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, currentInteractDistance, interactableObjsLayer);
        
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        WindowPeekController.OnPeekStarted -= SetWindowInteractionDistance;
        WindowPeekController.OnPeekEnded -= ResetInteractionDistance;
    }

    private void SetWindowInteractionDistance(float windowInteractionDistance) => currentInteractDistance = windowInteractionDistance;

    private void ResetInteractionDistance() => currentInteractDistance = normalInteractDistance;

    private void DeselectButton()
    {
        interactionText.SetActive(false);
        eventSystem.SetSelectedGameObject(null); // Deselect the button when the interaction is finished
    }
}
