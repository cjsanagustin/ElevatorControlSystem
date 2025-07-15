using ElevatorControlSystem.Elevators.States;
using ElevatorControlSystem.Enums;
using ElevatorControlSystem.Halls;
using ElevatorControlSystem.Interfaces;
using ElevatorControlSystem.Models;

namespace ElevatorControlSystem.Elevators
{
    public abstract class ElevatorMovementState
    {
        protected MovementStateContext? _context;

        public void SetContext(MovementStateContext context)
        {
            _context = context;
        }

        public abstract void MoveOneFloor(Elevator elevator);

        public abstract void PickUpPassenger(Elevator elevator, int destinationFloor);

        public abstract void ChangeDirection(Elevator elevator);


        public void OpenDoor(Elevator elevator)
        {
            var oldState = elevator.State;
            elevator.IsOpen = true;
            Thread.Sleep((int)elevator._dwellTime);
            elevator.IsOpen = false;
        }
        
        public void RequestFulfilled(Elevator elevator, FloorRequest? floorRequest)
        {
            // Remove request from queue after it is fulfilled
            if (floorRequest != null)
            {
                elevator.RequestQueue.RemoveAll(x => x.DestinationFloor == floorRequest.DestinationFloor);
            }
            OpenDoor(elevator);
        }

        public void SetToIdle(Elevator elevator)
        {
            elevator.State = ElevatorState.Idle;
        }

    }
}
