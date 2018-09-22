using System;
using System.IO;
using System.Reflection;
using MathNet.Numerics.Integration;

namespace ProbabilityModelGenerator.Example
{
    class Example
    {
        private static double AlphaDensity(double x)
        {
            const long a = 4, b = 20;

            return (b / (Math.Pow(x, 2) * Math.Sqrt(2 * Math.PI)))
                * Math.Pow(Math.E, -0.5 * Math.Pow(b / x - a, 2));
        }

        private static double AlphaDistribution(double x)
        {
            return NewtonCotesTrapeziumRule.IntegrateAdaptive(AlphaDensity, 0, x, x / 100);
        }

        static void Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("usage: {0} <left bound> <right bound> <step> <maximum amount> [<output file name>]",
                    Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
                return;
            }

            double leftBound, rightBound, step;
            long amount;
            if (!double.TryParse(args[0], out leftBound))
            {
                Console.WriteLine("Left bound should be floating point value");
                return;
            }
            if (!double.TryParse(args[1], out rightBound))
            {
                Console.WriteLine("Right bound should be floating point value");
                return;
            }
            if (!double.TryParse(args[2], out step))
            {
                Console.WriteLine("Step should be floating point value");
                return;
            }
            if (!long.TryParse(args[0], out amount))
            {
                Console.WriteLine("Amount should be integer number");
                return;
            }

            ProbabilityModelGenerator generator;
            if (args.Length < 5)
            {
                generator = new ProbabilityModelGenerator(AlphaDistribution);
            }
            else
            {
                generator = new ProbabilityModelGenerator(AlphaDistribution, new FileStream(args[4], FileMode.OpenOrCreate, FileAccess.Write));
            }
            generator.Generate(leftBound, rightBound, step, amount, true);
        }
    }
}
