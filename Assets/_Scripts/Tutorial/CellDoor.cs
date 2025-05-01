namespace ProjectWork
{
    public class CellDoor : SlidingDoor
    {
        private void Awake()
        {
            TutorialTaskChecker.OnDayPassed += HandleDoorOpening;
        }

        private void OnDestroy()
        {
            TutorialTaskChecker.OnDayPassed -= HandleDoorOpening;
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