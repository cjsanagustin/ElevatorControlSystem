using ElevatorControlSystem.Enums;
using ElevatorControlSystem.Halls;

namespace ElevatorControlSystem.Elevators
{
    public class ElevatorTravelTime
    {
        public decimal ComputeTraveltime(Elevator elevator, HallRequest hallRequest)
        {
            return ComputeTravelTimeUpward(elevator, hallRequest) +
                ComputeTravelTimeDownward(elevator, hallRequest) +
                ComputeRequestTimeUpward(elevator, hallRequest) +
                ComputeRequestTimeDownward(elevator, hallRequest);
        }

        private decimal ComputeTravelTimeUpward(Elevator elevator, HallRequest hallRequest)
        {
            decimal totalTravelTime = 0;

            // If same direction with the hall request, idle or open
            if ((elevator.State != ElevatorState.Down) && hallRequest.RequestDirection == RequestDirection.Upward)
            {
                totalTravelTime = Math.Abs(elevator.CurrentFloor - hallRequest.OriginFloor);
            }

            // If opposite direction with the hall request
            else if (elevator.State == ElevatorState.Down && hallRequest.RequestDirection == RequestDirection.Upward)
            {
                int count = elevator.RequestQueue.Count();
                if (count > 0)
                {
                    // Get lowest floor request to compute travelTime
                    int lowestFloorReq = elevator.RequestQueue.OrderBy(x => x.DestinationFloor).First().DestinationFloor;
                    totalTravelTime = Math.Abs(lowestFloorReq - elevator.CurrentFloor);

                    // Get roundtrip travel time starting from the lowest floor request
                    decimal roundTripTravelTime = Math.Abs(lowestFloorReq - hallRequest.OriginFloor);

                    // Consolidate
                    totalTravelTime += roundTripTravelTime;
                }
                else
                {
                    totalTravelTime = Math.Abs(elevator.CurrentFloor - hallRequest.OriginFloor);
                }
            }
            return totalTravelTime * (elevator._movementSpeed == 0 ? 1 : elevator._movementSpeed);
        }

        private decimal ComputeTravelTimeDownward(Elevator elevator, HallRequest hallRequest)
        {
            decimal totalTravelTime = 0;

            // If same direction with the hall request, idle or open
            if ((elevator.State != ElevatorState.Up) && hallRequest.RequestDirection == RequestDirection.Downward)
            {
                totalTravelTime = Math.Abs(elevator.CurrentFloor - hallRequest.OriginFloor);
            }

            // If opposite direction with the hall request
            else if (elevator.State == ElevatorState.Up && hallRequest.RequestDirection == RequestDirection.Downward)
            {
                int count = elevator.RequestQueue.Count();
                if (count > 0)
                {
                    // Get highest floor request to compute travelTime
                    int highestFloorReq = elevator.RequestQueue.OrderByDescending(x => x.DestinationFloor).First().DestinationFloor;
                    totalTravelTime = Math.Abs(highestFloorReq - elevator.CurrentFloor);

                    // Get roundtrip travel time starting from the highest floor request
                    decimal roundTripTravelTime = Math.Abs(highestFloorReq - hallRequest.OriginFloor);

                    // Consolidate
                    totalTravelTime += roundTripTravelTime;
                }
                else
                {
                    totalTravelTime = Math.Abs(elevator.CurrentFloor - hallRequest.OriginFloor);
                }
            }

            return totalTravelTime * (elevator._movementSpeed == 0 ? 1 : elevator._movementSpeed);
        }

        private decimal ComputeRequestTimeUpward(Elevator elevator, HallRequest hallRequest)
        {
            decimal totalRequestTime = 0;

            // If same direction with the hall request, idle or open, where CurrentFloor is less than or equal to the request OriginFloor
            if ((elevator.State != ElevatorState.Down) && hallRequest.RequestDirection == RequestDirection.Upward
                && elevator.CurrentFloor <= hallRequest.OriginFloor)
            {
                totalRequestTime = elevator.RequestQueue.Where(x => x.DestinationFloor <= hallRequest.OriginFloor).Count();
            }

            // If CurrentFloor is greater than the request OriginFloor
            else if (hallRequest.RequestDirection == RequestDirection.Upward)
            {
                int count = elevator.RequestQueue.Count();
                totalRequestTime = count;
                if (count > 0)
                {
                    // Get highest floor request to compute roundtrip
                    int highestFloorReq = elevator.RequestQueue.OrderByDescending(x => x.DestinationFloor).First().DestinationFloor;
                    totalRequestTime += Math.Abs(highestFloorReq - elevator.CurrentFloor);

                    // Get roundtrip travel time starting from the lowest floor request
                    decimal roundTripTravelTime = Math.Abs(highestFloorReq - hallRequest.OriginFloor);

                    // Consolidate
                    totalRequestTime += roundTripTravelTime;
                }
            }
            
            return totalRequestTime * (elevator._dwellTime == 0 ? 1 : elevator._dwellTime);

        }

        private decimal ComputeRequestTimeDownward(Elevator elevator, HallRequest hallRequest)
        {
            decimal totalRequestTime = 0;

            // If same direction with the hall request, idle or open, where CurrentFloor is greater than or equal to the request OriginFloor
            if ((elevator.State != ElevatorState.Up) && hallRequest.RequestDirection == RequestDirection.Downward
                && elevator.CurrentFloor >= hallRequest.OriginFloor)
            {
                totalRequestTime = elevator.RequestQueue.Where(x => x.DestinationFloor >= hallRequest.OriginFloor).Count();
            }

            // If CurrentFloor is less than the request OriginFloor
            else if (hallRequest.RequestDirection == RequestDirection.Downward)
            {
                int count = elevator.RequestQueue.Count();
                totalRequestTime = count;
                if (count > 0)
                {
                    // Get lowest floor request to compute roundtrip
                    int lowestFloorReq = elevator.RequestQueue.OrderBy(x => x.DestinationFloor).First().DestinationFloor;
                    totalRequestTime += Math.Abs(lowestFloorReq - elevator.CurrentFloor);

                    // Get roundtrip travel time starting from the lowest floor request
                    decimal roundTripTravelTime = Math.Abs(lowestFloorReq - hallRequest.OriginFloor);

                    // Consolidate
                    totalRequestTime += roundTripTravelTime;
                }
            }

            return totalRequestTime * (elevator._dwellTime == 0 ? 1 : elevator._dwellTime);
        }

    }
}
