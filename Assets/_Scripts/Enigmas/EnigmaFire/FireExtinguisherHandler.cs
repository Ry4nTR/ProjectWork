using UnityEngine;

namespace ProjectWork
{
    public class FireExtinguisherHandler : MonoBehaviour
    {
        [Header("Fire Extinguisher Stats")]
        [Tooltip("Charge consumption rate per second (Max Charge = 1f)")]
        [SerializeField, Min(0)] private float chargeConsumptionRate;

        private const float INITIAL_CHARGE = 1f;
        private float _currentCharge = INITIAL_CHARGE;

        [Header("References")]       
        [SerializeField] private MeshRenderer glassRenderer;
        private ExtinguisherFoamVFX fireExtinguisherVFX;
        private GameObject fireExtinguisherObj;
        private Material glassMaterial;

        public float CurrentCharge
        {
            get => _currentCharge;
            private set
            {
                _currentCharge = Mathf.Clamp01(value);
                glassMaterial.SetColor("_BaseColor", Color.Lerp(Color.red, Color.green, _currentCharge));
                if (_currentCharge == 0f)
                {
                    SetFireExtinguisherObject(false);
                    SetFireExtinguisherVFX(false);
                }      
            }
        }

        private void Awake()
        {
            fireExtinguisherVFX = GetComponentInChildren<ExtinguisherFoamVFX>(true);
            fireExtinguisherObj = fireExtinguisherVFX.transform.parent.gameObject;
            glassMaterial = glassRenderer.material;

            FireExtinguisher.OnFireExtinguisherPickedUp += EnableFireExtinguisherMode;
            FirePuzzle.OnPuzzleCompleted += DisableFireExtinguisherMode;
        }

        private void Start()
        {
            SetFireExtinguisherObject(false);
        }

        private void Update()
        {
            if(fireExtinguisherObj.activeSelf && CurrentCharge > 0f && Input.GetMouseButton(0))
            {
                SetFireExtinguisherVFX(true);
                CurrentCharge -= chargeConsumptionRate * Time.deltaTime;
            }
            else
            {
                SetFireExtinguisherVFX(false);
            }
        }

        private void OnDisable()
        {
            SetFireExtinguisherObject(false);
            SetFireExtinguisherVFX(false);
        }

        private void OnDestroy()
        {
            FireExtinguisher.OnFireExtinguisherPickedUp -= EnableFireExtinguisherMode;
            FirePuzzle.OnPuzzleCompleted -= DisableFireExtinguisherMode;
        }

        private void EnableFireExtinguisherMode()
        {
            SetFireExtinguisherObject(true);
        }

        private void DisableFireExtinguisherMode()
        {
            SetFireExtinguisherObject(false);
        }

        private void SetFireExtinguisherObject(bool isActive)
        {
            fireExtinguisherObj.SetActive(isActive);
            CurrentCharge = INITIAL_CHARGE;
        }

        private void SetFireExtinguisherVFX(bool isActive)
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
}