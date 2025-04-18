using UnityEngine;

namespace ProjectWork
{
    public class Food : InteractableObject
    {
        [Header("Food Settings")]
        [SerializeField] private FoodType foodType;

        public FoodType FoodType => foodType;

        public override void Interact()
        {
            base.Interact();  // Mantiene la logica base (eventi, black screen, ecc.)
            
            TrashManager.Instance.SpawnTrash();  // Spawna nel punto del TrashManager
            BlackScreenTextController.OnBlackScreenFullActivated += DestroyObject; // Si iscrive all'evento per distruggere l'oggetto
        }

        private void DestroyObject()
        {
            BlackScreenTextController.OnBlackScreenFullActivated -= DestroyObject; // Si disiscrive dall'evento
            Destroy(gameObject);
        }
    }
}