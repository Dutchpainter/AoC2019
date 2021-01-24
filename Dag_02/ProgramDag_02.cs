using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dag_02
{
    class Program
    {
        static void Main(string[] args)
        {
            using var file = new FileStream("Input.txt", FileMode.Open);
            using var reader = new StreamReader(file);
            // Inlezen informatie
            var input= reader.ReadLine();
            var software = new IntCode(input);
            software.Restore1202alarm();
            software.RunCode();
            var oplossing1 = software.OpCode[0];
            Console.WriteLine($"De oplossing op vraag 1 is: {oplossing1}");
            software = new IntCode(input);
            software.Restore1202alarm();
            software.GetNounVerb();
            var oplossing2 = software.OpCode[1]* 100 + software.OpCode[2];
            Console.WriteLine($"De oplossing op vraag 2 is: {oplossing2}");
            Console.WriteLine("De vraag is opgelost");
        }
    }

    public class IntCode
    {
        public List<int> OpCode { get; set; }
        public int Pointer { get; set; }
        public int Noun { get; set; }
        public int Verb { get; set; }
        public string Code { get; set; }
        public IntCode (string code)
        {
            OpCode = new List<int>(code.Split(',').Select(x => int.Parse(x)));
            Pointer = 0;
            Noun = 0;
            Verb = 0;
            Code = code;
        }
        public bool GetNounVerb()
        {
            for (var i = 0; i<=99; i++)
            {
                for (var j=0; j<=99; j++)
                {
                    RestoreMemory();
                    Restore1202alarm();
                    OpCode[1] = i; // Noun
                    OpCode[2] = j; //Verb
                    RunCode();
                    if (OpCode[0] == 19690720)
                    {
                        Noun = i;
                        Verb = j;
                        Console.WriteLine("Found it");
                        return true;
                    }
                    Console.WriteLine("Sorry, didn't find it...");
                }
            }
            return true;
        }
        public bool RestoreMemory()
        {
            OpCode.Clear();
            OpCode.AddRange(Code.Split(',').Select(x => int.Parse(x)).ToList());
            Pointer = 0;
            Noun = 0;
            Verb = 0;
            return true;
        }
        public bool Restore1202alarm()
        {
            OpCode[1] = 12;
            OpCode[2] = 2;
            return true;
        }
        public bool RunCode()
        {
            int firstNumber;
            int secondNumber;
            var opCode = OpCode[Pointer];
            while (opCode != 99)
            {
                switch (OpCode[Pointer])
                {
                    case 1:
                        // sum
                        Pointer++;
                        firstNumber = OpCode[OpCode[Pointer]];
                        Pointer++;
                        secondNumber = OpCode[OpCode[Pointer]];
                        Pointer++;
                        OpCode[OpCode[Pointer]] = firstNumber + secondNumber;
                        Pointer++;
                        opCode = OpCode[Pointer];
                        break;
                    case 2:
                        // multiply
                        Pointer++;
                        firstNumber = OpCode[OpCode[Pointer]]; 
                        Pointer++;
                        secondNumber = OpCode[OpCode[Pointer]]; 
                        Pointer++;
                        OpCode[OpCode[Pointer]] = firstNumber * secondNumber;
                        Pointer++;
                        opCode = OpCode[Pointer];
                        break;
                    case 99:
                        Console.WriteLine("The Program is finished");
                        // end program, but you'r not supposed to reach this point
                        break;
                    default:
                        // you'r not supposed to reach this point
                        Console.WriteLine("Oops... something went wrong");
                        break;
                }
            }
            return true;
        }
    }

}
