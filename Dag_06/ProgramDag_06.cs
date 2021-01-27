using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dag_06
{
    class Program
    {
        static void Main(string[] args)
        {
            var orbits = File
                 .ReadAllLines("Input.txt")
                 .Where(s => !string.IsNullOrWhiteSpace(s))
                 .ToList();

            var Orbits = new Orbits(orbits);
            Console.WriteLine($"het antwoord op vraag 1 is: {Orbits.OrbitCountChecksum}");

            var pathToSanta = Orbits.CalculatePathToSanta();
            Console.WriteLine($"het antwoord op vraag 2 is: {pathToSanta}");

            Console.WriteLine("De vraag is opgelost");
        }
    }
    public class Orbits
    {
        List<Orbit> ListOfOrbits { get; }
        Dictionary <string, string> DictOfOrbits { get;}
        public int DirectOrbits { get; set; }
        public int IndirectOrbits { get; set; }
        public int OrbitCountChecksum { get; }
        public Orbits(List<string> orbits)
        {
            ListOfOrbits = new List<Orbit>();
            DictOfOrbits = new Dictionary<string, string>();
            foreach (var orbit in orbits)
            {
                var newOrbit = new Orbit(orbit);
                ListOfOrbits.Add(newOrbit);
                DictOfOrbits.Add(newOrbit.Mover, newOrbit.Center);
            }
            CountOrbits();
            OrbitCountChecksum = DirectOrbits + IndirectOrbits;
        }
        public int CalculatePathToSanta()
        {
            var santasPath = CalculatePath("SAN");
            var yourPath = CalculatePath("YOU");
            foreach (var position in santasPath)
            {
                if (yourPath.ContainsKey(position.Key))
                {
                    return yourPath[position.Key] + position.Value;
                }
            }
            return 0;
        }
        Dictionary<string, int> CalculatePath(string origin)
        {
            var path = new Dictionary<string, int>();
            var mover = origin;
            var center = DictOfOrbits[mover];
            var orbits = 0;
            while (center != "COM")
            {
                path.Add(center, orbits);
                center = DictOfOrbits[center];
                orbits++;
            }
            return path;
        }
        int CountOrbits()
        {
            foreach (var orbit in ListOfOrbits)
            {
                var center = orbit.Center;
                var mover = orbit.Mover;
                DirectOrbits++;
                while (center != "COM")
                { 
                    center = DictOfOrbits[mover];
                    mover = center;
                    if (center != "COM") IndirectOrbits++;
                }
            }
            return 0;
        }
    }
    public record Orbit
    {
        public string Center { get; }
        public string Mover { get; }
        public Orbit(string orbit)
        {
            var combo = orbit.Split(')');
            Center = combo[0];
            Mover = combo[1];
        }
    }
}
