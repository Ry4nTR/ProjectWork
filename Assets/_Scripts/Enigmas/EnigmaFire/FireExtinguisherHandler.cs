using UnityEngine;

public class FireExtinguisherHandler : MonoBehaviour
{
    [SerializeField] private bool isButtonPressed;
    [SerializeField] private ParticleSystem fireExtinguisherVFX;

    private GameObject fireExtinguisherObj;

    private void Awake()
    {
        fireExtinguisherObj = fireExtinguisherVFX.transform.parent.gameObject;
    }

    private void OnEnable()
    {
        fireExtinguisherObj.SetActive(true);
        SetFireExtinguisher(true);
    }

    private void Update()
    {
        SetFireExtinguisher(Input.GetMouseButton(0));
    }

    private void OnDisable()
    {
        fireExtinguisherObj.SetActive(false);
        SetFireExtinguisher(false);
    }

    private void SetFireExtinguisher(bool isActive)
    {
        if(isActive)
        {
            fireExtinguisherVFX.Play();
        }
        else
        {
            fireExtinguisherVFX.Stop();
        } 
            
    }
}
