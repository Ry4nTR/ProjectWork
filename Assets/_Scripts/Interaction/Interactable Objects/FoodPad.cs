using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ProjectWork
{
    public class FoodPad : InteractableObject
    {
        public static event Action<FoodType> OnSelectedFood = delegate { };
        [SerializeField] private List<OrderFoodButton> orderFoodButtons;
        [Header("Emergency Number")]
        [SerializeField, Range(0,9)] private byte puzzleNumber;
        [SerializeField] private TextMeshProUGUI numberText;

        private void Awake()
        {
            orderFoodButtons = new List<OrderFoodButton>(GetComponentsInChildren<OrderFoodButton>());
            foreach (OrderFoodButton button in orderFoodButtons)
            {
                button.OnButtonClicked += OrderFood;
            }

            //GameInteractionManager.OnPuzzleCompleted += OnPuzzleCompleted;
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
            OnSelectedFood?.Invoke(foodType);
            //Block other orders
        }
    }
}