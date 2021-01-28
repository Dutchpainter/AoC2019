using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dag_07
{
    class Program
    {
        static void Main(string[] args)
        {
            using var file = new FileStream("Inputtest.txt", FileMode.Open);
            using var reader = new StreamReader(file);
            // Reading Opcode
            var input = reader.ReadLine();

            var amplifiers = new Amplifiers(input);
            //var maxResult = amplifiers.GetMaxAmplifier();
            //Console.WriteLine($"het antwoord op vraag 1: {maxResult}");


            //var phaseStartValue = 5;
            var inputString = "9,8,7,6,5,";
            var factor = amplifiers.RunAmplifiers(inputString);

            //var maxResult2 = amplifiers.GetMaxAmplifier(phaseStartValue, loopAmplifiers);
            //Console.WriteLine($"het antwoord op vraag 2: {maxResult2}");

            Console.WriteLine("De vraag is opgelost");
        }

    }
    public class Amplifiers
    {
        List<Amplifier> ListOfAmplifiers { get; }
        public Amplifiers(string input)
        {
            ListOfAmplifiers = new List<Amplifier>();
            for (var i = 0; i < 5; i++)
            {
                ListOfAmplifiers.Add(new Amplifier(input));
            }
        }
        public int GetMaxAmplifier(int fromValue = 0)
        {
            var maxResult = 0;
            for (var i0 = fromValue; i0 <= fromValue + 4; i0++)
            {
                for (var i1 = fromValue; i1 <= fromValue + 4; i1++)
                {
                    if (i1 != i0)
                    {
                        for (var i2 = fromValue; i2 <= fromValue + 4; i2++)
                        {
                            if (i2 != i1 && i2 != i0)
                            {
                                for (var i3 = fromValue; i3 <= fromValue + 4; i3++)
                                {
                                    if (i3 != i2 && i3 != i1 && i3 != i0)
                                    {
                                        for (var i4 = fromValue; i4 <= fromValue + 4; i4++)
                                        {
                                            if (i4 != i3 && i4 != i2 && i4 != i1 && i4 != i0)
                                            {
                                                var inputString = i0.ToString() + "," + i1.ToString() + "," + i2.ToString() + "," + i3.ToString() + "," + i4.ToString();
                                                var factor = RunAmplifiers(inputString);
                                                maxResult = (factor > maxResult) ? factor : maxResult;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return maxResult;
        }
        public int RunAmplifiers(string phases)
        {
            var listOfPhases = new List<string>(phases.Split(','));
            var input = "0";
            var result = "";
            for (var i = 0; i < ListOfAmplifiers.Count; i++)
            {
                result = ListOfAmplifiers[i].RunAmplifier(listOfPhases[i] + "," + input);
                input = result;
            }
            return int.Parse(result);
        }
    }
    public class Amplifier
    {
        IntCode Program { get; }
        public Amplifier(string input)
        {
            Program = new IntCode(input);
        }
        public string RunAmplifier(string input)
        {
            return Program.RunCode(input);
        }
    }
    public class IntCode
    {
        public List<int> OpCode { get; set; }
        public int Pointer { get; set; }
        public string Code { get; set; }
        public IntCode(string code)
        {
            OpCode = new List<int>(code.Split(',').Select(x => int.Parse(x)));
            Pointer = 0;
            Code = code;
        }
        public string RunCode(string input = "")
        {
            var opCode = OpCode[Pointer] % 100;
            var output = "";
            var inputs = new List<string>(input.Split(','));
            var inputCounter = 0;
            while (opCode != 99)
            {
                switch (opCode)
                {
                    case 1:
                        opCode = OpCode1(); // sum
                        break;
                    case 2:
                        opCode = OpCode2(); // multiply
                        break;
                    case 3:
                        input = inputs[inputCounter];
                        if (inputCounter < inputs.Count - 1) inputCounter++;
                        opCode = OpCode3(input); // read input
                        break;
                    case 4:
                        var result = OpCode4();
                        output = result.output; // output output
                        opCode = result.opcode; // write output
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
                    case 99: //  end program, but you'r not supposed to reach this point
                        Console.WriteLine("The Program is finished");
                        break;
                    default:
                        // you'r not supposed to reach this point
                        Console.WriteLine("Oops... something went wrong");
                        break;
                }
            }
            Pointer = 0;
            Console.WriteLine("Program finished");
            return output;
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
        int OpCode3(string input) //read input
        {
            Pointer++;
            var firstNumber = OpCode[Pointer];
            Pointer++;
            if (input == "")
            {
                Console.WriteLine("Enter input");
                input = Console.ReadLine();
            }
            //Console.WriteLine($"Amplifier input is: {input}");
            OpCode[firstNumber] = int.Parse(input);
            return OpCode[Pointer] % 100;
        }
        (int opcode, string output) OpCode4() //write output
        {
            Pointer++;
            var firstNumber = OpCode[Pointer];
            Pointer++;
            var output = OpCode[firstNumber];
            //Console.WriteLine($"Amplifier output is: {output}");
            //Console.WriteLine();
            return (OpCode[Pointer] % 100, output.ToString());
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
}
