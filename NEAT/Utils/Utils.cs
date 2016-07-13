using NEAT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.Genetics
{
    public static class Utils
    {
        private static FastRandom rnd = new FastRandom();

        //Function taken from SharpNeat
        public static double ProbabilisticRound(double val)
        {
            double integerPart = Math.Floor(val);
            double fractionalPart = val - integerPart;
            return rnd.NextDouble() < fractionalPart ? integerPart + 1.0 : integerPart;
        }

        //Based on rouletteWheel from SharpNeat
        public static int RandomSelectIndex(double[] weights)
        {
            double sum = weights.Sum();
            if (sum == 0)
            {
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i] = 1;
                }
                sum = weights.Length;
            }
            double selected = sum * rnd.NextDouble();
            double iterator = 0D;

            for (int i = 0; i < weights.Length; i++)
            {
                iterator += weights[i];
                if (selected < iterator)
                    return i;
            }

            for (int i = 0; i < weights.Length; i++)
            {
                if (weights[i] != 0)
                    return i;
            }

            throw new InvalidOperationException("All weights are zero.");
        }
        public static int RandomSelectIndex(int[] weights)
        {
            double sum = weights.Sum();
            double selected = sum * rnd.NextDouble();
            double iterator = 0D;

            for (int i = 0; i < weights.Length; i++)
            {
                iterator += weights[i];
                if (iterator < selected)
                    return i;
            }

            for (int i = 0; i < weights.Length; i++)
            {
                if (weights[i] != 0)
                    return i;
            }

            throw new InvalidOperationException("All weights are zero.");
        }

        public static int RandomInt(int upperBound)
        {
            return rnd.Next(upperBound);
        }
    }
}
