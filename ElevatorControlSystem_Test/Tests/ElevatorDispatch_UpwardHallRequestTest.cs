using ElevatorControlSystem.Elevators;
using ElevatorControlSystem.Elevators.States;
using ElevatorControlSystem.Enums;
using ElevatorControlSystem.Halls;
using ElevatorControlSystem.Halls.Strategy;
using ElevatorControlSystem.Models;
using ElevatorControlSystem_Test.TestData;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorControlSystem_Test.Tests
{
    public class ElevatorDispatch_UpwardHallRequestTest
    {

        private readonly CentralDispatchStrategy _strategy;
        private readonly ElevatorTestData _elevatorTestData;

        public ElevatorDispatch_UpwardHallRequestTest()
        {
            _strategy = new CentralDispatchStrategy();
            _elevatorTestData = new ElevatorTestData();
        }

        [Theory]
        [InlineData(3, 4, 5, 1, 1)]
        [InlineData(3, 4, 5, 3, 1)]
        [InlineData(3, 4, 5, 4, 2)]
        [InlineData(3, 4, 5, 5, 3)]
        [InlineData(3, 4, 5, 7, 3)]
        public void Elevator_FromIdleElevatorOnly(int currentFloor1, int currentFloor2, int currentFloor3
            , int hallRequestOriginFloor, int dispatchedElevatorId)
        {
            var (elevator1, elevator2, elevator3) = _elevatorTestData.GetIdleElevator(currentFloor1, currentFloor2, currentFloor3);

            var dispatchedElevator = _strategy.SelectNearestElevator(new List<Elevator> { elevator1, elevator2, elevator3 }
                , new HallRequest(hallRequestOriginFloor, RequestDirection.Upward, new List<FloorRequest>()));

            Assert.Equal(ElevatorState.Idle, elevator1.State);
            Assert.Equal(ElevatorState.Idle, elevator2.State);
            Assert.Equal(ElevatorState.Idle, elevator3.State);
            Assert.Equal(dispatchedElevatorId, dispatchedElevator != null ? dispatchedElevator.Id : 0);
        }

        [Theory]
        [InlineData(3, 4, 5, 1, 1)]
        [InlineData(3, 4, 5, 3, 1)]
        [InlineData(3, 4, 5, 4, 2)]
        [InlineData(3, 4, 5, 5, 3)]
        public void Elevator_FromUpwardElevatorOnly_NoFloorRequests(int currentFloor1, int currentFloor2, int currentFloor3
            , int hallRequestOriginFloor, int dispatchedElevatorId)
        {
            var (elevator1, elevator2, elevator3) = _elevatorTestData.GetUpwardElevator_NoFloorRequests(currentFloor1, currentFloor2, currentFloor3);

            var dispatchedElevator = _strategy.SelectNearestElevator(new List<Elevator> { elevator1, elevator2, elevator3 }
                , new HallRequest(hallRequestOriginFloor, RequestDirection.Upward, new List<FloorRequest>()));
            
            Assert.Equal(ElevatorState.Up, elevator1.State);
            Assert.Equal(ElevatorState.Up, elevator2.State);
            Assert.Equal(ElevatorState.Up, elevator3.State);
            Assert.Equal(dispatchedElevatorId, dispatchedElevator != null ? dispatchedElevator.Id : 0);
        }

        [Theory]
        [InlineData(3, 4, 5, 1, 3)]
        [InlineData(3, 4, 5, 3, 1)]
        [InlineData(3, 4, 5, 4, 2)]
        [InlineData(3, 4, 5, 5, 3)]
        [InlineData(3, 4, 5, 7, 3)]
        [InlineData(3, 4, 5, 9, 3)]
        public void Elevator_FromUpwardElevatorOnly_WithFloorRequests(int currentFloor1, int currentFloor2, int currentFloor3
            , int hallRequestOriginFloor, int dispatchedElevatorId)
        {
            var (elevator1, elevator2, elevator3) = _elevatorTestData.GetUpwardElevator_WithFloorRequests(currentFloor1, currentFloor2, currentFloor3);

            var dispatchedElevator = _strategy.SelectNearestElevator(new List<Elevator> { elevator1, elevator2, elevator3 }
                , new HallRequest(hallRequestOriginFloor, RequestDirection.Upward, new List<FloorRequest>()));

            Assert.Equal(ElevatorState.Up, elevator1.State);
            Assert.Equal(ElevatorState.Up, elevator2.State);
            Assert.Equal(ElevatorState.Up, elevator3.State);
            Assert.Equal(dispatchedElevatorId, dispatchedElevator != null ? dispatchedElevator.Id : 0);
        }

        [Theory]
        [InlineData(5, 6, 7, 5, 1)]
        [InlineData(5, 6, 7, 6, 2)]
        [InlineData(5, 6, 7, 9, 3)]
        public void Elevator_FromDownwardElevatorOnly_NoFloorRequests(int currentFloor1, int currentFloor2, int currentFloor3
            , int hallRequestOriginFloor, int dispatchedElevatorId)
        {
            var (elevator1, elevator2, elevator3) = _elevatorTestData.GetDownwardElevator_NoFloorRequests(currentFloor1, currentFloor2, currentFloor3);
            
            var dispatchedElevator = _strategy.SelectNearestElevator(new List<Elevator> { elevator1, elevator2, elevator3 }
                , new HallRequest(hallRequestOriginFloor, RequestDirection.Upward, new List<FloorRequest>()));

            Assert.Equal(ElevatorState.Down, elevator1.State);
            Assert.Equal(ElevatorState.Down, elevator2.State);
            Assert.Equal(ElevatorState.Down, elevator3.State);
            Assert.Equal(dispatchedElevatorId, dispatchedElevator != null ? dispatchedElevator.Id : 0);
        }

        [Theory]
        [InlineData(5, 6, 7, 1, 1)]
        [InlineData(5, 6, 7, 3, 1)]
        [InlineData(5, 6, 7, 5, 3)]
        [InlineData(5, 6, 7, 7, 3)]
        [InlineData(5, 6, 7, 9, 3)]
        public void Elevator_FromDownwardElevatorOnly_WithFloorRequests(int currentFloor1, int currentFloor2, int currentFloor3
            , int hallRequestOriginFloor, int dispatchedElevatorId)
        {
            var (elevator1, elevator2, elevator3) = _elevatorTestData.GetDownwardElevator_WithFloorRequests(currentFloor1, currentFloor2, currentFloor3);

            var dispatchedElevator = _strategy.SelectNearestElevator(new List<Elevator> { elevator1, elevator2, elevator3 }
                , new HallRequest(hallRequestOriginFloor, RequestDirection.Upward, new List<FloorRequest>()));

            Assert.Equal(ElevatorState.Down, elevator1.State);
            Assert.Equal(ElevatorState.Down, elevator2.State);
            Assert.Equal(ElevatorState.Down, elevator3.State);
            Assert.Equal(dispatchedElevatorId, dispatchedElevator != null ? dispatchedElevator.Id : 0);
        }
    }
}
