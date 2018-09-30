using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

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

        public void Generate(double leftBound, double rightBound, double step, long amount, bool shouldOutputTotalNumber = false)
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
            for (double x = leftBound + step / 2; (x + step / 2) <= rightBound; x += step)
            {
                modelData[x] = (long)((DistributionFunction(x + step / 2) - DistributionFunction(x - step / 2)) * amount);
                TotalData += modelData[x];
            }

            NumberFormatInfo numberFormatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };
            using (StreamWriter outputStreamWriter = new StreamWriter(OutputStream))
            {
                foreach (KeyValuePair<double, long> valueAmountPair in modelData)
                {
                    for (int i = 0; i < valueAmountPair.Value; i++)
                    {
                        outputStreamWriter.WriteLine(valueAmountPair.Key.ToString(numberFormatInfo));
                    }
                }

                if (shouldOutputTotalNumber)
                {
                    outputStreamWriter.WriteLine();
                    outputStreamWriter.WriteLine("Total amount of data: {0}", TotalData);
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
