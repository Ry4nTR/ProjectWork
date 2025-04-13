using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWork
{
    public class FoodSpawner : MonoBehaviour
    {
        [Serializable]
        public class FoodPrefab
        {
            public FoodType foodType;
            public GameObject prefab;
        }

        //Let GameInteractionManager know the food is spawned
        public static event Action<GameObject> OnFoodSpawned = delegate { };

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
                GameObject foodObj =  Instantiate(foodPrefab.prefab, spawnPoint.position, spawnPoint.rotation);
                OnFoodSpawned?.Invoke(foodObj);
            }
        }
    }
}