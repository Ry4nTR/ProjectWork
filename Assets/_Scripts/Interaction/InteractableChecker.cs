using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWork
{
    public class InteractableChecker : MonoBehaviour
    {
        private Camera cam;

        [SerializeField] private float raycastRate;
        [SerializeField] private float raycastMaxDistance = 3f;
        [SerializeField] private LayerMask raycastLayerMask;
        private Button currentButtonSelected = null;

        private void Awake()
        {
            cam = Camera.main;
        }

        private void Start()
        {
            StartCoroutine(CheckForInteractables());
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (currentButtonSelected != null)
                {
                    currentButtonSelected.onClick.Invoke();
                    currentButtonSelected = null;
                }
                //LOGIC FOR KEYPAD BUTTONS
                //else
                //{
                //    var ray = cam.ScreenPointToRay(Input.mousePosition);
                //    RaycastHit[] hits = Physics.RaycastAll(ray, raycastMaxDistance, raycastLayerMask);
                //    if (hits.Length > 0)
                //    {
                //        foreach (var hit in hits)
                //        {
                //            if (hit.collider.TryGetComponent(out KeypadButton keypadButton))
                //            {
                //                keypadButton.PressButton();
                //            }
                //        }
                //    }
                //}
            }
        }

        private IEnumerator CheckForInteractables()
        {
            IInteractable currentInteractable = null;
            while (true)
            {
                Ray ray = new(cam.transform.position, cam.transform.forward);

                
                if (Physics.Raycast(ray, out RaycastHit info, raycastMaxDistance, raycastLayerMask))
                {

                    Debug.Log($"Raycasted: {info.collider.name}");
                    //Used for touchscreen buttons
                    if (info.transform.TryGetComponent(out Button button))
                    {
                        //Debug.Log($"{info.collider.name} is a button");
                        currentButtonSelected = button;
                        currentButtonSelected.Select();
                    }
                    //Used for keypad buttons
                    //else if (info.collider.TryGetComponent(out KeypadButton keypadButton))
                    //{
                    //  keypadButton.PressButton();
                    //}
                    //Any other interactable object
                    else if (info.collider.TryGetComponent(out IInteractable interactable))
                    {
                        //Debug.Log($"{info.collider.name} is interactable");
                        //currentInteractable = interactable;
                        //currentInteractable.Interact(/*true*/);
                    }
                    else
                    {
                        //Debug.Log($"{info.collider.name} is NOT interactable");
                        if (currentInteractable != null)
                        {
                            //Debug.Log($"Disabling outlines for {currentInteractable}");
                            //currentInteractable.Interact(/*false*/);
                        }
                        currentInteractable = null;
                        currentButtonSelected = null;
                    }
                    
                }
                yield return new WaitForSeconds(raycastRate);
            }
        }
    }
}