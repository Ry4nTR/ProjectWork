using System;
using UnityEngine;

namespace ProjectWork
{
    public enum FoodType
    {
        Pizza,
        Burger,
        Salad,
        Sushi
    }
    public class OrderFoodButton : MonoBehaviour
    {
        public event Action<FoodType> OnButtonClicked = delegate { };
        [SerializeField] private FoodType foodType;

        public void OnClick()
        {
            OnButtonClicked?.Invoke(foodType);
            Debug.Log($"Button clicked: {foodType}");
        }
    }
}
