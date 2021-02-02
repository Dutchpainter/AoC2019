using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Numerics;


namespace Dag_10
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File
                 .ReadAllLines("Input.txt")
                 .Where(s => !string.IsNullOrWhiteSpace(s))
                 .ToList();

            var asteroidMap = new AsteroidMap(lines);
            var maxVisibleAsteroids = asteroidMap.VisibleAsteroids.Select(x => x.number).Max();
            var xPos = asteroidMap.VisibleAsteroids.Where(x => x.number == maxVisibleAsteroids).SingleOrDefault().xpos;
            var yPos = asteroidMap.VisibleAsteroids.Where(x => x.number == maxVisibleAsteroids).SingleOrDefault().ypos;
            Console.WriteLine($"Opdracht1: op positie x: {xPos} en y: {yPos} zijn {maxVisibleAsteroids} asteroiden zichtbaar");

            var vap200 = asteroidMap.CountAsteroids(xPos,yPos)[199].xpos*100 + asteroidMap.CountAsteroids(xPos, yPos)[199].ypos;
            Console.WriteLine($"Opdracht2: {vap200}");
            Console.WriteLine("De opdracht is afgerond");
        }
    }
    public class AsteroidMap
    {
        char[,] Map { get; }
        List<(int xpos, int ypos)> ListOfAsteroids { get; }
        public List<(int xpos, int ypos, int number)> VisibleAsteroids { get; }
        public AsteroidMap(List<string> input)
        {
            ListOfAsteroids = new List<(int, int)>();
            VisibleAsteroids = new List<(int, int, int)>();
            Map = new char[input[0].Length, input.Count];
            for (var h = 0; h < input.Count; h++)
            {
                for (var w = 0; w < input[0].Length; w++)
                {
                    Map[w, h] = (input[h][w]);
                    if (input[h][w] == '#') ListOfAsteroids.Add((w,h));
                }
            }
            CountVisibleAsteroids();
        }
        public bool CountVisibleAsteroids()
        {
            foreach (var asteroid in ListOfAsteroids)
            {
                VisibleAsteroids.Add((asteroid.xpos, asteroid.ypos, CountAsteroids(asteroid.xpos, asteroid.ypos).Count));
            }
            return true;
        }
        public List<(int angle, int radius, int xpos, int ypos)> CountAsteroids(int x, int y )
        {
            var allAsteroids = new List<(int angle, int radius, int xpos, int ypos)>(ListOfAsteroids
                .Where(a => !(a.xpos == x && a.ypos == y))
                .Select(a => ((9000 - ((int) (100 * Math.Round(Math.Atan2(a.xpos - x, a.ypos - y ) * (180 / Math.PI) , 2)))) % 36000 , (int) (Math.Round(Math.Pow(a.xpos - x,2) + Math.Pow(a.ypos - y,2),0)), a.xpos, a.ypos))
                .ToList());
            allAsteroids.Sort();

            var visible = new List<(int angle, int radius, int xpos, int ypos)>();
            visible.Add(allAsteroids[0]);
            for (var i = 1; i < allAsteroids.Count; i++)
            {
                if (allAsteroids[i - 1].angle != allAsteroids[i].angle)
                {
                    visible.Add(allAsteroids[i]);
                }
            }
            return visible;
        }
    }
}
