using UnityEngine;
using DG.Tweening;

namespace ProjectWork
{
    public class SlidingDoor : MonoBehaviour
    {
        [SerializeField] private Vector3 closedPosition;
        [SerializeField] private Vector3 openedPosition;
        [SerializeField] private float timeToMove;
        [SerializeField] private bool isStartedClosed;

        protected void Start()
        {
            if(isStartedClosed)
            {
                transform.localPosition = closedPosition;
            }
        }

        public void OpenDoor()
        {
            transform.DOLocalMove(openedPosition, timeToMove);
        }

        public void CloseDoor()
        {
            transform.DOLocalMove(closedPosition, timeToMove);
        }
    }
}