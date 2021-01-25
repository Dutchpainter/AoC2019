using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dag_03
{
    class Program
    {
        static void Main(string[] args)
        {
            using var file = new FileStream("Input.txt", FileMode.Open);
            using var reader = new StreamReader(file);
            // Inlezen informatie
            var input = reader.ReadLine();
            var firstWire = new Wire(input);

            input = reader.ReadLine();
            var secondWire = new Wire(input);

            var crossings = firstWire.GetCrossing(secondWire);
            var oplossing1 = crossings.Select(x => Math.Abs(x.Xpos) + Math.Abs(x.Ypos)).ToList().Min();
            Console.WriteLine($"Vraag 1: {oplossing1}");

            var oplossing2 = crossings.Select(x=> x.Step1+x.Step2).ToList().Min();
            Console.WriteLine($"Vraag 2: {oplossing2}");

            Console.WriteLine("De vraag is opgelost");
        }
    }
    public record Wire
    {
        public List<string> Path { get; }
        public Dictionary <(int, int), int > Points { get; }
        public Wire(string input)
        {
            Path = new List<string>(input.Split(','));
            Points = new Dictionary<(int, int), int>();
            GetPoints();
        }
        bool GetPoints()
        {
            var posX = 0;
            var posY = 0;
            var steps = 0;
            foreach (var move in Path){
                var direction = move.First();
                var length = int.Parse(move[1..]);
                (int posX, int posY) point;
                switch (direction)
                {
                    case 'U':
                        for (var i = 1; i <= length; i++)
                        {
                            steps = steps + 1;
                            posY += 1;
                            point = (posX, posY);
                            if (!Points.ContainsKey(point))
                            {
                                Points.Add(point,steps);
                            }
                        }
                        break;
                    case 'D':
                        for (var i = 1; i <= length; i++)
                        {
                            steps = steps + 1;
                            posY -= 1;
                            point = (posX, posY);
                            if (!Points.ContainsKey(point))
                            {
                                Points.Add(point, steps);
                            }
                        }
                        break;
                    case 'L':
                        for (var i = 1; i <= length; i++)
                        {
                            steps = steps + 1;
                            posX -= 1;
                            point = (posX, posY);
                            if (!Points.ContainsKey(point))
                            {
                                Points.Add(point, steps);
                            }
                        }
                        break;
                    case 'R':
                        for (var i = 1; i <= length; i++)
                        {
                            steps = steps + 1;
                            posX += 1;
                            point = (posX, posY);
                            if (!Points.ContainsKey(point))
                            {
                                Points.Add(point, steps);
                            }
                        }
                        break;
                    default:
                        Console.WriteLine("Oops, something went wrong");
                        break;
                }
            }
            return true;
        }
        public List<(int Xpos,int Ypos, int Step1, int Step2)> GetCrossing(Wire wire)
        {
            var crossings = new List<(int, int, int, int)>();
            foreach (var key in wire.Points.Keys)
            {
                if (Points.ContainsKey(key))
                {
                    crossings.Add((key.Item1, key.Item2, wire.Points[key], Points[key]));
                }
            }
            return crossings;
        }
    }
}
