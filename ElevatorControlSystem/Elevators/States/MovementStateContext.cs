namespace ElevatorControlSystem.Elevators.States
{
    public class MovementStateContext
    {
        private ElevatorMovementState? _state;

        public MovementStateContext(ElevatorMovementState state)
        {
            this.TransitionTo(state);
        }

        public void TransitionTo(ElevatorMovementState state)
        {
            _state = state;
            _state.SetContext(this);
        }

        public void MoveOneFloor(Elevator elevator)
        {
            _state?.MoveOneFloor(elevator);
        }

        public void PickUpPassenger(Elevator elevator, int destinationFloor)
        {
            _state?.PickUpPassenger(elevator, destinationFloor);
        }
    }
}
