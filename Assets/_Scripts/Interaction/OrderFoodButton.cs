using System;
using UnityEngine;

namespace ProjectWork
{
    public enum FoodType
    {
        Pizza,
        Pollo,
        Donut,
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
