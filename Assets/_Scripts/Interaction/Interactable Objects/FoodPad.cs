using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField, Range(0,9)] private byte puzzleNumber;
        private TextMeshProUGUI numberText;

        public struct OrderFoodButton
        {
            public Button button;
            public FoodType foodType;
        }

        private List<OrderFoodButton> orderFoodButtons;

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

            //GameInteractionManager.OnPuzzleCompleted += OnPuzzleCompleted;
        }

        private void OnDestroy()
        {
            foreach (OrderFoodButton button in orderFoodButtons)
            {
                //button.OnButtonClicked -= OrderFood;
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