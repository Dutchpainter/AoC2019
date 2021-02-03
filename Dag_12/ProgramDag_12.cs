using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Numerics;

namespace Dag_12
{
    class Program
    {
        static void Main(string[] args)
        {
            var moonPositions = File
                 .ReadAllLines("Input.txt")
                 .Where(s => !string.IsNullOrWhiteSpace(s))
                 .ToList();

            var moons = new Moons(moonPositions);
            //moons.PrintMoons();
            moons.StepThroughTime(-1);
            moons.CalculateRepeat();
            Console.WriteLine();
            Console.WriteLine("De opdracht is afgerond.");
        }
    }
    public class Moons
    {
        int Time { get; set; }
        int[] PositionsArray { get; set; }
        int[] VelocitiesArray { get; set; }
        int NumberOfMoons { get; }
        int [] InitPosArray { get; set; }
        List <int> RepeatStepAxis { get; set; }
        bool XIsZero { get; set; }
        bool YIsZero { get; set; }
        bool ZIsZero { get; set; }
        public Moons(List<string> positions)
        {
            Time = 0;
            NumberOfMoons = 4;
            PositionsArray = new int[12];
            InitPosArray = new int[12];
            VelocitiesArray = new int[12];
            RepeatStepAxis = new List<int>();
            InitPositions(positions);
        }
        public bool CalculateRepeat()
        {
            var bigX = new BigInteger(RepeatStepAxis[0]);
            var bigY = new BigInteger(RepeatStepAxis[1]);
            var bigZ = new BigInteger(RepeatStepAxis[2]);
            var lcmYZ = Lcm(bigY, bigZ);
            var lcmXYZ = Lcm(bigX, lcmYZ);
            Console.WriteLine($"Het universum herhaalt zich na {lcmXYZ} stappen");
            return true;
        }
        public static BigInteger Lcm(BigInteger a, BigInteger b)
        {
            return (a * b) / BigInteger.GreatestCommonDivisor(a, b);
        }
        public int CalculateTotalEnergy()
        {
            var potEnergy = new int[4];
            var kinEnergy = new int[4];
            var totalEnergy = 0;
            for (var i = 0; i<NumberOfMoons; i++)
            {
                for (var j = 0; j< 3; j++)
                {
                    potEnergy[i] += Math.Abs(PositionsArray[i * 3 + j]);
                    kinEnergy[i] += Math.Abs(VelocitiesArray[i * 3 + j]);
                }
            }
            for (var i = 0; i< NumberOfMoons; i++)
            {
                totalEnergy += potEnergy[i] * kinEnergy[i];
            }
            return totalEnergy;
        }
        public bool InitPositions(List<string> inputStrings)
        {
            for (var i = 0; i < inputStrings.Count; i++)
            {
                var inputValues = inputStrings[i].Split(',');
                PositionsArray[i * 3 + 0] = int.Parse(inputValues[0][3..]);
                PositionsArray[i * 3 + 1] = int.Parse(inputValues[1][3..]);
                PositionsArray[i * 3 + 2] = int.Parse(inputValues[2][3..^1]);
            }
            for (var i = 0; i < 12; i++)
            {
                InitPosArray[i] = PositionsArray[i];
            }
            for (var i = 0; i < 3; i++)
            {
                RepeatStepAxis.Add(0);
            }
            XIsZero = false;
            YIsZero = false;
            ZIsZero = false;
            return true;
        }
        bool UpdateSpeed()
        {
            for (var i = 0; i < 12; i++)
            {
                PositionsArray[i] += VelocitiesArray[i];
            }
            return true;
        }
        bool ApplyGravity()
        {
            var gravity = new int[12];
            for (var moon = 1; moon < NumberOfMoons; moon++)
            {
                for (var pos = 0; pos < 3; pos++)
                {
                    gravity[0 + pos] += (PositionsArray[(moon * 3 + 0 + pos) % 12] == PositionsArray[0 + pos]) ? 0 : (PositionsArray[(moon * 3 + 0 + pos) % 12] > PositionsArray[0 + pos]) ? +1 : -1;
                    gravity[3 + pos] += (PositionsArray[(moon * 3 + 3 + pos) % 12] == PositionsArray[3 + pos]) ? 0 : (PositionsArray[(moon * 3 + 3 + pos) % 12] > PositionsArray[3 + pos]) ? +1 : -1;
                    gravity[6 + pos] += (PositionsArray[(moon * 3 + 6 + pos) % 12] == PositionsArray[6 + pos]) ? 0 : (PositionsArray[(moon * 3 + 6 + pos) % 12] > PositionsArray[6 + pos]) ? +1 : -1;
                    gravity[9 + pos] += (PositionsArray[(moon * 3 + 9 + pos) % 12] == PositionsArray[9 + pos]) ? 0 : (PositionsArray[(moon * 3 + 9 + pos) % 12] > PositionsArray[9 + pos]) ? +1 : -1;
                }
            }
            for (var i = 0; i< 12; i++)
            {
                VelocitiesArray[i] += gravity[i];
            }
            return true;
        }
        bool CheckRepeat()
        {
            var allAreZero = true;
            // X-as
            for(var i = 0; i < 12; i += 3)
            {
                if (VelocitiesArray[i] != 0) allAreZero = false;
            }
            if ( allAreZero && !XIsZero)
            {
                for (var i = 0; i < 12; i+=3)
                {
                    if (PositionsArray[i] != InitPosArray[i]) allAreZero = false;
                }
                if (allAreZero)
                {
                    XIsZero = true;
                    RepeatStepAxis[0] = Time;
                    Console.WriteLine($"X-axis value found: {Time}");
                }
            }
            // Y-as
            allAreZero = true;
            for (var i = 0; i < 12; i += 3)
            {
                if (VelocitiesArray[i + 1] != 0) allAreZero = false;
            }
            if (allAreZero && !YIsZero)
            {
                for (var i = 0; i < 12; i += 3)
                {
                    if (PositionsArray[i+1] != InitPosArray[i+1]) allAreZero = false;
                }
                if (allAreZero)
                {
                    YIsZero = true;
                    RepeatStepAxis[1] = Time;
                    Console.WriteLine($"Y-axis value found: {Time}");
                }
            }
            // Z-as
            allAreZero = true;
            for (var i = 0; i < 12; i += 3)
            {
                if (VelocitiesArray[i + 2] != 0) allAreZero = false;
            }
            if (allAreZero && !ZIsZero)
            {
                for (var i = 0; i < 12; i += 3)
                {
                    if (PositionsArray[i+2] != InitPosArray[i+2]) allAreZero = false;
                }
                if (allAreZero)
                {
                    ZIsZero = true;
                    RepeatStepAxis[2] = Time;
                    Console.WriteLine($"Z-axis value found: {Time}");
                }
            }
            if (!YIsZero) allAreZero = false;
            if (!XIsZero) allAreZero = false;
            return allAreZero;
        }
        public List<string> MakeStrings(int[] values)
        {
            var newValues = new List<string>();
            for (var i = 0; i < NumberOfMoons; i++)
            {
                newValues.Add("<x=" + values[i * 3 + 0].ToString()
                + ", y=" + values[i * 3 + 1].ToString()
                + ", z=" + values[i * 3 + 2].ToString() + ">");
            }
            return newValues;
        }
        public bool PrintMoons()
        {
            Console.WriteLine($"After {Time} steps ");
            var energy = CalculateTotalEnergy();
            var speeds = MakeStrings(VelocitiesArray);
            var pos = MakeStrings(PositionsArray);
            for (var i = 0; i < NumberOfMoons; i++)
            {
                Console.WriteLine($"{pos[i]} {speeds[i]}");
            }
            Console.WriteLine($"Total Energy {energy}");
            return true;
        }
        public int StepThroughTime(int times = -1)
        {
            while (Time < times || times == -1)
            {
                ApplyGravity();
                UpdateSpeed();
                Time++;
                if (CheckRepeat()) return Time + 1;
            }
            return Time;
        }
    }
}
