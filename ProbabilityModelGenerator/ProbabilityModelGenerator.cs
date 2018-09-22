using System;
using System.IO;
using System.Collections.Generic;

namespace ProbabilityModelGenerator
{
    public class ProbabilityModelGenerator
    {
        public delegate double DistributionFunctionCallback(double x);

        public long TotalData
        { get; protected set; }

        protected DistributionFunctionCallback DistributionFunction
        { get; set; }
        protected Stream OutputStream
        { get; set; }  

        public void Generate(double leftBound, double rightBound, double step, long amount)
        {
            if (rightBound < leftBound)
            {
                throw new ArgumentException("Right bound can't be smaller than left bound");
            }
            if (step <= 0)
            {
                throw new ArgumentOutOfRangeException("Step should be positive");
            }
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException("Amount should be positive");
            }
            if (!OutputStream.CanWrite)
            {
                throw new IOException("Can't write to specified stream");
            }

            var modelData = new Dictionary<double, long>();
            TotalData = 0;
            for (double x = leftBound; x <= rightBound; x += step)
            {
                modelData[x] = (long)(DistributionFunction(x) * amount);
                TotalData += modelData[x];
            }

            using (StreamWriter outputStreamWriter = new StreamWriter(OutputStream))
            {
                foreach (KeyValuePair<double, long> valueAmountPair in modelData)
                {
                    for (int i = 0; i < valueAmountPair.Value; i++)
                    {
                        outputStreamWriter.WriteLine(valueAmountPair.Key);
                    }
                }
            }
        }

        public ProbabilityModelGenerator(DistributionFunctionCallback distributionFunction)
        {
            DistributionFunction = distributionFunction;
            OutputStream = Console.OpenStandardOutput();
        }

        public ProbabilityModelGenerator(DistributionFunctionCallback distributionFunction, Stream outputStream)
        {
            DistributionFunction = distributionFunction;
            OutputStream = outputStream;
        }
    }
}
