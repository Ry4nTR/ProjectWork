using System.Collections.Generic;
using ProjectWork;
using UnityEngine;

namespace ITSProjectWork
{
    public class FoodSpawner : MonoBehaviour
    {
        [System.Serializable]
        public class FoodPrefab
        {
            public FoodType foodType;
            public GameObject prefab;
        }

        [SerializeField] private List<FoodPrefab> foodPrefabs = new List<FoodPrefab>();
        [SerializeField] private Transform spawnPoint;

        private void OnEnable()
        {
            FoodPad.OnSelectedFood += SpawnFood;
        }

        private void OnDisable()
        {
            FoodPad.OnSelectedFood -= SpawnFood;
        }

        private void SpawnFood(FoodType foodType)
        {
            FoodPrefab foodPrefab = foodPrefabs.Find(x => x.foodType == foodType);
            if (foodPrefab != null && foodPrefab.prefab != null)
            {
                Instantiate(foodPrefab.prefab, spawnPoint.position, spawnPoint.rotation);
            }
        }
    }
}