using System;
public static class ExponentialSampler
{
    public static double Sample(double mean)
    {
        double u = new Random().NextDouble();
        double lambda = (-1/mean);
        return (Math.Log(1 - u))/lambda;
    }
}