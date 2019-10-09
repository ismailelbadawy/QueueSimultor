using System;
using MathNet.Numerics;

namespace Waiting_Queue
{
    class Program
    {
        static void Main(string[] args)
        {
            new PrivelegedQueueSimulator().Run(1.0, 1.1);
        }
    }
}
