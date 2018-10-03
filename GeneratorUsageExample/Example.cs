using System;
using System.IO;
using System.Reflection;

namespace ProbabilityModelGenerator.Example
{
    class Example
    {
        public delegate double IntegratableFunctionCallback(double x);

        private static double SimpsonsIntegrate(IntegratableFunctionCallback functionCallback, double leftBound, double rightBound, uint segments)
        {
            if (segments == 0)
            {
                throw new ArgumentException("Segments should be positive number");
            }
            if (segments % 2 == 1)
            {
                throw new ArgumentException("Segments should be even");
            }
            if (rightBound < leftBound)
            {
                throw new ArgumentException("Right bound should be above left bound");
            }

            double sum = 0;
            double step = (rightBound - leftBound) / segments;
            double x = leftBound + step;

            while (x < rightBound)
            {
                sum += 4 * functionCallback(x);
                x += step;
                if (x < rightBound)
                {
                    sum += 2 * functionCallback(x);
                    x += step;
                }
            }

            return (step / 3) * (sum + functionCallback(leftBound) + functionCallback(rightBound));
        }

        private static double AlphaDensity(double x)
        {
            const double a = 1, b = 1;

            return (b / (Math.Pow(x, 2) * Math.Sqrt(2 * Math.PI)))
                * Math.Exp(-0.5 * Math.Pow(b / x - a, 2));
        }

        private static double AlphaDistribution(double x)
        {
            const int segments = 10000;
            const double leftBound = 0.001;

            return SimpsonsIntegrate(AlphaDensity, leftBound, x, segments);
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
            if (!long.TryParse(args[3], out amount))
            {
                Console.WriteLine("Amount should be integer number");
                return;
            }

            ProbabilityModelGenerator generator;
            if (args.Length < 5)
            {
                generator = new ProbabilityModelGenerator(AlphaDistribution);
                generator.Generate(leftBound, rightBound, step, amount, true);
            }
            else
            {
                generator = new ProbabilityModelGenerator(AlphaDistribution, new FileStream(args[4], FileMode.OpenOrCreate, FileAccess.Write));
                generator.Generate(leftBound, rightBound, step, amount, false);
            }

            Console.WriteLine("Done.");
            Console.ReadKey();
        }
    }
}
