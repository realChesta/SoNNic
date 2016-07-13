﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.NeuralNetworks
{
    public class NeuralNetwork : INeuralNetwork<double>
    {
        public INeuralInputNode<double>[] Inputs { get; set; }
        public INeuralOutputNode<double>[] Outputs { get; set; }
        public INeuralHiddenNode<double>[][] DeepNodes { get; set; }

        public INeuralNode<double>[] GetAllNodes()
        {
            INeuralNode<double>[] toReturn = new INeuralNode<double>[Inputs.Length + Outputs.Length + DeepNodes.Sum(l => l.Length)];
            Inputs.CopyTo(toReturn, 0);
            int copied = Inputs.Length;
            for (int i = 0; i < DeepNodes.Length; i++)
            {
                DeepNodes[i].CopyTo(toReturn, copied);
                copied += DeepNodes[i].Length;
            }
            Outputs.CopyTo(toReturn, copied);

            return toReturn;
        }

        public Dictionary<int, INeuralNode<double>> GetAllNodesWithNumbers()
        {
            Dictionary<int, INeuralNode<double>> toReturn = new Dictionary<int, INeuralNode<double>>();
            int count = Inputs.Length + Outputs.Length + DeepNodes.Sum(l => l.Length);

            for (int i = 0; i < Inputs.Length; i++)
            {
                toReturn.Add(Inputs[i].NodeNumber, Inputs[i]);
            }
            for (int i = 0; i < DeepNodes.Length; i++)
            {
                for (int j = 0; j < DeepNodes[i].Length; j++)
                {
                    toReturn.Add(DeepNodes[i][j].NodeNumber, DeepNodes[i][j]);
                }
            }
            for (int i = 0; i < Outputs.Length; i++)
            {
                toReturn.Add(Outputs[i].NodeNumber, Outputs[i]);
            }

            return toReturn;
        }

        public ISynapse<double>[] GetAllConnections()
        {
            List<ISynapse<double>> cons = new List<ISynapse<double>>();

            for (int i = 0; i < Inputs.Length; i++)
            {
                cons.AddRange(Inputs[i].Outputs);
            }
            for (int i = 0; i < DeepNodes.Length; i++)
            {
                for (int j = 0; j < DeepNodes[i].Length; j++)
                {
                    cons.AddRange(DeepNodes[i][j].Outputs);
                }
            }

            return cons.ToArray();
        }

        /// <summary>
        /// Creates a Neural Network with equal Deep Layer sizes.
        /// </summary>
        /// <param name="inputs">The amount of inputs.</param>
        /// <param name="outputs">The amount of outputs.</param>
        /// <param name="deepLayers"></param>
        /// <param name="layerSize"></param>
        public NeuralNetwork(int inputs, int outputs, int deepLayers, int layerSize)
        {
            Inputs = new InputNeuron[inputs];
            Outputs = new OutputNeuron[outputs];
            DeepNodes = new Neuron[deepLayers][];
            for (int i = 0; i < DeepNodes.Length; i++)
            {
                DeepNodes[i] = new Neuron[layerSize];
            }
        }
        /// <summary>
        /// Creates a Neural Network with varying layer sizes.
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        /// <param name="deepLayers"></param>
        /// <param name="layerSizes"></param>
        public NeuralNetwork(int inputs, int outputs, int deepLayers, int[] layerSizes)
        {
            Inputs = new InputNeuron[inputs];
            Outputs = new OutputNeuron[outputs];
            DeepNodes = new Neuron[deepLayers][];
            for (int i = 0; i < DeepNodes.Length; i++)
            {
                DeepNodes[i] = new Neuron[layerSizes[i]];
            }
        }

        public void CreateRandom(IActivationFunction<double> function)
        {
            FastRandom rnd = new FastRandom();
            int nodeNr = 0;

            //Create Neurons
            for (int i = 0; i < DeepNodes.Length; i++)
            {
                for (int j = 0; j < DeepNodes[i].Length; j++)
                {
                    DeepNodes[i][j] = new Neuron(nodeNr++, rnd.NextDouble() * 10D, function);
                }
            }
            for (int i = 0; i < Inputs.Length; i++)
            {
                Inputs[i] = new InputNeuron(nodeNr++);
                //Connect Input to first-layer-Neurons
                for (int j = 0; j < DeepNodes[0].Length; j++)
                {
                    Inputs[i].AddOutput(DeepNodes[0][j], rnd.NextDouble() * 5D);
                }
            }
            for (int i = 0; i < Outputs.Length; i++)
            {
                Outputs[i] = new OutputNeuron(nodeNr++, rnd.NextDouble() * 10D, function);
            }

            //Connect Neurons
            if (DeepNodes.Length > 0)
            {
                for (int i = 0; i < DeepNodes.Length; i++)
                {
                    if (i == (DeepNodes.Length - 1))
                    {
                        //Connect every last-layer-Neuron to every output
                        for (int j = 0; j < DeepNodes[i].Length; j++)
                        {
                            for (int k = 0; k < Outputs.Length; k++)
                            {
                                DeepNodes[i][j].AddOutput(Outputs[k], rnd.NextDouble() * 5D);
                            }
                        }
                        continue;
                    }

                    for (int j = 0; j < DeepNodes[i].Length; j++)
                    {
                        //Connect every Neuron to the layer after it
                        for (int k = 0; k < DeepNodes[i + 1].Length; k++)
                        {
                            DeepNodes[i][j].AddOutput(DeepNodes[i + 1][k], rnd.NextDouble() * 5D);
                        }
                    }
                }
            }
        }
    }
}