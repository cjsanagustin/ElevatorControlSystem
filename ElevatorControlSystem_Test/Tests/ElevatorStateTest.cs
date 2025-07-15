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
    public class ElevatorStateTest
    {

        private Elevator CreateElevator(int id = 1, int currentFloor = 1, string TravelTimePerFloor = "0", string FloorDwellTime = "0")
        {
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["ElevatorParameters:NumberOfFloors"]).Returns("10");
            mockConfig.Setup(c => c["ElevatorParameters:NumberOfElevators"]).Returns("4");
            mockConfig.Setup(c => c["ElevatorParameters:TravelTimePerFloor"]).Returns(TravelTimePerFloor);
            mockConfig.Setup(c => c["ElevatorParameters:FloorDwellTime"]).Returns(FloorDwellTime);
            return new Elevator(id, currentFloor, mockConfig.Object);
        }

        [Fact]
        public void Elevator_IdleState()
        {
            var elevator = CreateElevator();
            Assert.Equal(ElevatorState.Idle, elevator.State);
        }

        [Theory]
        [InlineData(2, 0.1)]
        [InlineData(3, 0.1)]
        [InlineData(4, 0.1)]
        public void Elevator_UpState(int floorRequest, decimal delayInSeconds)
        {
            int currentFloor = 1;
            // Set N seconds travel time
            var elevator = CreateElevator(1, currentFloor, delayInSeconds.ToString());
            elevator.RequestQueue.Add(new FloorRequest(currentFloor, floorRequest));

            int floorDiff = Math.Abs(currentFloor - floorRequest);
            decimal totalDelayInSeconds = floorDiff * delayInSeconds;

            var start = Task.Run(() => elevator.Start());

            // Check status in the middle of delay
            var checkUpState = Task.Run(() => {
                Thread.Sleep(Convert.ToInt32(((decimal)totalDelayInSeconds / (decimal)2) * (decimal)1000));
                Assert.Equal(ElevatorState.Up, elevator.State);
            });

            // Check status after the delay
            var checkIdleState = Task.Run(() => {
                Thread.Sleep((int)(totalDelayInSeconds + floorDiff + 2) * 1000);
                Assert.Equal(ElevatorState.Idle, elevator.State);
            });

            Task.WaitAll(start, checkUpState, checkIdleState);

        }

        [Theory]
        [InlineData(4, 0.1)]
        [InlineData(3, 0.1)]
        [InlineData(2, 0.1)]
        public void Elevator_DownState(int floorRequest, decimal delayInSeconds)
        {
            int currentFloor = 5;

            // Set N seconds travel time
            var elevator = CreateElevator(1, currentFloor, delayInSeconds.ToString());
            elevator.RequestQueue.Add(new FloorRequest(currentFloor, floorRequest));

            int floorDiff = Math.Abs(currentFloor - floorRequest);
            decimal totalDelayInSeconds = floorDiff * delayInSeconds;

            var start = Task.Run(() => elevator.Start());

            // Check status in the middle of delay
            var checkDownState = Task.Run(() => {
                Thread.Sleep(Convert.ToInt32(((decimal)totalDelayInSeconds / (decimal)2) * (decimal)1000));
                Assert.Equal(ElevatorState.Down, elevator.State);
            });

            // Check status after the delay
            var checkIdleState = Task.Run(() => {
                Thread.Sleep((int)(totalDelayInSeconds + floorDiff + 2) * 1000);
                Assert.Equal(ElevatorState.Idle, elevator.State);
            });

            Task.WaitAll(start, checkDownState, checkIdleState);

        }

        [Theory]
        [InlineData(2, 0.5, 2)]
        [InlineData(3, 0.5, 2)]
        [InlineData(4, 0.5, 2)]
        public void Elevator_OpenState(int floorRequest, decimal delayInSeconds, decimal dwellTimeInSeconds)
        {
            int currentFloor = 1;

            // Set N seconds travel time
            var elevator = CreateElevator(1, currentFloor, delayInSeconds.ToString(), dwellTimeInSeconds.ToString());
            elevator.RequestQueue.Add(new FloorRequest(currentFloor, floorRequest));

            int floorDiff = Math.Abs(currentFloor - floorRequest);
            decimal totalDelayInSeconds = floorDiff * delayInSeconds;

            var start = Task.Run(() => elevator.Start());

            // Check status 
            var checkOpenState = Task.Run(() => {
                Thread.Sleep((int)(totalDelayInSeconds + dwellTimeInSeconds + floorDiff + 2) * 1000);
                Assert.Equal(true, elevator.IsOpen);
            });

            // Check status after the delay
            var checkIdleState = Task.Run(() => {
                Thread.Sleep((int)(totalDelayInSeconds + dwellTimeInSeconds + floorDiff + 4) * 1000);
                Assert.Equal(ElevatorState.Idle, elevator.State);
            });

            Task.WaitAll(start, checkOpenState, checkIdleState);

        }


    }
}
