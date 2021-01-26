using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dag_05
{
    class Program
    {
        static void Main(string[] args)
        {
            using var file = new FileStream("Input.txt", FileMode.Open);
            using var reader = new StreamReader(file);
            // Reading Opcode
            var input = reader.ReadLine();
            var software = new IntCode(input);

            software.RunCode();

            Console.WriteLine("De vraag is opgelost");
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

        public bool RunCode()
        {
            var opCode = OpCode[Pointer] % 100;
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
                        opCode = OpCode3(); // read input
                        break;
                    case 4:
                        opCode = OpCode4(); // write output
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
                    case 99: // stop program
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
        int OpCode3() //read input
        {
            Pointer++;
            var firstNumber = OpCode[Pointer];
            Pointer++;
            Console.WriteLine("Enter input");
            var input = Console.ReadLine();
            OpCode[firstNumber] = int.Parse(input);
            return OpCode[Pointer] % 100;
        }
        int OpCode4() //write output
        {
            Pointer++;
            var firstNumber = OpCode[Pointer];
            Pointer++;
            Console.WriteLine(OpCode[firstNumber]);
            return OpCode[Pointer] % 100;
        }
        int OpCode5() //jump if true
        {
            //change
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
            //change
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
            //change
            var mode1 = (OpCode[Pointer] / 100) % 10 == 1 ? true : false;
            var mode2 = (OpCode[Pointer] / 1000) % 10 == 1 ? true : false;
            Pointer++;
            var firstNumber = (mode1) ? OpCode[Pointer] : OpCode[OpCode[Pointer]];
            Pointer++;
            var secondNumber = (mode2) ? OpCode[Pointer] : OpCode[OpCode[Pointer]];
            Pointer++;
            OpCode[OpCode[Pointer]] = (firstNumber < secondNumber ) ? 1 : 0 ;
            Pointer++;
            return OpCode[Pointer] % 100;
        }
        int OpCode8() // equals
        {
            //change
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
