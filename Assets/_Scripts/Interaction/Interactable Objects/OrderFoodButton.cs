using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Needed for pointer interfaces

namespace ProjectWork
{
    public enum FoodType
    {
        Pizza,
        Chicken,
        Donut,
    }

    public class OrderFoodButton : InteractableObject
    {
        [SerializeField] private Button button;
        [SerializeField] public FoodType foodType;

        public Button Button => button;

        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();
        }

        public override void Interact()
        {
            button.onClick.Invoke();
        }
    }
}