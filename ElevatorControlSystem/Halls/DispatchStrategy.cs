using ElevatorControlSystem.Elevators;

namespace ElevatorControlSystem.Halls
{
    public abstract class DispatchStrategy
    {
        public abstract void InitialDispatch(List<Elevator> elevators, HallRequest hallRequest);
        
        public abstract Elevator? SelectNearestElevator(List<Elevator> elevators, HallRequest hallRequest);
    }
}
