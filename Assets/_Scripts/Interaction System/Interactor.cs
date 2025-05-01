using ProjectWork;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interactor : BlackScreenEnabler
{
    private InteractionText interactionText;
    private EventSystem eventSystem;

    [SerializeField] private CameraManager cam;
    [SerializeField] private float normalInteractDistance = 3f;
    private float currentInteractDistance;

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

        bool foundInteractable = false;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent(out InteractableObject interactable) && interactable.CanInteract)
            {
                // Set the interaction text from the interactable object
                interactionText.SetInteractionText(interactable.InteractionPrompt);
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

                foundInteractable = true;
                break;
            }
        }

        if (!foundInteractable)
        {
            DeselectButton();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        WindowPeekController.OnPeekStarted -= SetWindowInteractionDistance;
        WindowPeekController.OnPeekEnded -= ResetInteractionDistance;
    }

    private void SetWindowInteractionDistance(Window _, float windowInteractionDistance) => currentInteractDistance = windowInteractionDistance;

    private void ResetInteractionDistance() => currentInteractDistance = normalInteractDistance;

    private void DeselectButton()
    {
        interactionText.SetActive(false);
        eventSystem.SetSelectedGameObject(null);
    }
}