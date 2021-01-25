using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dag_04
{
    class Program
    {
        static void Main(string[] args)
        {
            //367479-893698
            var start ="367479";
            var end ="893698";
            var results = new Passwords(start, end);
            var oplossing1 = results.Possibilities.Count;
            Console.WriteLine($"De oplossing van deel 1 is: {oplossing1}");
            var oplossing2 = results.Possibilities2.Count;
            // 149 te laag, 420 te hoog/ 435 te hoog
            Console.WriteLine($"De oplossing van deel 2 is: {oplossing2}");
            Console.WriteLine("De vraag is opgelost");
        }
    }
    public class Passwords
    {
        string Start { get; }
        string End { get; }
        List<int> Starts { get; }
        List<int> Ends { get; }
        public List<List<int>> Possibilities { get;}
        public List<List<int>> Possibilities2 { get; }
        public Passwords(string start, string end)
        {
            Start = start;
            End = end;
            Starts = new List<int>();
            Ends = new List<int>();
            for (var i = 0; i< Start.Length; i++)
            {
                Starts.Add(int.Parse(Start[i].ToString()));
                Ends.Add(int.Parse(End[i].ToString()));
            }
            Possibilities = new List<List<int>>();
            GetPossibilities();
            Possibilities2 = new List<List<int>>();
            GetPossibilities2();
        }
        bool GetPossibilities2()
        {
            foreach (var number in Possibilities)
            {
                bool ok = false;
                for (var i = 1; i < Starts.Count - 2; i++)
                {
                    if (number[i - 1] != number[i] && number[i] == number[i + 1] && number[i] != number[i + 2]) ok = true;
                }
                if (number[0] == number[1] && number[1] != number[2]) ok = true;
                if (number[3] != number[4] && number[4] == number[5]) ok = true;
                if (ok) Possibilities2.Add(number);
            }
            return true;
        }
        bool GetPossibilities()
        { 
            for (var i0= Starts[0]; i0 <= Ends[0]; i0++)
            {
                for (var i1 = i0; i1 <= 9; i1++)
                {
                    for (var i2 = i1; i2 <= 9; i2++)
                    {
                        for (var i3 = i2; i3 <= 9; i3++)
                        {
                            for (var i4 = i3; i4 <= 9; i4++)
                            {
                                for (var i5 = i4; i5 <= 9; i5++)
                                {
                                    if (i0 == i1 || i1 == i2 || i2 == i3 || i3 == i4 || i4 == i5)
                                    {
                                        var getal = i5 + 10 * i4 + 100 * i3 + 1000 * i2 + 10000 * i1 + 100000 * i0;
                                        if (getal >= int.Parse(Start) && getal <= int.Parse(End))
                                        {
                                            var option = new List<int>();
                                            option.Add(i0);
                                            option.Add(i1);
                                            option.Add(i2);
                                            option.Add(i3);
                                            option.Add(i4);
                                            option.Add(i5);
                                            Possibilities.Add(option);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}
