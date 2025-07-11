﻿using ProjectWork;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interactor : BlackScreenEnabler
{
    private InteractionText interactionText;
    private EventSystem eventSystem;

    [SerializeField] private CameraManager cam;
    [SerializeField] private float normalInteractDistance = 3f;
    [SerializeField] private float planetInteractionDistance = 15f; // Extended range for planets
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

    private void SetWindowInteractionDistance(WindowTrigger windowTrigger, float windowInteractionDistance)
    {
        // Check if this is the final decision window that needs extended range for planets
        if (windowTrigger.isFinalDecisionWindow)
        {
            currentInteractDistance = planetInteractionDistance;
        }
        else
        {
            // Use the window's specified interaction distance for other windows
            currentInteractDistance = windowInteractionDistance;
        }
    }

    private void ResetInteractionDistance()
    {
        currentInteractDistance = normalInteractDistance;
    }

    private void DeselectButton()
    {
        interactionText.SetActive(false);
        eventSystem.SetSelectedGameObject(null);
    }
}