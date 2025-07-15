using ElevatorControlSystem.Elevators;
using ElevatorControlSystem.Elevators.States;
using ElevatorControlSystem.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorControlSystem_Test.TestData
{
    public class ElevatorTestData
    {
        private readonly MovementStateContext _movementStateContextUpward;
        private readonly MovementStateContext _movementStateContextDownward;
        private readonly Mock<IConfiguration> _mockConfig;
        public ElevatorTestData()
        {
            _movementStateContextUpward = new MovementStateContext(new UpwardMovementState());
            _movementStateContextDownward = new MovementStateContext(new DownwardMovementState());
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["ElevatorParameters:NumberOfFloors"]).Returns("10");
            _mockConfig.Setup(c => c["ElevatorParameters:NumberOfElevators"]).Returns("4");
            _mockConfig.Setup(c => c["ElevatorParameters:TravelTimePerFloor"]).Returns("0");
            _mockConfig.Setup(c => c["ElevatorParameters:FloorDwellTime"]).Returns("0");
        }

        private Elevator CreateElevator(int id = 1, int currentFloor = 1)
        {
            return new Elevator(id, currentFloor, _mockConfig.Object);
        }

        public (Elevator, Elevator, Elevator) GetIdleElevator(int currentFloor1, int currentFloor2, int currentFloor3)
        {
            var elevator1 = CreateElevator(1, currentFloor1);
            var elevator2 = CreateElevator(2, currentFloor2);
            var elevator3 = CreateElevator(3, currentFloor3);

            return (elevator1, elevator2, elevator3);
        }

        public (Elevator, Elevator, Elevator) GetUpwardElevator_NoFloorRequests(int currentFloor1, int currentFloor2, int currentFloor3)
        {
            var elevator1 = CreateElevator(1, 1);
            _movementStateContextUpward.PickUpPassenger(elevator1, currentFloor1);
            var elevator2 = CreateElevator(2, 1);
            _movementStateContextUpward.PickUpPassenger(elevator2, currentFloor2);
            var elevator3 = CreateElevator(3, 1);
            _movementStateContextUpward.PickUpPassenger(elevator3, currentFloor3);

            return (elevator1, elevator2, elevator3);
        }

        public (Elevator, Elevator, Elevator) GetDownwardElevator_NoFloorRequests(int currentFloor1, int currentFloor2, int currentFloor3)
        {
            var elevator1 = CreateElevator(1, 10);
            _movementStateContextDownward.PickUpPassenger(elevator1, currentFloor1);
            var elevator2 = CreateElevator(2, 10);
            _movementStateContextDownward.PickUpPassenger(elevator2, currentFloor2);
            var elevator3 = CreateElevator(3, 10);
            _movementStateContextDownward.PickUpPassenger(elevator3, currentFloor3);

            return (elevator1, elevator2, elevator3);
        }

        public (Elevator, Elevator, Elevator) GetUpwardElevator_WithFloorRequests(int currentFloor1, int currentFloor2, int currentFloor3)
        {
            var elevator1 = CreateElevator(1, 1);
            _movementStateContextUpward.PickUpPassenger(elevator1, currentFloor1);
            elevator1.RequestQueue.Add(new FloorRequest(currentFloor1, currentFloor1 + 1));
            elevator1.RequestQueue.Add(new FloorRequest(currentFloor1, currentFloor1 + 4));

            var elevator2 = CreateElevator(2, 1);
            _movementStateContextUpward.PickUpPassenger(elevator2, currentFloor2);
            elevator2.RequestQueue.Add(new FloorRequest(currentFloor2, currentFloor2 + 1));
            elevator2.RequestQueue.Add(new FloorRequest(currentFloor2, currentFloor2 + 2));
            elevator2.RequestQueue.Add(new FloorRequest(currentFloor2, currentFloor2 + 3));
            elevator2.RequestQueue.Add(new FloorRequest(currentFloor2, currentFloor2 + 4));

            var elevator3 = CreateElevator(3, 1);
            _movementStateContextUpward.PickUpPassenger(elevator3, currentFloor3);
            elevator3.RequestQueue.Add(new FloorRequest(currentFloor3, currentFloor3 + 1));

            return (elevator1, elevator2, elevator3);
        }

        public (Elevator, Elevator, Elevator) GetDownwardElevator_WithFloorRequests(int currentFloor1, int currentFloor2, int currentFloor3)
        {
            var elevator1 = CreateElevator(1, 10);
            _movementStateContextDownward.PickUpPassenger(elevator1, currentFloor1);
            elevator1.RequestQueue.Add(new FloorRequest(currentFloor1, currentFloor1 - 1));
            elevator1.RequestQueue.Add(new FloorRequest(currentFloor1, currentFloor1 - 2));

            var elevator2 = CreateElevator(2, 10);
            _movementStateContextDownward.PickUpPassenger(elevator2, currentFloor2);
            elevator2.RequestQueue.Add(new FloorRequest(currentFloor2, currentFloor2 - 1));
            elevator2.RequestQueue.Add(new FloorRequest(currentFloor2, currentFloor2 - 2));
            elevator2.RequestQueue.Add(new FloorRequest(currentFloor2, currentFloor2 - 3));
            elevator2.RequestQueue.Add(new FloorRequest(currentFloor2, currentFloor2 - 4));

            var elevator3 = CreateElevator(3, 10);
            _movementStateContextDownward.PickUpPassenger(elevator3, currentFloor3);
            elevator3.RequestQueue.Add(new FloorRequest(currentFloor3, currentFloor3 - 1));

            return (elevator1, elevator2, elevator3);
        }

    }
}
