using ElevatorControlSystem.Elevators.States;
using ElevatorControlSystem.Enums;
using ElevatorControlSystem.Halls;
using ElevatorControlSystem.Interfaces;
using ElevatorControlSystem.Models;

namespace ElevatorControlSystem.Elevators
{
    public class Elevator
    {
        public int Id { get; }
        public int CurrentFloor { get; set; }
        public ElevatorState State { get; set; }
        public List<FloorRequest> PendingOppositeRequestQueue { get; set; } = [];
        public List<FloorRequest> RequestQueue = [];
        public string StatusSummary => $"Car {Id} - ({CurrentFloor.ToString("D2")}) {StateText} {RequestQueueText}";
        public bool IsOpen { get; set; }
        private string StateText
        {
            get
            {
                string result = "";
                if(IsOpen)
                    result = "[ ]";
                else if (State == ElevatorState.Up)
                    result = " /\\";
                else if (State == ElevatorState.Down)
                    result = " \\/";
                else if (State == ElevatorState.Idle)
                    result = "...";
                
                return result;
            }
        }
        private string RequestQueueText
        {
            get
            {
                string result = "";
                
                if(RequestQueue.Count > 0)
                    result = $"[{RequestQueue.First().OriginFloor} - {string.Join(",", RequestQueue.Select(x => x.DestinationFloor).OrderBy(x => x).ToArray())}]";

                return result;
            }
        }
        public bool IsMoving => RequestQueue.Count != 0;

        public readonly decimal _movementSpeed;
        public readonly decimal _dwellTime;
        public readonly int _numberOfFloors;

        public Elevator(int id, int currentFloor, IConfiguration configuration)
        {
            Id = id;
            CurrentFloor = currentFloor;
            State = ElevatorState.Idle;

            _numberOfFloors = int.Parse(configuration["ElevatorParameters:NumberOfFloors"] ?? "0");

            // Convert seconds into milliseconds
            _movementSpeed = decimal.Parse(configuration["ElevatorParameters:TravelTimePerFloor"] ?? "0") * (decimal)1000;
            _dwellTime = decimal.Parse(configuration["ElevatorParameters:FloorDwellTime"] ?? "0") * (decimal)1000;
        }

        public void Start()
        {
            var context = new MovementStateContext(new UpwardMovementState());
            context.MoveOneFloor(this);
        }

        public void PickUpPassenger(int destinationFloor)
        {
            if (CurrentFloor > destinationFloor)
            {
                new MovementStateContext(new DownwardMovementState()).PickUpPassenger(this, destinationFloor);
            }
            else
            {
                new MovementStateContext(new UpwardMovementState()).PickUpPassenger(this, destinationFloor);
            }
        }

    }
}
