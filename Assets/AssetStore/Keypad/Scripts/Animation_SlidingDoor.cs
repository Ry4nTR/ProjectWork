using UnityEngine;

namespace NavKeypad
{
    public class Animation_SlidingDoor : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        public bool IsOpoen => isOpen;
        private bool isOpen = false;

        public void ToggleDoor()
        {
            isOpen = !isOpen;
            anim.SetBool("isOpen", isOpen);
        }

        public void OpenDoor()
        {
            isOpen = true;
            anim.SetBool("isOpen", isOpen);
        }
        public void CloseDoor()
        {
            isOpen = false;
            anim.SetBool("isOpen", isOpen);
        }
    }
}