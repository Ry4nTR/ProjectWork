using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;

    public float throwForce = 500f;
    public float pickUpRange = 5f;
    private float rotationSensitivity = 0f;
    private GameObject heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true;
    private int LayerNumber;
    private float originalSesitivityValue = 2.5f;
    public CameraManager mouseLookScript;
    [SerializeField] private GameObject interactionText;
    [SerializeField] private GameObject heldText;

    [Header("Hologram Settings")]
    [SerializeField] private GameObject hologram;
    [SerializeField] private float placementDistance = 0.5f;
    private bool isInPlacementRange = false;

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("holdLayer");
        if (hologram != null) hologram.SetActive(false);
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

            if (hologram != null && hologram.activeSelf)
            {
                float dist = Vector3.Distance(heldObj.transform.position, hologram.transform.position);
                isInPlacementRange = dist <= placementDistance;

                if (isInPlacementRange)
                {
                    PlaceObject();
                    return;
                }
            }

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

    private void ShowPickUpPrompt()
    {
        interactionText.SetActive(true);
    }

    private void ShowHeldObjectPrompt()
    {
        heldText.SetActive(true);
    }

    private void HidePickUpPrompt()
    {
        interactionText.SetActive(false);
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

            heldObj.layer = LayerNumber;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);

            if (hologram != null) hologram.SetActive(true);
        }
    }

    void PlaceObject()
    {
        if (heldObj == null || hologram == null) return;

        heldObj.transform.position = hologram.transform.position;
        heldObj.transform.rotation = hologram.transform.rotation;

        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObj.tag = "Untagged";

        hologram.SetActive(false);
        heldObj = null;
        heldObjRb = null;
        heldText.SetActive(false);
    }

    void DropObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObj = null;

        if (hologram != null) hologram.SetActive(false);
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
            mouseLookScript.mouseSensitivity = originalSesitivityValue;
            canDrop = true;
        }
    }

    void ThrowObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;

        if (hologram != null) hologram.SetActive(false);
    }

    void StopClipping()
    {
        heldText.SetActive(false);
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        if (hits.Length > 1)
        {
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f);
        }
    }
}