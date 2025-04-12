using UnityEngine;

public class TrashManager : MonoBehaviour
{
    public static TrashManager Instance { get; private set; }

    [SerializeField] private GameObject trashPrefab;  // Assegna il prefab dall'Inspector

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void SpawnTrash()
    {
        if (trashPrefab != null)
        {
            Instantiate(trashPrefab, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogError("Trash prefab not assigned in TrashManager!", this);
        }
    }
}