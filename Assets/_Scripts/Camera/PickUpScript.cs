using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWork
{
    public class PickUpScript : MonoBehaviour
    {
        public static event Action<FoodType> OnFoodPickedUp = delegate { };

        public GameObject player;
        public Transform holdPos;

        public float throwForce = 500f;
        public float pickUpRange = 5f;
        private float rotationSensitivity = 0f;
        private GameObject heldObj;
        private Rigidbody heldObjRb;
        private bool canDrop = true;
        private int LayerNumber;
        private int originalLayerNumber; //Used to store the original layer number of the picked up object
        private float originalSensitivityValue = 2.5f;
        public CameraManager mouseLookScript;
        private PickupText PicUpText;
        //[SerializeField] private GameObject heldText;

        private bool isInPlacementRange = false;

        private void Awake()
        {
            FoodHologram.OnFoodPlaced += DropObject;
            PicUpText = FindFirstObjectByType<PickupText>(FindObjectsInactive.Include);
        }

        void Start()
        {
            LayerNumber = LayerMask.NameToLayer("holdLayer");
        }

        void Update()
        {
            RaycastHit hit;
            bool isLookingAtObject = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange);

            if (heldObj == null)
            {
                if (isLookingAtObject && hit.transform.CompareTag("canPickUp"))
                {
                    ShowPickUpPrompt();

                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                }
                else
                {
                    HidePickUpPrompt();
                }
            }
            else
            {
                HidePickUpPrompt();
                ShowHeldObjectPrompt();

                MoveObject();
                RotateObject();

                if (Input.GetKeyDown(KeyCode.Mouse0) && canDrop && !isInPlacementRange)
                {
                    StopClipping();
                    ThrowObject();
                }
                else if (Input.GetKeyDown(KeyCode.F) && canDrop && !isInPlacementRange)
                {
                    StopClipping();
                    DropObject();
                }
            }
        }

        private void OnDestroy()
        {
            FoodHologram.OnFoodPlaced -= DropObject;
        }

        private void ShowPickUpPrompt()
        {
            PicUpText.SetActive(true);
        }

        private void ShowHeldObjectPrompt()
        {
            //heldText.SetActive(true);
        }

        private void HidePickUpPrompt()
        {
            PicUpText.SetActive(false);
        }

        void PickUpObject(GameObject pickUpObj)
        {
            if (pickUpObj.GetComponent<Rigidbody>())
            {
                heldObj = pickUpObj;
                heldObjRb = pickUpObj.GetComponent<Rigidbody>();
                heldObjRb.isKinematic = true;
                heldObjRb.transform.parent = holdPos.transform;

                // Reset rotation to match hold position
                heldObj.transform.rotation = holdPos.rotation;

                originalLayerNumber = heldObj.layer; // Store the original layer number
                heldObj.layer = LayerNumber;
                Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);

                //if (hologram != null) hologram.gameObject.SetActive(true);
                if(pickUpObj.TryGetComponent(out Food foodObj))
                {
                    OnFoodPickedUp?.Invoke(foodObj.FoodType);
                }
            }
        }

        void DropObject()
        {
            Collider heldObjCollider = heldObj.GetComponent<Collider>();
            Collider playerCollider = player.GetComponent<Collider>();
            Physics.IgnoreCollision(heldObjCollider, playerCollider, false);
            heldObj.layer = originalLayerNumber; // Restore the original layer number
            heldObjRb.isKinematic = false;
            heldObj.transform.parent = null;
            heldObjRb = null;
            heldObj = null;
            canDrop = false;
        }

        void MoveObject()
        {
            heldObj.transform.position = holdPos.transform.position;
        }

        void RotateObject()
        {
            if (Input.GetKey(KeyCode.R))
            {
                canDrop = false;
                mouseLookScript.mouseSensitivity = 0f;

                float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
                float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;
                heldObj.transform.Rotate(Vector3.down, XaxisRotation);
                heldObj.transform.Rotate(Vector3.right, YaxisRotation);
            }
            else
            {
                mouseLookScript.mouseSensitivity = originalSensitivityValue;
                canDrop = true;
            }
        }

        void ThrowObject()
        {
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
            heldObj.layer = originalLayerNumber;
            heldObjRb.isKinematic = false;
            heldObj.transform.parent = null;
            heldObjRb.AddForce(transform.forward * throwForce);
            heldObj = null;
        }

        void StopClipping()
        {
            //heldText.SetActive(false);
            var clipRange = Vector3.Distance(heldObj.transform.position, transform.position);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
            if (hits.Length > 1)
            {
                heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f);
            }
        }
    }
}