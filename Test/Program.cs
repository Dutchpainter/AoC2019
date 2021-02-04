using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dag_12
{
    class Program
    {
        static void Main(string[] args)
        {
            var moonPositions = File
                 .ReadAllLines("Inputtest.txt")
                 .Where(s => !string.IsNullOrWhiteSpace(s))
                 .ToList();

            var moons = new Moons(moonPositions);
            moons.PrintMoons();
            moons.StepThroughTime(-1);
            moons.PrintMoons();
            Console.WriteLine();
            Console.WriteLine("De opdracht is afgerond.");
        }
    }
    public class Moons
    {
        public List<(string Name, Moon PosSpeed)> ListOfMoons { get; }
        List<string> ListOfNames { get; }
        int Time { get; set; }
        public int TotalEnergy { get; set; }
        public ((int Potential, int Kinetic) Io,
            (int Potential, int Kinetic) Europe,
            (int Potential, int Kinetic) Ganymede,
            (int Potential, int Kinetic) Callisto)
            ZeroState
        { get; set; }
        public List<string> Positions { get; }
        public Moons(List<string> positions)
        {
            Positions = new List<string>(positions);
            ListOfNames = new List<string> { "Io", "Europe", "Ganymede", "Callisto" };
            ListOfMoons = new List<(string, Moon)>();
            for (var i = 0; i < positions.Count; i++)
            {
                ListOfMoons.Add((ListOfNames[i], new Moon(positions[i])));
            }
            Time = 0;
            CalculateTotalEnergy();
            DefineZeroState();
        }
        bool CheckRepeat()
        {
            var checkstring = "<x=" + ListOfMoons[0].PosSpeed.XPos.ToString()
                + ", y=" + ListOfMoons[0].PosSpeed.YPos.ToString()
                + ", z=" + ListOfMoons[0].PosSpeed.ZPos.ToString() + ">";
            if (checkstring == Positions[0])
            {
                CalculateTotalEnergy();
                Console.WriteLine($"TotalEnergy = 0, time: {Time + 1}");
                var energyOverview = new List<(int, int)>();
                for (var i = 0; i < ListOfNames.Count; i++)
                {
                    energyOverview.Add(ListOfMoons.Where(x => x.Name == ListOfNames[i])
                        .Select(x => (x.PosSpeed.PotentialEnergy, x.PosSpeed.KineticEnergy)).SingleOrDefault());
                }
                if (ZeroState == (energyOverview[0], energyOverview[1], energyOverview[2], energyOverview[3]))
                {
                    return true;
                }
            }
            return false;
        }

        bool DefineZeroState()
        {
            if (TotalEnergy == 0)
            {
                var energyOverview = new List<(int, int)>();
                for (var i = 0; i < ListOfNames.Count; i++)
                {
                    energyOverview.Add(ListOfMoons.Where(x => x.Name == ListOfNames[i])
                        .Select(x => (x.PosSpeed.PotentialEnergy, x.PosSpeed.KineticEnergy)).SingleOrDefault());
                }
                ZeroState = (energyOverview[0], energyOverview[1], energyOverview[2], energyOverview[3]);
            }
            return true;
        }
        public bool CalculateTotalEnergy()
        {
            TotalEnergy = 0;
            foreach (var moon in ListOfMoons)
            {
                moon.PosSpeed.CalculateEnergy();
                TotalEnergy += moon.PosSpeed.PotentialEnergy * moon.PosSpeed.KineticEnergy;
            }
            return true;
        }
        public bool PrintMoons()
        {
            Console.WriteLine($"After {Time} steps ");
            CalculateTotalEnergy();
            foreach (var moon in ListOfMoons)
            {
                Console.WriteLine($"pos =< x = {moon.PosSpeed.XPos}, y = {moon.PosSpeed.YPos}, z = {moon.PosSpeed.ZPos} >, vel =< x = {moon.PosSpeed.XSpeed}, y = {moon.PosSpeed.YSpeed}, z = {moon.PosSpeed.ZSpeed} > ");
            }
            Console.WriteLine($"Total Energy {TotalEnergy}");
            return true;
        }
        public int StepThroughTime(int times = -1)
        {
            var count = 0;
            while (Time < times || times == -1)
            {
                ApplyGravity();
                UpdateSpeed();
                Time++;
                if (CheckRepeat()) return Time;
                count++;
                if (count / 1000000 == 1)
                {
                    Console.WriteLine(Time);
                    count = 0;
                }
            }
            return Time;
        }
        bool ApplyGravity()
        {
            var gravity = new List<(string name, (int xSpeed, int ySpeed, int zSpeed) speeds)>();
            foreach (var moon in ListOfMoons)
            {
                var posSpeeds = new List<(int, int, int)>(ListOfMoons
                    .Where(m => m.Name != moon.Name).Select(m => m.PosSpeed)
                    .Select(x => (x.XPos - moon.PosSpeed.XPos, x.YPos - moon.PosSpeed.YPos, x.ZPos - moon.PosSpeed.ZPos)))
                    .Select(x => ((x.Item1 == 0) ? 0 : (x.Item1 / Math.Abs(x.Item1)), (x.Item2 == 0) ? 0 : (x.Item2 / Math.Abs(x.Item2)), (x.Item3 == 0) ? 0 : (x.Item3 / Math.Abs(x.Item3))))
                    .ToList()
                    .Aggregate((0, 0, 0), (a, b) => (a.Item1 + b.Item1, a.Item2 + b.Item2, a.Item3 + b.Item3));
                gravity.Add((moon.Name, posSpeeds));
            }
            foreach (var moon in ListOfMoons)
            {
                moon.PosSpeed.XSpeed += gravity.Where(x => x.name == moon.Name).SingleOrDefault().speeds.xSpeed;
                moon.PosSpeed.YSpeed += gravity.Where(x => x.name == moon.Name).SingleOrDefault().speeds.ySpeed;
                moon.PosSpeed.ZSpeed += gravity.Where(x => x.name == moon.Name).SingleOrDefault().speeds.zSpeed;
            }
            return true;
        }
        bool UpdateSpeed()
        {
            foreach (var moon in ListOfMoons)
            {
                moon.PosSpeed.XPos += moon.PosSpeed.XSpeed;
                moon.PosSpeed.YPos += moon.PosSpeed.YSpeed;
                moon.PosSpeed.ZPos += moon.PosSpeed.ZSpeed;
            }
            return true;
        }
    }

    public class Moon
    {
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int ZPos { get; set; }
        public int XSpeed { get; set; }
        public int YSpeed { get; set; }
        public int ZSpeed { get; set; }
        public int PotentialEnergy { get; set; }
        public int KineticEnergy { get; set; }
        public Moon(string positions)
        {
            var stringPositions = positions.Split(',');
            XSpeed = 0;
            YSpeed = 0;
            ZSpeed = 0;
            XPos = int.Parse(stringPositions[0].Substring(3));
            YPos = int.Parse(stringPositions[1].Substring(3));
            ZPos = int.Parse(stringPositions[2].Substring(3, stringPositions[2].Length - 4));
            CalculateEnergy();
        }
        public bool CalculateEnergy()
        {
            PotentialEnergy = Math.Abs(XPos) + Math.Abs(YPos) + Math.Abs(ZPos);
            KineticEnergy = Math.Abs(XSpeed) + Math.Abs(YSpeed) + Math.Abs(ZSpeed);
            return true;
        }
    }
}
