using ProjectWork;
using UnityEngine;

/// <summary>
/// Handles player interactions with objects in the game world.
/// </summary>
public class Interactor : BlackScreenEnabler
{
    public float interactDistance = 3f;
    public GameObject interactionText;
    [SerializeField] private new CameraManager camera;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, interactDistance))
        {
            InteractableObject interactable = hit.transform.GetComponent<InteractableObject>();

            if (interactable != null 
                && interactable.CanInteract)
                //&& !GameInteractionManager.Instance.IsItemCompleted(interactable))
            {
                interactionText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactionText.SetActive(false);
                    interactable.Interact();
                }
                else if(Input.GetMouseButtonDown(0))
                {
                    interactable.Interact();
                }
            }
            else
            {
                interactionText.SetActive(false);
            }
        }
        else
        {
            interactionText.SetActive(false);
        }
    }
}
