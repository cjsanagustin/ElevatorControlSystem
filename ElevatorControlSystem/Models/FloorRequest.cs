
namespace ElevatorControlSystem.Models
{
    public class FloorRequest
    {
        public int OriginFloor { get; }
        public int DestinationFloor { get; }

        public FloorRequest(int originFloor, int destinationFloor)
        {
            OriginFloor = originFloor;
            DestinationFloor = destinationFloor;
        }
    }
}
