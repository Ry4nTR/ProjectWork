using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProjectWork.UI;

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
        public struct OrderFoodButton
        {
            public Button button;
            public FoodType foodType;
        }
        public static event Action<FoodType> OnSelectedFood = delegate { };

        [Header("Food Pad Settings")]
        [SerializeField] private PadState _currentPadState = PadState.Selection;
        
        [Header("References")]
        [SerializeField] private List<PadScreen> screens = new();
        [SerializeField] private List<OrderFoodButton> orderFoodButtons;

        [Header("Emergency Number")]
        [SerializeField, Range(0, 9)] private byte puzzleNumber;

        public PadState CurrentPadState => _currentPadState;

        private void Awake()
        {
            foreach (OrderFoodButton foodButton in orderFoodButtons)
            {
                switch(foodButton.foodType)
                {
                    case FoodType.Pizza:
                        foodButton.button.onClick.AddListener(OrderPizza);
                        break;
                    case FoodType.Chicken:
                        foodButton.button.onClick.AddListener(OrderChicken);
                        break;
                    case FoodType.Donut:
                        foodButton.button.onClick.AddListener(OrderDonut);
                        break;
                }
            }

            GameInteractionManager.OnTutorialFinished += ChangeToNumberScreen;
        }

        private void OnDestroy()
        {
            foreach (OrderFoodButton foodButton in orderFoodButtons)
            {
                switch (foodButton.foodType)
                {
                    case FoodType.Pizza:
                        foodButton.button.onClick.RemoveListener(OrderPizza);
                        break;
                    case FoodType.Chicken:
                        foodButton.button.onClick.RemoveListener(OrderChicken);
                        break;
                    case FoodType.Donut:
                        foodButton.button.onClick.RemoveListener(OrderDonut);
                        break;
                }
            }

            GameInteractionManager.OnTutorialFinished -= ChangeToNumberScreen;
        }

        private void OrderPizza() => OrderFood(FoodType.Pizza);
        private void OrderChicken() => OrderFood(FoodType.Chicken);
        private void OrderDonut() => OrderFood(FoodType.Donut);

        private void OrderFood(FoodType foodType)
        {
            Debug.Log($"Ordering {foodType}");
            OnSelectedFood?.Invoke(foodType);
            SetState(PadState.AfterSelection);
            //Block other orders
        }

        private void ChangeToNumberScreen() => SetState(PadState.DisplayNumber);  

        private void SetState(PadState newState)
        {
            _currentPadState = newState;
            UpdateScreen();
        }

        private void UpdateScreen()
        {
            foreach (PadScreen screen in screens)
            {
                screen.SetCanvasGroup(screen.ScreenType == _currentPadState);
                screen.gameObject.SetActive(screen.ScreenType == _currentPadState);
            }
        }
    }
}