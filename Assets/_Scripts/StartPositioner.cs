using UnityEngine;

namespace ProjectWork
{   
    /// <summary>
    /// This class is responsible for setting the player's starting position in the game world.
    /// </summary>
    public class StartPositioner : MonoBehaviour
    {
        [SerializeField] private Transform startPosition; // The transform representing the player's starting position in the game world.

        private void Start()
        {
            transform.position = startPosition.position; // Set the position of this object to the player's starting position.
        }
    }
}
