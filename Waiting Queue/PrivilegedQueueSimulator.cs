
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;

public class PrivelegedQueueSimulator : ISimulator
{
    private double _currentTime { get; set; }
    private List<Tourist> _queue = new List<Tourist>();
    private Tourist _inService;

    public SimulatorViewModel Run(double arrivalTimeIntervalMean, double serviceTimeMean)
    {
        this._currentTime = 0;
        double profit = 0;
        this._inService = null;

        Arrive(arrivalTimeIntervalMean, serviceTimeMean);
        this._currentTime = 0.0;
        this.ProcessQueue();

        return new SimulatorViewModel()
        {
            AverageWaitingTime = 0,
            AverageWaitingTimeNonPrivileged = 0,
            AverageWaitingTimePrivileged = 0,
            Profit = profit
        };
    }

    private void ProcessQueue()
    {
        while (this.IsOperating())
        {
            var arrivedTourists = this._queue.Where(s => s.ArrivalTime <= this._currentTime).ToList();
            if (arrivedTourists.Count != 0)
            {
                if (arrivedTourists.FirstOrDefault(z => z.IsPrivileged) == null)
                {
                    this._inService = arrivedTourists.First();
                }
                else
                {
                    this._inService = arrivedTourists.First(s => s.IsPrivileged);
                }
                this._queue.Remove(this._inService);
                this.StartService(this._inService);
            }
            if (_inService != null)
            {
                this._currentTime += this._inService.ServiceTime;
                this.FinishService(this._inService);
                this._inService = null;
            }
            else if (this._queue.Count != 0)
            {
                if (this._queue.Where(s => s.ArrivalTime < this._currentTime).Count() == 0)
                {
                    this._currentTime = this._queue.First().ArrivalTime;
                }
            }
            Log();
            Thread.Sleep(200);
        }
    }

    private void Arrive(double arrivalTimeIntervalMean, double serviceTimeMean)
    {
        while (this.IsOpen())
        {
            double arrivalTimeInterval = ExponentialSampler.Sample(arrivalTimeIntervalMean);
            if (!(arrivalTimeInterval + _currentTime >= 360.0))
            {
                this._queue.Add(new Tourist()
                {
                    ArrivalTime = _currentTime + arrivalTimeInterval,
                    ServiceTime = ExponentialSampler.Sample(serviceTimeMean),
                    IsPrivileged = _queue.Count != 0 && new Random().NextDouble() > 0.5
                });

                this._currentTime += arrivalTimeInterval;
            }
            else
            {
                this._currentTime = 360.0;
            }
            Log();
            Thread.Sleep(200);
        }
    }

    private void StartService(Tourist tourist)
    {
        tourist.StartTime = this._currentTime;
    }
    private void FinishService(Tourist tourist)
    {
        tourist.FinishTime = this._currentTime;
        tourist.WaitingTime = tourist.FinishTime - tourist.StartTime;
    }

    ///<summary> 
    /// This function logs the current state of the option.
    /// </summary>
    private void Log()
    {
        Console.WriteLine($"Current Time {this._currentTime}");
        Console.WriteLine($"State of the system : {(IsOperating() ? "Operating" : "Stopped")}");
        Console.WriteLine($"State of the doors: {(IsOpen() ? "Open" : "Closed")}");
        Console.WriteLine($"People in queue: {this._queue.Where(s => s.ArrivalTime <= this._currentTime).Count()} people, {this._queue.Where(s => s.IsPrivileged && s.ArrivalTime <= this._currentTime).Count()} in privilege");
    }

    private bool IsOpen()
    {
        return _currentTime < (6 * 60);
    }

    private bool IsOperating()
    {
        return this._queue.Count != 0;
    }
}

public class Tourist
{
    public double ArrivalTime { get; set; }
    public double ServiceTime { get; set; }
    public bool IsPrivileged { get; set; }
    public double StartTime { get; set; }
    public double FinishTime { get; set; }
    public double WaitingTime { get; set; }
    public Tourist() { }
}