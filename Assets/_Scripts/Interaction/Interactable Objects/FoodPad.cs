using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ProjectWork
{
    public class FoodPad : InteractableObject
    {
        public enum PadState
        {
            Selection,
            AfterSelection,
            DisplayNumber
        }

        [Serializable]
        public struct PadScreen
        {
            public PadState state;
            public GameObject screenObject;
        }
        public static event Action<FoodType> OnSelectedFood = delegate { };

        [SerializeField] private PadState _currentPadState = PadState.Selection;
        [SerializeField] private List<PadScreen> screens = new List<PadScreen>();

        [Header("Emergency Number")]
        [SerializeField, Range(0, 9)] private byte puzzleNumber;
        private TextMeshProUGUI numberText;

        [SerializeField] private List<OrderFoodButton> orderFoodButtons = new List<OrderFoodButton>();

        public PadState CurrentPadState => _currentPadState;

        private void Awake()
        {
            orderFoodButtons = new List<OrderFoodButton>(GetComponentsInChildren<OrderFoodButton>(true));

            foreach (var foodButton in orderFoodButtons)
            {
                FoodType currentFoodType = foodButton.foodType;
                foodButton.Button.onClick.AddListener(() => OrderFood(currentFoodType));
            }
        }

        private void OnDestroy()
        {
            foreach (var foodButton in orderFoodButtons)
            {
                foodButton.Button.onClick.RemoveAllListeners();
            }
        }

        private void OrderPizza()
        {
            OrderFood(FoodType.Pizza);
        }

        private void OrderChicken()
        {
            OrderFood(FoodType.Chicken);
        }

        private void OrderDonut()
        {
            OrderFood(FoodType.Donut);
        }

        private void OrderFood(FoodType foodType)
        {
            Debug.Log($"Ordering {foodType}");
            OnSelectedFood?.Invoke(foodType);
            SetState(PadState.AfterSelection);
            //Block other orders
        }

        private void SetState(PadState newState)
        {
            _currentPadState = newState;
            UpdateScreen();
        }

        private void UpdateScreen()
        {
            foreach (PadScreen screen in screens)
            {
                screen.screenObject.SetActive(screen.state == _currentPadState);
            }
        }
    }
}