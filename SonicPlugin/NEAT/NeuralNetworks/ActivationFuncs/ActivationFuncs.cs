using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.NeuralNetworks
{
    public static class ActivationFunctions
    {
        public static IActivationFunction<double> FromName(string name)
        {
            switch (name.ToLower())
            {
                case "sigmoid":
                    return new Sigmoid();

                case "evensigmoid":
                    return new EvenSigmoid();

                case "step":
                    return new StepFunction();

                default:
                    return null;
            }
        }

        public class Sigmoid : IActivationFunction<double>
        {
            public double Coefficient;

            public Sigmoid(double coefficient = 4.9)
            {
                this.Coefficient = coefficient;
            }

            public double GetValue(double x)
            {
                return (1D / (1D + Math.Exp(-Coefficient * x)));
            }

            public override string ToString()
            {
                return "sigmoid";
            }
        }

        public class StepFunction : IActivationFunction<double>
        {
            public double GetValue(double x)
            {
                return (x < 0) ? 0 : 1;
            }

            public override string ToString()
            {
                return "step";
            }
        }

        public class EvenSigmoid : NEAT.NeuralNetworks.IActivationFunction<double>
        {
            double a;
            double b;

            public EvenSigmoid(double range = 1)
            {
                a = range * 2;
                b = range;
            }

            public double GetValue(double x)
            {
                return (a / (Math.Exp(-4.9 * x) + 1)) - b;
            }

            public override string ToString()
            {
                return "evenSigmoid";
            }
        }
    }
}
