using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter the directory containing the saves:");
            string path = Console.ReadLine();

            if (!Directory.Exists(path))
            {
                Console.WriteLine("Path does not exist. Press any key to exit.");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("Reading...");
            string[] filenames = Directory.GetFiles(path).OrderBy(s => int.Parse(Path.GetFileNameWithoutExtension(s).Substring(3))).ToArray();
            Generation[] generations = filenames.Select(f => new Generation(File.ReadAllLines(f))).ToArray();

            

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Found " + generations.Length + " generations in " + Path.GetFileName(path) + ".");
                Console.WriteLine("What do you want to do?");
                Console.WriteLine("0: Exit");
                Console.WriteLine("1: Print info for every generation");
                Console.WriteLine("2: Save best fitnesses (with runtimes) as CSV");
                Console.WriteLine();

                switch (Console.ReadKey(true).KeyChar)
                {
                    case '1':
                        PrintInfo(generations);
                        break;

                    case '2':
                        SaveBestFitnesses(generations);
                        break;

                    case '0':
                        return;

                    default:
                        Console.WriteLine("Invalid input. Press any key to try again.");
                        Console.ReadKey();
                        break;

                }
            }
        }

        private static void PrintInfo(Generation[] gens)
        {
            foreach (Generation g in gens)
            {
                Console.WriteLine("GENERATION " + (g.Number + 1));
                Console.WriteLine("Best fitness: " + g.MaxFitness + " (" + g.MaxFitnessTime.ToReadableString() + ")");
                Console.WriteLine("Average fitness: " + g.AvgFitness + " (" + g.AvgTime.ToReadableString() + ")");
                Console.WriteLine(new string('=', 33));
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to return to the menu.");
            Console.ReadKey(true);
        }

        private static void SaveBestFitnesses(Generation[] gens)
        {
            Console.WriteLine("Please enter your desired filename: ");
            string name = Console.ReadLine();

            try
            {
                using (var writer = File.CreateText(name))
                {
                    writer.WriteLine("GenerationNumber;MaxFitness;MaxFitnessTime");
                    foreach (Generation g in gens)
                    {
                        writer.WriteLine((g.Number + 1) + ";" + g.MaxFitness + ";" + g.MaxFitnessTime.TotalSeconds);
                    }
                }

                Console.WriteLine("Successfully saved file as \"" + name + "\".");
            }
            catch
            {
                Console.WriteLine("Something went wrong. Please try again.");
            }

            Console.WriteLine("Press any key to return to the menu.");
            Console.ReadKey(true);
        }
    }
}
