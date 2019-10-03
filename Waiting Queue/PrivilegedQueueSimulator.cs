
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;

public class PrivelegedQueueSimulator : ISimulator
{
    private double _currentTime { get; set; }
    private List<Tourist> _queue = new List<Tourist>();

    public SimulatorViewModel Run(double arrivalTimeIntervalMean, double serviceTimeMean)
    {
        _currentTime = 0;
        double profit = 0;
        Tourist inService = null;
        while (this.IsOperating())
        {
            if (this.IsOpen())
            {
                double arrivalTimeInterval = ExponentialSampler.Sample(arrivalTimeIntervalMean);
                if (!(arrivalTimeInterval + _currentTime > 360))
                {
                    this._queue.Add(new Tourist()
                    {
                        ArrivalTime = _currentTime + arrivalTimeInterval,
                        ServiceTime = ExponentialSampler.Sample(serviceTimeMean),
                        IsPrivileged = _queue.Count != 0 && new Random().NextDouble() > 0.5
                    });
                }
                _currentTime += arrivalTimeInterval;
                if (inService.ServiceTime + inService.StartTime > _currentTime)
                {
                    inService = null;
                    // Switch in service tourist.
                    var firstPrivileged = this._queue.FirstOrDefault(s => s.IsPrivileged);
                    var firstNonPrivileged = this._queue.FirstOrDefault(s => !s.IsPrivileged);
                    if (firstPrivileged == null)
                    {
                        if(firstNonPrivileged == null)
                        {

                        } else 
                        {
                            
                        }
                    }
                }
                Log();
                Thread.Sleep(500);
            }
        }

        return new SimulatorViewModel()
        {
            AverageWaitingTime = 0,
            AverageWaitingTimeNonPrivileged = 0,
            AverageWaitingTimePrivileged = 0,
            Profit = profit
        };
    }

    private void Log()
    {
        Console.WriteLine($"Current Time {this._currentTime}");
        Console.WriteLine($"State of the system : {(IsOperating() ? "Operating" : "Stopped")}");
        Console.WriteLine($"State of the doors: {(IsOpen() ? "Open" : "Closed")}");
        Console.WriteLine($"People in queue: {this._queue.Count} people, {this._queue.Where(s => s.IsPrivileged).Count()} in privilege");
    }

    private bool IsOpen()
    {
        return _currentTime < (6 * 60);
    }

    private bool IsOperating()
    {
        return this.IsOpen() || this._queue.Count != 0;
    }
}

public class Tourist
{
    public double ArrivalTime { get; set; }
    public double ServiceTime { get; set; }
    public bool IsPrivileged { get; set; }
    public double StartTime { get; set; }
    public Tourist() { }
}