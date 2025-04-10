using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace ITSProjectWork
{
    public class FoodPad : MonoBehaviour, IInteractable
    {
        public static event System.Action<ShapeType> OnSelectedShape = delegate { };
        [SerializeField] private List<OrderFoodButton> orderFoodButtons;

        private void Awake()
        {
            orderFoodButtons = new List<OrderFoodButton>(GetComponentsInChildren<OrderFoodButton>());
            foreach (OrderFoodButton button in orderFoodButtons)
            {
                button.OnButtonClicked += OrderFood;
            }
        }

        private void OnDestroy()
        {
            foreach (OrderFoodButton button in orderFoodButtons)
            {
                button.OnButtonClicked -= OrderFood;
            }
        }

        private void OrderFood(FoodType foodType)
        {
            Debug.Log($"Ordering {foodType}");
        }

        public void Interact()
        {
            Debug.Log("Interacting with FoodPad");
        }
    }
}