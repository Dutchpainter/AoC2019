using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dag_01
{
    class Program
    {
        static void Main(string[] args)
        {
            // Inlezen bestand
            var mass = File
                   .ReadAllLines("Input.txt")
                   .Where(s => !string.IsNullOrWhiteSpace(s))
                   .ToList(); 
            var fuel = mass.Select(x => int.Parse(x))
                .Select(x => (Math.Floor((double)x / 3) - 2))
                .ToList()
                .Aggregate(0,(a,b) => (int)(a + b));

            Console.WriteLine($"Het antwoord op opdracht 1: {fuel}");
            var allFuel = new List<int>();
            foreach(var module in mass)
            {
                Console.WriteLine("Adding Fuel");
                
                var fuelToAdd = Math.Floor(double.Parse(module) / 3) - 2;
                while (fuelToAdd >= 0)
                {
                    allFuel.Add((int)fuelToAdd);
                    fuelToAdd = Math.Floor(fuelToAdd/ 3) - 2;   
                }
            }
            var Oplossing2 = allFuel.Aggregate(0, (a, b) => (int)(a + b));
            Console.WriteLine($"Het antwoord op opdracht 2: {Oplossing2} ");
            Console.WriteLine("De opdracht is klaar");
        }
    }
}
