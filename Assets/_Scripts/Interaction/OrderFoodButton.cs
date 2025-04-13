using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        private Button button;
        [SerializeField] private FoodType foodType;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void Update()
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            button.OnPointerEnter(pointerEventData);
        }

        public override void Interact()
        {
            button.onClick.Invoke();
            
        }
    }
}
