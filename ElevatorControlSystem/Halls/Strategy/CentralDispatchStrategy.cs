using ElevatorControlSystem.Elevators;
using ElevatorControlSystem.Enums;

namespace ElevatorControlSystem.Halls.Strategy
{
    public class CentralDispatchStrategy : DispatchStrategy
    {
        public override void InitialDispatch(List<Elevator> elevators, HallRequest hallRequest)
        {
            // Select the nearest elevator by Computed ETA
            var elevator = SelectNearestElevator(elevators, hallRequest);

            if (elevator != null)
            {
                if (elevator.RequestQueue.Count == 0)
                {
                    elevator.RequestQueue.AddRange(hallRequest.RequestQueue);
                    elevator.PickUpPassenger(hallRequest.OriginFloor);
                    elevator.Start();
                }
                else
                {
                    
                    if (
                        (
                            hallRequest.RequestDirection == RequestDirection.Upward 
                            && elevator.State == ElevatorState.Up
                        )
                        ||
                        (
                            hallRequest.RequestDirection == RequestDirection.Downward
                            && elevator.State == ElevatorState.Down
                        )
                        ||
                        elevator.State == ElevatorState.Idle
                    )
                    {
                        elevator.RequestQueue.AddRange(hallRequest.RequestQueue);
                    }
                    else
                    {
                        // this will be transferred into RequestQueue upon Changing of direction
                        elevator.PendingOppositeRequestQueue.AddRange(hallRequest.RequestQueue);
                    }
                }
            }
        }

        public override Elevator? SelectNearestElevator(List<Elevator> elevators, HallRequest hallRequest)
        {
            return elevators
                .OrderBy(x => new ElevatorTravelTime().ComputeTraveltime(x, hallRequest)).FirstOrDefault();
        }

    }
}
