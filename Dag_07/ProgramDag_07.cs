using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Dag_07
{
    class Program
    {
        static void Main(string[] args)
        {
            using var file = new FileStream("Input.txt", FileMode.Open);
            using var reader = new StreamReader(file);
            // Reading Opcode
            var input = reader.ReadLine();

            var amplifiers = new Amplifiers(input);

            //var listOfPhases = new List<string> { "1", "0", "4", "3", "2" };
            //var factor = amplifiers.RunAmplifiers(listOfPhases);
            //Console.WriteLine($"het antwoord op vraag 1: {factor}");


            var allowedPhases = new List<string> { "0", "1", "2", "3", "4" };
            var maxResult = amplifiers.GetMaxAmplifier(allowedPhases);
            Console.WriteLine($"het antwoord op vraag 1: {maxResult}");

            var newAmplifiers = new Amplifiers(input);

            //var listOfPhases = new List<string> { "9", "8", "7", "6", "5" };
            //var factor = newAmplifiers.RunAmplifiers(listOfPhases);
            //Console.WriteLine($"het antwoord op vraag 2: {factor}");

            var allowedPhases2 = new List<string> { "5", "6", "7", "8", "9" };
            var maxResult2 = newAmplifiers.GetMaxAmplifier(allowedPhases2);
            Console.WriteLine($"het antwoord op vraag 2: {maxResult2}");

            Console.WriteLine("De vraag is opgelost");
        }

    }
    public class Amplifiers
    {
        List<IntCode> ListOfAmplifiers { get; }
        List<Queue<string>> ListOfInputQueues { get; }
        public Amplifiers(string input)
        {
            ListOfAmplifiers = new List<IntCode>();
            ListOfInputQueues = new List<Queue<string>>();
            for (var i = 0; i < 5; i++)
            {
                ListOfAmplifiers.Add(new IntCode(input));
                ListOfInputQueues.Add(new Queue<string>());
            }
        }
        public int GetMaxAmplifier(List<string> allowedPhases)
        {
            var maxResult = 0;
            var possibleListOfPhases = Helperfunctions.GetPossiblePhases(allowedPhases);
            {
                foreach(var phase in possibleListOfPhases)
                {
                    var factor = RunAmplifiers(phase);
                    maxResult = (factor > maxResult) ? factor : maxResult;
                }
            }
            return maxResult;
        }
        public int RunAmplifiers(List<string> phases)
        {
            var result = 0;
            for (var i = 0; i < 5; i++)
            {
                ListOfAmplifiers[i].Initialize();
                ListOfInputQueues[i].Enqueue(phases[i]);
                result = ListOfAmplifiers[i].RunCode(ListOfInputQueues[i], ListOfInputQueues[(i + 1) % 5]);
            }
            //Console.WriteLine("Phases initialised");
            ListOfInputQueues[0].Enqueue("0");
            while (result != 99)
            {
                ListOfAmplifiers[0].RunCode(ListOfInputQueues[0], ListOfInputQueues[1]);
                ListOfAmplifiers[1].RunCode(ListOfInputQueues[1], ListOfInputQueues[2]);
                ListOfAmplifiers[2].RunCode(ListOfInputQueues[2], ListOfInputQueues[3]);
                ListOfAmplifiers[3].RunCode(ListOfInputQueues[3], ListOfInputQueues[4]);
                result = ListOfAmplifiers[4].RunCode(ListOfInputQueues[4], ListOfInputQueues[0]);
            }
            //Console.WriteLine("Program stopped");
            var gelukt = false;
            var waarde = "";
            while (!gelukt)
            {
                gelukt = ListOfInputQueues[0].TryDequeue(out waarde);
            }
            //Console.WriteLine($"Waarde in RunAplifiers {waarde}");
            return int.Parse(waarde);
        }
    }
    public class IntCode
    {
        public List<int> OpCode { get; set; }
        public int Pointer { get; set; }
        string Code { get; }
        public IntCode(string code)
        {
            var pointer = 0;
            Code = code;
            OpCode = new List<int>(Code.Split(',').Select(x => int.Parse(x)));
            Pointer = pointer;
        }
        public bool Initialize()
        {
            Pointer = 0;
            OpCode.Clear();
            OpCode.AddRange(Code.Split(',').Select(x => int.Parse(x)));
            return true;
        }
        public int RunCode(Queue<string> input, Queue<string> output)
        {
            var opCode = OpCode[Pointer] % 100;
            switch (opCode)
            {
                case 1:
                    opCode = OpCode1(); // sum
                    break;
                case 2:
                    opCode = OpCode2(); // multiply
                    break;
                case 3:
                    opCode = OpCode3(input); // read input
                    break;
                case 4:
                    opCode = OpCode4(output); // write output
                    break;
                case 5:
                    opCode = OpCode5(); // jump if true
                    break;
                case 6:
                    opCode = OpCode6(); // jump if false
                    break;
                case 7:
                    opCode = OpCode7(); // less then
                    break;
                case 8:
                    opCode = OpCode8(); // equals
                    break;
                case 99: //  end program, 
                         //Console.WriteLine("The Program is finished");
                    break;
                default:
                    // you'r not supposed to reach this point
                    Console.WriteLine("Oops... something went wrong");
                    break;
            }
            return opCode;
        }
        int OpCode1() // sum
        {
            var mode1 = (OpCode[Pointer] / 100) % 10 == 1 ? true : false;
            var mode2 = (OpCode[Pointer] / 1000) % 10 == 1 ? true : false;
            Pointer++;
            var firstNumber = (mode1) ? OpCode[Pointer] : OpCode[OpCode[Pointer]];
            Pointer++;
            var secondNumber = (mode2) ? OpCode[Pointer] : OpCode[OpCode[Pointer]];
            Pointer++;
            OpCode[OpCode[Pointer]] = firstNumber + secondNumber;
            Pointer++;
            return OpCode[Pointer] % 100;
        }
        int OpCode2() // multiply
        {
            var mode1 = (OpCode[Pointer] / 100) % 10 == 1 ? true : false;
            var mode2 = (OpCode[Pointer] / 1000) % 10 == 1 ? true : false;
            Pointer++;
            var firstNumber = (mode1) ? OpCode[Pointer] : OpCode[OpCode[Pointer]];
            Pointer++;
            var secondNumber = (mode2) ? OpCode[Pointer] : OpCode[OpCode[Pointer]];
            Pointer++;
            OpCode[OpCode[Pointer]] = firstNumber * secondNumber;
            Pointer++;
            return OpCode[Pointer] % 100;
        }
        int OpCode3(Queue<string> input) //read input
        {
            var gelukt = input.TryDequeue(out var waarde);
            if (gelukt)
            {
                Pointer++;
                var firstNumber = OpCode[Pointer];
                Pointer++;
                //Console.WriteLine($"Amplifier input is: {waarde}");
                OpCode[firstNumber] = int.Parse(waarde);
            }
            return OpCode[Pointer] % 100;
        }
        int OpCode4(Queue<string> output) //write output
        {
            Pointer++;
            var firstNumber = OpCode[Pointer];
            Pointer++;
            output.Enqueue(OpCode[firstNumber].ToString());
            //Console.WriteLine($"Amplifier output is: {OpCode[firstNumber]}");
            return OpCode[Pointer] % 100;
        }
        int OpCode5() //jump if true
        {
            var mode1 = (OpCode[Pointer] / 100) % 10 == 1 ? true : false;
            var mode2 = (OpCode[Pointer] / 1000) % 10 == 1 ? true : false;
            Pointer++;
            var firstNumber = (mode1) ? OpCode[Pointer] : OpCode[OpCode[Pointer]];
            Pointer++;
            var secondNumber = (mode2) ? OpCode[Pointer] : OpCode[OpCode[Pointer]];
            if (firstNumber != 0) Pointer = secondNumber; else Pointer++;
            return OpCode[Pointer] % 100;
        }
        int OpCode6() //jump if false
        {
            var mode1 = (OpCode[Pointer] / 100) % 10 == 1 ? true : false;
            var mode2 = (OpCode[Pointer] / 1000) % 10 == 1 ? true : false;
            Pointer++;
            var firstNumber = (mode1) ? OpCode[Pointer] : OpCode[OpCode[Pointer]];
            Pointer++;
            var secondNumber = (mode2) ? OpCode[Pointer] : OpCode[OpCode[Pointer]];
            if (firstNumber == 0) Pointer = secondNumber; else Pointer++;
            return OpCode[Pointer] % 100;
        }
        int OpCode7() // less then
        {
            var mode1 = (OpCode[Pointer] / 100) % 10 == 1 ? true : false;
            var mode2 = (OpCode[Pointer] / 1000) % 10 == 1 ? true : false;
            Pointer++;
            var firstNumber = (mode1) ? OpCode[Pointer] : OpCode[OpCode[Pointer]];
            Pointer++;
            var secondNumber = (mode2) ? OpCode[Pointer] : OpCode[OpCode[Pointer]];
            Pointer++;
            OpCode[OpCode[Pointer]] = (firstNumber < secondNumber) ? 1 : 0;
            Pointer++;
            return OpCode[Pointer] % 100;
        }
        int OpCode8() // equals
        {
            var mode1 = (OpCode[Pointer] / 100) % 10 == 1 ? true : false;
            var mode2 = (OpCode[Pointer] / 1000) % 10 == 1 ? true : false;
            Pointer++;
            var firstNumber = (mode1) ? OpCode[Pointer] : OpCode[OpCode[Pointer]];
            Pointer++;
            var secondNumber = (mode2) ? OpCode[Pointer] : OpCode[OpCode[Pointer]];
            Pointer++;
            OpCode[OpCode[Pointer]] = (firstNumber == secondNumber) ? 1 : 0;
            Pointer++;
            return OpCode[Pointer] % 100;
        }
    }

    public class Helperfunctions
    {
        public static List<List<string>> GetPossiblePhases(List<string> phases)
        {
            var listOfPhases = new List<List<string>>();
            for (var i0 = 0; i0 <= 4; i0++)
            {
                for (var i1 = 0; i1 <= 4; i1++)
                {
                    if (i1 != i0)
                    {
                        for (var i2 = 0; i2 <= 4; i2++)
                        {
                            if (i2 != i1 && i2 != i0)
                            {
                                for (var i3 = 0; i3 <= 4; i3++)
                                {
                                    if (i3 != i2 && i3 != i1 && i3 != i0)
                                    {
                                        for (var i4 = 0; i4 <= 4; i4++)
                                        {
                                            if (i4 != i3 && i4 != i2 && i4 != i1 && i4 != i0)
                                            {
                                                var newPhase = new List<string>
                                                {
                                                    phases[i0],
                                                    phases[i1],
                                                    phases[i2],
                                                    phases[i3],
                                                    phases[i4],
                                                };
                                                listOfPhases.Add(newPhase);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return listOfPhases;
        }
    }
}
