using ITSProjectWork;
using UnityEngine;

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

            if (interactable != null && !GameInteractionManager.Instance.IsItemCompleted(interactable))
            {
                interactionText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
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
