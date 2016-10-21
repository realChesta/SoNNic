using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analyzer
{
    public class Generation
    {
        public readonly int Number;
        public readonly int InnovationNumber;
        public readonly TimeSpan TotalTime;
        public readonly Subject[] Population;

        public int MaxFitness;
        public TimeSpan MaxFitnessTime;
        public int MinFitness;
        public int AvgFitness;
        public TimeSpan AvgTime;

        public Generation(int number, int innovation, Subject[] population)
        {
            this.Number = number;
            this.InnovationNumber = innovation;
            this.Population = population;

            this.CalcStats();
        }

        public Generation(int number, int innovation, string[] population)
        {
            this.Number = number;
            this.InnovationNumber = innovation;

            this.Population = new Subject[population.Length];
            for (int i = 0; i < population.Length; i++)
            {
                string[] subject = population[i].Split('/');
                this.Population[i] = new Subject(int.Parse(subject[0]), double.Parse(subject[1]), subject[2]);
            }

            this.CalcStats();
        }

        public Generation(string[] file)
        {
            this.Number = int.Parse(file[0]);
            this.InnovationNumber = int.Parse(file[1]);
            this.TotalTime = TimeSpan.FromSeconds(double.Parse(file[2]));

            this.Population = new Subject[file.Length - 3];
            for (int i = 0; i < this.Population.Length; i++)
            {
                string[] subject = file[i + 3].Split('/');
                this.Population[i] = new Subject(int.Parse(subject[0]), double.Parse(subject[1]), subject[2]);
            }

            this.CalcStats();
        }

        private void CalcStats()
        {
            var sorted = Population.OrderByDescending(s => s.Fitness);
            this.MaxFitness = sorted.First().Fitness;
            this.MaxFitnessTime = sorted.First().Runtime;
            this.MinFitness = sorted.Last().Fitness;
            this.AvgFitness = (int)sorted.Average(s => s.Fitness);
            this.AvgTime = TimeSpan.FromSeconds(sorted.Average(s => s.Runtime.TotalSeconds));
        }
    }
}
