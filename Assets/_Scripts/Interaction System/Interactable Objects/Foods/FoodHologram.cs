using System;
using UnityEngine;

namespace ProjectWork
{
    public class FoodHologram : MonoBehaviour
    {
        public static event Action OnFoodPlaced = delegate { };

        [SerializeField] private FoodType _foodType;
        public FoodType HologramFoodType => _foodType;

        private void Awake()
        {
            PickUpScript.OnFoodPickedUp += SetActivation;
        }

        private void Start()
        {
            SetActive(false);
        }

        private void OnDestroy()
        {
            PickUpScript.OnFoodPickedUp -= SetActivation;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out Food food))
            {
                if (food.FoodType == _foodType)
                {              
                    PlaceFoodHere(food);
                    food.UnlockInteraction();
                    SetActive(false);
                    OnFoodPlaced?.Invoke();
                }
            }
        }

        private void PlaceFoodHere(Food food)
        {
            food.gameObject.layer = LayerMask.NameToLayer("Interactable");
            food.transform.parent = transform;
            food.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            //food.transform.SetPositionAndRotation(transform.position, transform.rotation);
            food.tag = "Untagged";
        }

        private void SetActivation(FoodType pickedUpFoodType)
        {
            SetActive(pickedUpFoodType == _foodType);
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}