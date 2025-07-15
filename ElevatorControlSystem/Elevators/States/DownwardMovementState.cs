using ElevatorControlSystem.Enums;

namespace ElevatorControlSystem.Elevators.States
{
    public class DownwardMovementState : ElevatorMovementState
    {
        public override void MoveOneFloor(Elevator elevator)
        {
            if (elevator.IsMoving)
            {
                var nextRequest = elevator.RequestQueue.Where(x => x.DestinationFloor < elevator.CurrentFloor)
                    .OrderByDescending(x => x.DestinationFloor).ToList().FirstOrDefault();
                if (nextRequest != null)
                {
                    var hasPickUp = elevator.RequestQueue.Where(x => x.OriginFloor == elevator.CurrentFloor).Any();
                    if (hasPickUp)
                    {
                        OpenDoor(elevator);
                    }

                    UpdateCurrentState(elevator);

                    if (elevator.CurrentFloor == nextRequest.DestinationFloor)
                    {
                        RequestFulfilled(elevator, nextRequest);
                    }
                }

                if (elevator.RequestQueue.Where(x => elevator.CurrentFloor > x.DestinationFloor).ToList().Count() == 0 &&
                    elevator.RequestQueue.Where(x => elevator.CurrentFloor < x.DestinationFloor).ToList().Count() > 0)
                {
                    ChangeDirection(elevator);
                }
                else if (elevator.RequestQueue.Count == 0 && elevator.PendingOppositeRequestQueue.Count > 0)
                {
                    ChangeDirection(elevator);
                }
                else
                {
                    MoveOneFloor(elevator);
                }
            }
            else
            {
                SetToIdle(elevator);
            }
        }

        public override void PickUpPassenger(Elevator elevator, int destinationFloor)
        {
            if (elevator.CurrentFloor != destinationFloor)
            {
                UpdateCurrentState(elevator);
                PickUpPassenger(elevator, destinationFloor);
            }
        }

        private void UpdateCurrentState(Elevator elevator)
        {
            elevator.State = ElevatorState.Down;
            Thread.Sleep((int)elevator._movementSpeed);
            // For unit tests
            if (elevator._movementSpeed != 0)
                Thread.Sleep(1000);
            elevator.CurrentFloor--;
        }

        /// <summary>
        /// Change elevator direction if there are one or more upward request and no downward request
        /// </summary>
        /// <param name="elevator"></param>
        public override void ChangeDirection(Elevator elevator)
        {
            elevator.State = ElevatorState.Up;
            elevator.RequestQueue.AddRange(elevator.PendingOppositeRequestQueue);
            elevator.PendingOppositeRequestQueue = [];
            _context?.TransitionTo(new UpwardMovementState());
            _context?.MoveOneFloor(elevator);
        }
    }
}
