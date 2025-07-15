using ElevatorControlSystem.Interfaces;

namespace ElevatorControlSystem.Utilities
{
    public class Logger : ICustomLogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
