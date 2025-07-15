using ElevatorControlSystem.Elevators;
using ElevatorControlSystem.Enums;
using ElevatorControlSystem.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorControlSystem_Test.Tests
{
    public class ElevatorOperationTest
    {

        private readonly Mock<IConfiguration> _mockConfig;
        public ElevatorOperationTest()
        {
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

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Elevator_StartsOnFloorN(int currentFloor)
        {
            var elevator = CreateElevator(1, currentFloor);
            Assert.Equal(currentFloor, elevator.CurrentFloor);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(1, 3)]
        [InlineData(1, 4)]
        [InlineData(2, 1)]
        [InlineData(2, 2)]
        [InlineData(2, 3)]
        [InlineData(2, 4)]
        [InlineData(3, 1)]
        [InlineData(3, 2)]
        [InlineData(3, 3)]
        [InlineData(3, 4)]
        [InlineData(4, 1)]
        [InlineData(4, 2)]
        [InlineData(4, 3)]
        [InlineData(4, 4)]
        public void Elevator_PickUpPassengerOnFloorN(int currentFloor, int destinationFloor)
        {
            var elevator = CreateElevator(1, currentFloor);
            elevator.PickUpPassenger(destinationFloor);

            Assert.Equal(destinationFloor, elevator.CurrentFloor);
        }

        [Theory]
        [InlineData(ElevatorState.Up, 1, 3, ElevatorState.Down, 1)]
        [InlineData(ElevatorState.Up, 3, 5, ElevatorState.Down, 1)]
        [InlineData(ElevatorState.Down, 3, 1, ElevatorState.Up, 3)]
        [InlineData(ElevatorState.Down, 3, 2, ElevatorState.Up, 5)]
        public void Elevator_ChangeDirection(ElevatorState elevatorState1, int currentFloor1, int destinationFloor1, 
            ElevatorState elevatorState2, int destinationFloor2)
        {
            var elevator = CreateElevator(1, currentFloor1);
            elevator.PickUpPassenger(destinationFloor1);
            Assert.Equal(elevatorState1, elevator.State);
            
            elevator.PickUpPassenger(destinationFloor2);
            Assert.Equal(elevatorState2, elevator.State);
        }



    }
}
