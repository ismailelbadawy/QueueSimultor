public interface ISimulator
{
    SimulatorViewModel Run(double arrivalTimeMean, double serviceTimeMean);
}

///<summary>
/// This view model allows the simulators to output the same set of parameters
///</summary>
public class SimulatorViewModel
{
    public double Profit { get; set; }
    public double AverageWaitingTime { get; set; }
    public double AverageWaitingTimePrivileged { get; set; }
    public double AverageWaitingTimeNonPrivileged { get; set; }
}