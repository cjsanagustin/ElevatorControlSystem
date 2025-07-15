using ElevatorControlSystem.Halls;
using ElevatorControlSystem.Halls.Strategy;
using ElevatorControlSystem.Interfaces;
using ElevatorControlSystem.Services;
using ElevatorControlSystem.Utilities;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<ICustomLogger, Logger>();
        services.AddSingleton<DispatchStrategy, CentralDispatchStrategy>();
        services.AddSingleton<ElevatorSimulationService>();
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
var elevatorSimulationService = host.Services.GetRequiredService<ElevatorSimulationService>();
elevatorSimulationService.Start();
