using ElevatorControlSystem.Elevators;
using ElevatorControlSystem.Enums;
using ElevatorControlSystem.Halls;
using ElevatorControlSystem.Interfaces;
using ElevatorControlSystem.Models;

namespace ElevatorControlSystem.Services
{
    public class ElevatorSimulationService
    {
        private readonly ICustomLogger _logger;
        private readonly IConfiguration _configuration;
        private readonly List<Elevator> elevators = [];
        private readonly Random rand = new Random();
        private readonly int _elevatorCount;
        private readonly int _floorCount;
        private readonly int _randFloorCount;
        private readonly decimal _movementSpeed;
        private readonly decimal _dwellTime;

        private readonly DispatchStrategy _dispatchStrategy;

        public ElevatorSimulationService(ICustomLogger logger, IConfiguration configuration, DispatchStrategy dispatchStrategy)
        {
            _logger = logger;
            _configuration = configuration;

            // Setup Parameters
            _elevatorCount = int.Parse(configuration["ElevatorParameters:NumberOfElevators"] ?? "0");
            _floorCount = int.Parse(configuration["ElevatorParameters:NumberOfFloors"] ?? "0");
            _movementSpeed = decimal.Parse(configuration["ElevatorParameters:TravelTimePerFloor"] ?? "0");
            _dwellTime = decimal.Parse(configuration["ElevatorParameters:FloorDwellTime"] ?? "0");
            _randFloorCount = _floorCount + 1;
            
            elevators = CreateDefaultElevators(_elevatorCount);
            _dispatchStrategy = dispatchStrategy;
        }

        public void Start()
        {
            int DurationMinutes = 3;
            _logger.Log($"Number of Elevators:{_elevatorCount}, Number of Floors:{_floorCount}, Elevator speed(secs):{(int)_movementSpeed}, Dwell Time(secs):{(int)_dwellTime} ");
            _logger.Log($"Elevator Legends: ... - Idle,  [ ] - Embark/Disembark, \\/ - Moving Down, /\\ - Moving Up");
            _logger.Log($"------------------------------------------------------------------------------------------");

            Task showStatus = Task.Run(() => ShowElevatorStatusesPerSecond(DurationMinutes));
            Task generate = Task.Run(() => GenerateRandomHallRequests(DurationMinutes));
            Task.WaitAll(showStatus, generate);
        }

        private List<FloorRequest> GenerateFloorRequests(int currentFloor, RequestDirection requestDirection)
        {
            List<FloorRequest> result = [];
            if (requestDirection == RequestDirection.Upward)
            {
                for (int i = currentFloor + 1; i <= _floorCount; i++)
                {
                    if (rand.Next(2) == 0)
                    {
                        var destination = rand.Next(i, _randFloorCount);
                        result.Add(new FloorRequest(currentFloor, destination));
                    }
                }
            }
            else
            {
                for (int i = currentFloor - 1; i >= 1; i--)
                {
                    if (rand.Next(2) == 0)
                    {
                        var destination = rand.Next(1, i);
                        result.Add(new FloorRequest(currentFloor, destination));
                    }
                }
            }
            
            result = result.GroupBy(x => x.DestinationFloor).Select(x => x.First()).ToList();
            if (result.Count == 0)
            {
                result = GenerateFloorRequests(currentFloor, requestDirection);
            }
            return result;
        }

        /// <summary>
        /// Generate hall requests on random floors in random delays for N number of minutes
        /// </summary>
        /// <param name="Minutes">Duration of generating random hall requests</param>
        private void GenerateRandomHallRequests(int Minutes)
        {
            DateTime startTime = DateTime.Now;
            TimeSpan duration = TimeSpan.FromMinutes(Minutes);
            while (DateTime.Now - startTime < duration)
            {
                var floor = rand.Next(1, _randFloorCount);

                // Request direction is fixed for the first and last floor
                RequestDirection direction;
                if (floor == 1)
                    direction = RequestDirection.Upward;
                else if (floor == _floorCount)
                    direction = RequestDirection.Downward;
                else
                    direction = rand.Next(2) == 0 ? RequestDirection.Upward: RequestDirection.Downward;

                List<FloorRequest> floorRequests = GenerateFloorRequests(floor, direction);
                var hallRequest = new HallRequest(floor, direction, floorRequests);

                var floorsArr = floorRequests.Select(x => x.DestinationFloor).OrderBy(x => x).ToArray();
                _logger.Log($"{DateTime.Now.ToString("HH:mm:ss")} > NEW REQUEST floor#:{floor},{direction.ToString().ToUpper()} | requests:[{string.Join(",", floorsArr)}].");

                Task.Run(() => {
                    _dispatchStrategy.InitialDispatch(elevators, hallRequest);
                });
                
                // Add delay between hall requests, ranging from 20 seconds to 1 minute
                int requestDelay = rand.Next(20, 60) * 1000;
                Thread.Sleep(requestDelay);
            }
        }

        /// <summary>
        /// Show status for all elevators every 2000ms for N number of minutes
        /// </summary>
        private void ShowElevatorStatusesPerSecond(int Minutes)
        {
            bool withElevatorMovement = true;
            ShowElevatorStatus();

            DateTime startTime = DateTime.Now;
            TimeSpan duration = TimeSpan.FromMinutes(Minutes);
            while ((DateTime.Now - startTime < duration) || withElevatorMovement)
            {
                ShowElevatorStatus();
                withElevatorMovement = elevators.Where(x => x.State != ElevatorState.Idle).Count() > 0;
            }
            ShowElevatorStatus();
            _logger.Log("-- ALL ELEVATORS ARE NOW IDLE --");
        }

        private void ShowElevatorStatus()
        {
            var statusArr = elevators.Select(x => x.StatusSummary).ToArray();
            _logger.Log(string.Concat(DateTime.Now.ToString("HH:mm:ss")," > ", string.Join(" | ", statusArr)));
            Thread.Sleep(2000);
        }

        /// <summary>
        /// Initialize elevators with random current floors
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private List<Elevator> CreateDefaultElevators(int count)
        {
            var list = new List<Elevator>();
            for (int i = 1; i <= count; i++)
                list.Add(new Elevator(i, rand.Next(1, _randFloorCount), _configuration));
            return list;
        }

    }
}
