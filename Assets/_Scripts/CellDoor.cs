namespace ProjectWork
{
    public class CellDoor : SlidingDoor
    {
        private void Awake()
        {
            GameInteractionManager.OnDayPassed += HandleDoorOpening;
        }

        private void OnDestroy()
        {
            GameInteractionManager.OnDayPassed -= HandleDoorOpening;
        }

        private void HandleDoorOpening(bool areDaysPassed)
        {
            if (areDaysPassed)
            {
                OpenDoor();
            }
        }
    }
}