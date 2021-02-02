using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace Dag_09
{
    class Program
    {
        static void Main(string[] args)
        {
            using var file = new FileStream("Input.txt", FileMode.Open);
            using var reader = new StreamReader(file);
            // Reading Opcode
            var codelines = reader.ReadLine();

            var paintingMachine = new PaintingMachine(codelines);
            paintingMachine.RunPaintingMachine();
            var paintedTiles = paintingMachine.PaintedTiles.Count;
            Console.WriteLine($"Number of painted Tiles: {paintedTiles}");
            Console.WriteLine();
            var paintingMachine2 = new PaintingMachine(codelines);
            paintingMachine2.RunPaintingMachine(Color.White);
            paintingMachine2.PrintPainting();
            Console.WriteLine();
            Console.WriteLine("De vraag is opgelost");
        }
    }

    public class PaintingMachine
    {
        public Dictionary<(int xPos, int yPos), Color > PaintedTiles { get; set; }
        int PosX { get; set; }
        int PosY { get; set; }
        int Counter { get; set; }
        Directions Direction { get; set; }
        IntCode Computer { get; }
        bool Init { get; set; }
        public PaintingMachine( string input)
        {
            PaintedTiles = new Dictionary<(int xPos, int yPos), Color>();
            PosX = 0;
            PosY = 0;
            Direction = Directions.Up;
            Computer = new IntCode(input);
            Init = true;
        }
        public bool PrintPainting()
        {
            var maxLine = PaintedTiles.Keys.Select(k => k.yPos).Max();
            var maxRow = PaintedTiles.Keys.Select(k => k.xPos).Max();
            var minLine = PaintedTiles.Keys.Select(k => k.yPos).Min();
            var minRow = PaintedTiles.Keys.Select(k => k.xPos).Min();
            char[,] painting = new char[maxLine - minLine + 1, maxRow - minRow + 1];
            foreach(var tile in PaintedTiles)
            {
                int line = tile.Key.yPos;
                int row = tile.Key.xPos;
                char value = (tile.Value == Color.White) ? '#' : ' ';
                painting[line-minLine, row-minRow] = value;
            }
            for(var l = maxLine - minLine; l >= 0; l--)
            {
                for (var r = 0; r <= maxRow-minRow; r++)
                    {
                    if (painting[l, r] == 0)
                    {
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.Write(painting[l, r]);
                    } 
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            return true;
        }
        public bool RunPaintingMachine(Color startColor = Color.Black)
        {
            var output = new Queue<long>();
            var input = new Queue<long>();
            long result = 0;
            long moveDirection;
            if (Init)input.Enqueue((long)startColor);
            while (result != 99)
            {
                if (PaintedTiles.ContainsKey((PosX, PosY)))
                {
                    input.Enqueue((long)PaintedTiles[(PosX, PosY)]);
                }
                else
                {
                    if (!Init) input.Enqueue((long)Color.Black);
                    Init = false;
                }
                bool gelukt = false;
                long waarde = 0;
                while (!gelukt && result != 99)
                {
                    result = Computer.RunCode(output, input);
                    gelukt = output.TryDequeue(out waarde);
                }
                var paintColor = (Color)waarde;
                if(PaintedTiles.ContainsKey((PosX, PosY)))
                {
                    PaintedTiles[(PosX, PosY)] = paintColor;
                }
                else
                {
                    PaintedTiles.Add((PosX, PosY),paintColor);   
                }
                gelukt = false;
                while (!gelukt && result != 99)
                {
                    result = Computer.RunCode(output, input);
                    gelukt = output.TryDequeue(out waarde);
                }
                moveDirection = waarde;
                calculateNewPosition(moveDirection);      
            }
            return true;
        }
        bool calculateNewPosition(long moveDirection)
        {
            switch (Direction)
            {
                case Directions.Up:
                    if (moveDirection == 0)
                    {
                        Direction = Directions.Left;
                        PosX--;
                    }
                    else
                    {
                        Direction = Directions.Right;
                        PosX++;
                    }
                    break;
                case Directions.Right:
                    if (moveDirection == 0)
                    {
                        Direction = Directions.Up;
                        PosY++;
                    }
                    else
                    {
                        Direction = Directions.Down;
                        PosY--;
                    }
                    break;
                case Directions.Down:
                    if (moveDirection == 0)
                    {
                        Direction = Directions.Right;
                        PosX++;
                    }
                    else
                    {
                        Direction = Directions.Left;
                        PosX--;
                    }
                    break;
                case Directions.Left:
                    if (moveDirection == 0)
                    {
                        Direction = Directions.Down;
                        PosY--;
                    }
                    else
                    {
                        Direction = Directions.Up;
                        PosY++;
                    }
                    break;
                default:
                    Console.WriteLine("Oops, something went wrong");
                    break;
            }
            return true;
        }
    }
    public class IntCode
    {
        List<long> OpCode { get; set; }
        int Pointer { get; set; }
        int RelativeBasePointer { get; set; }
        int MemCount { get; set; }
        int MaxArgs { get; }
        string Code { get; }
        public IntCode(string code)
        {
            MaxArgs = 3;
            Code = code;
            OpCode = new List<long>(Code.Split(',').Select(x => long.Parse(x)));
            Pointer = 0;
            RelativeBasePointer = 0;
            MemCount = OpCode.Count;
        }
        bool CheckMem(long value = 0)
        {
            if (value == 0) value = Pointer;
            if (value >= MemCount - MaxArgs)
            {
                for (var i = 0; i <= (value - MemCount + MaxArgs); i++)
                {
                    OpCode.Add(0);
                }
            }
            MemCount = OpCode.Count;
            return true;
        }
        (long modePosition1, long modePosition2, long modePosition3) GetModes()
        {
            CheckMem();
            var modePosition1 = OpCode[Pointer] / 100 % 10;
            var modePosition2 = OpCode[Pointer] / 1000 % 10;
            var modePosition3 = OpCode[Pointer] / 10000;
            return (modePosition1, modePosition2, modePosition3);
        }
        bool SetValue(long value, long mode)
        {
            switch (mode)
            {
                case 0:
                    CheckMem();
                    CheckMem(OpCode[Pointer]);
                    OpCode[(int)OpCode[Pointer]] = value;
                    break;
                case 1:
                    // you'r not supposed to reach this point it is unused
                    Console.WriteLine("Oops... something went wrong");
                    break;
                case 2:
                    CheckMem();
                    CheckMem(OpCode[Pointer] + RelativeBasePointer);
                    OpCode[(int)OpCode[Pointer] + RelativeBasePointer] = value;
                    break;
                default:
                    // you'r not supposed to reach this point
                    Console.WriteLine("Oops... something went wrong");
                    break;
            }
            return true;
        }
        long GetOpCode()
        {
            var result = OpCode[Pointer] % 100;
            return result;
        }
        long GetValue(long mode)
        {
            long value = 0;
            switch (mode)
            {
                case 0: // position mode
                    CheckMem();
                    CheckMem(OpCode[Pointer]);
                    value = OpCode[(int)OpCode[Pointer]];
                    break;
                case 1: // immediate mode
                    CheckMem();
                    value = OpCode[Pointer];
                    break;
                case 2: // relative mode
                    CheckMem();
                    CheckMem(OpCode[Pointer] + RelativeBasePointer);
                    value = OpCode[(int)OpCode[Pointer] + RelativeBasePointer];
                    break;
                default:
                    // you'r not supposed to reach this point
                    Console.WriteLine("Oops... something went wrong");
                    break;
            }
            return value;
        }
        public bool Initialize()
        {
            Pointer = 0;
            RelativeBasePointer = 0;
            OpCode.Clear();
            OpCode.AddRange(Code.Split(',').Select(x => long.Parse(x)));
            MemCount = OpCode.Count;
            return true;
        }
        public bool RunSoftware(Queue<long> output, Queue<long> input = null)
        {
            long result = 0;
            while (result != 99)
            {
                result = RunCode(output, input);
            }
            return true;
        }
        public long RunCode(Queue<long> output, Queue<long> input = null)
        {
            var opCode = GetOpCode();

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
                case 9:
                    opCode = OpCode9(); // adjust relative base offset
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
        long OpCode1() // sum
        {
            var modes = GetModes();
            Pointer++;
            var firstNumber = GetValue(modes.modePosition1);
            Pointer++;
            var secondNumber = GetValue(modes.modePosition2);
            Pointer++;
            var result = firstNumber + secondNumber;
            SetValue(result, modes.modePosition3);
            Pointer++;
            return GetOpCode();
        }
        long OpCode2() // multiply
        {
            var modes = GetModes();
            Pointer++;
            var firstNumber = GetValue(modes.modePosition1);
            Pointer++;
            var secondNumber = GetValue(modes.modePosition2);
            Pointer++;
            var result = firstNumber * secondNumber;
            SetValue(result, modes.modePosition3);
            Pointer++;
            return GetOpCode();
        }
        long OpCode3(Queue<long> input) //read input
        {
            var modes = GetModes();
            var gelukt = input.TryDequeue(out var waarde);
            if (gelukt)
            {
                Pointer++;
                //Console.WriteLine($"input is: {waarde}");
                SetValue(waarde, modes.modePosition1);
                Pointer++;
            }
            return GetOpCode();
        }
        long OpCode4(Queue<long> output) //write output
        {
            var modes = GetModes();
            Pointer++;
            var firstNumber = GetValue(modes.modePosition1);
            Pointer++;
            output.Enqueue(firstNumber);
            //Console.WriteLine($"output is: {firstNumber}");
            return GetOpCode();
        }
        long OpCode5() //jump if true
        {
            var modes = GetModes();
            Pointer++;
            var firstNumber = GetValue(modes.modePosition1);
            Pointer++;
            var secondNumber = GetValue(modes.modePosition2);
            if (firstNumber != 0) Pointer = (int)secondNumber; else Pointer++;
            return GetOpCode();
        }
        long OpCode6() //jump if false
        {
            var modes = GetModes();
            Pointer++;
            var firstNumber = GetValue(modes.modePosition1);
            Pointer++;
            var secondNumber = GetValue(modes.modePosition2);
            if (firstNumber == 0) Pointer = (int)secondNumber; else Pointer++;
            return GetOpCode();
        }
        long OpCode7() // less then
        {
            var modes = GetModes();
            Pointer++;
            var firstNumber = GetValue(modes.modePosition1);
            Pointer++;
            var secondNumber = GetValue(modes.modePosition2);
            Pointer++;
            long result = (firstNumber < secondNumber) ? 1 : 0;
            SetValue(result, modes.modePosition3);
            Pointer++;
            return GetOpCode();
        }
        long OpCode8() // equals
        {
            var modes = GetModes();
            Pointer++;
            var firstNumber = GetValue(modes.modePosition1);
            Pointer++;
            var secondNumber = GetValue(modes.modePosition2);
            Pointer++;
            long result = (firstNumber == secondNumber) ? 1 : 0;
            SetValue(result, modes.modePosition3);
            Pointer++;
            return GetOpCode();
        }
        long OpCode9() // adjust relative base offset
        {
            var modes = GetModes();
            Pointer++;
            var firstNumber = GetValue(modes.modePosition1);
            Pointer++;
            RelativeBasePointer += (int)firstNumber;
            return GetOpCode();
        }
    }

    public class Helperfunctions
    {
        public static bool PrintQueue(Queue<long> output)
        {
            Console.WriteLine("De output is: ");
            while (output.Count > 0)
            {
                {
                    Console.Write($"{output.Dequeue()} ");
                }
            }
            Console.WriteLine();
            Console.WriteLine();
            return true;
        }
    }
    public enum Directions
    {
        Up,
        Right,
        Down,
        Left
    }
    public enum Color
    {
        Black,
        White
    }
}
