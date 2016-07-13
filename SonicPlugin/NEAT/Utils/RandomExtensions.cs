using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.Genetics
{
    public static class RandomExtensions
    {
        public static double NextDouble(this Random random, double minValue, double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }

        public static bool NextBool(this Random random)
        {
            return (random.Next(0, 2) == 1);
        }
        public static bool NextBool(this Random random, double chance)
        {
            return (random.NextDouble() < chance);
        }
    }
}
