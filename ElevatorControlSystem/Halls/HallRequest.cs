using ElevatorControlSystem.Elevators;
using ElevatorControlSystem.Enums;
using ElevatorControlSystem.Models;

namespace ElevatorControlSystem.Halls
{
    public class HallRequest
    {
        public int OriginFloor { get; }
        public RequestDirection RequestDirection { get; set; }
        public List<FloorRequest> RequestQueue { get; set; } = [];

        public HallRequest(int originFloor, RequestDirection requestDirection, List<FloorRequest> requestQueue)
        {
            OriginFloor = originFloor;
            RequestDirection = requestDirection;
            RequestQueue = requestQueue;
        }
    }
}
