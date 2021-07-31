using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace Dag_15
{
    class Program
    {
        static void Main(string[] args)
        {
            using var file = new FileStream("Input.txt", FileMode.Open);
            using var reader = new StreamReader(file);
            Console.WriteLine("Start Software");
            var codelines = reader.ReadLine();
            var output = new Queue<long>();
            var input = new Queue<long>();
            var repairDroid = new IntCode(codelines);

            // question 1
            repairDroid.RunDroid();
            

            Console.WriteLine("De vraag is opgelost");
        }
    }
    
    public class IntCode
    {
        public List<long> OpCode { get; set; }
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
        public bool AutoRunDroid()
        {
            // aanpassen.
            var output = new Queue<long>();
            var input = new Queue<long>();
            int posX = 0;
            int posY = 0;
            var Field = new Dictionary<(int x, int y), char>();
            Field.Add((posX, posY), 'X');
            long readval = 1;
            long result = 0;

            while (result != 99 && readval != 0)
            {
                readval = long.Parse(Console.ReadKey().KeyChar.ToString());

                while (readval < 0 || readval > 4)
                {
                    readval = long.Parse(Console.ReadKey().KeyChar.ToString());
                }
                input.Enqueue(readval);
                Console.WriteLine();
                while (output.Count == 0 && result != 99 && readval != 0)
                {
                    result = RunCode(output, input);
                }
                if (output.Count != 0)
                {
                    Console.WriteLine($"Direction is: {(Directions)readval}");
                    var fieldVal = output.Dequeue();
                    Console.WriteLine($"Environment is: {(Environment)fieldVal} ");
                    var newCoord = readval switch
                    {
                        1 => (posX, posY - 1),
                        2 => (posX, posY + 1),
                        3 => (posX - 1, posY),
                        4 => (posX + 1, posY),
                        _ => throw new InvalidOperationException(),
                    };
                    if (!Field.ContainsKey(newCoord))
                    {
                        Field.Add(newCoord, fieldVal switch
                        {
                            0 => '#',
                            1 => '.',
                            2 => 'O',
                            _ => throw new InvalidOperationException(),
                        });
                    }
                    // Print field
                    Helperfunctions.PrintField(Field, posX, posY);
                }
            }
            return true;
        }
        public bool RunDroid()
        {
            var output = new Queue<long>();
            var input = new Queue<long>();
            int posX = 0;
            int posY = 0;
            var Field = new Dictionary<(int x, int y), char>();
            Field.Add((posX, posY), 'X');
            long readval = 1;
            long result = 0;
            while (result != 99 && readval!=0)
            {
                readval = long.Parse(Console.ReadKey().KeyChar.ToString());
                
                while(readval <0 || readval > 4)
                {
                    readval = long.Parse(Console.ReadKey().KeyChar.ToString());
                }
                input.Enqueue(readval);
                Console.WriteLine();
                while (output.Count == 0 && result !=99 && readval != 0)
                {
                    result = RunCode(output, input);
                }
                if (output.Count != 0)
                {
                    Console.WriteLine($"Direction is: {(Directions)readval}");
                    var fieldVal = output.Dequeue();
                    Console.WriteLine($"Environment is: {(Environment)fieldVal} ");
                    // Add position and value to dictionary
                    switch (fieldVal)
                    {
                        case 0:
                            // droid hits wall, position unchanged
                            // add wall to dictionary
                            switch (readval)
                            {
                                case 1:
                                    //north
                                    // check if entry exists
                                    if(!Field.ContainsKey((posX, posY - 1)))
                                    {
                                        Field.Add((posX, posY - 1), '#');
                                    }
                                    break;
                                case 2:
                                    //south
                                    if (!Field.ContainsKey((posX, posY + 1)))
                                    {
                                        Field.Add((posX, posY + 1), '#');
                                    }
                                    break;
                                case 3:
                                    //west
                                    if (!Field.ContainsKey((posX - 1, posY)))
                                    {
                                        Field.Add((posX - 1, posY), '#');
                                    }
                                    break;
                                case 4:
                                    //east
                                    if (!Field.ContainsKey((posX + 1, posY)))
                                    {
                                        Field.Add((posX + 1, posY ), '#');
                                    }
                                    break;
                                default:
                                    // error, you are not supposed to reach this
                                    break;
                            }
                            break;
                        case 1:
                            // droid moves in direction
                            // change position
                            // add explored position to dictionary
                            switch (readval)
                            {
                                case 1:
                                    //north
                                    // check if entry exists
                                    if (!Field.ContainsKey((posX, posY - 1)))
                                    {
                                        Field.Add((posX, posY - 1), '.');
                                    }
                                    posY--;
                                    break;
                                case 2:
                                    //south
                                    if (!Field.ContainsKey((posX, posY + 1)))
                                    {
                                        Field.Add((posX, posY + 1), '.');
                                    }
                                    posY++;
                                    break;
                                case 3:
                                    //west
                                    if (!Field.ContainsKey((posX - 1, posY)))
                                    {
                                        Field.Add((posX - 1, posY), '.');
                                    }
                                    posX--;
                                    break;
                                case 4:
                                    //east
                                    if (!Field.ContainsKey((posX + 1, posY)))
                                    {
                                        Field.Add((posX + 1, posY), '.');
                                    }
                                    posX++;
                                    break;
                                default:
                                    // error, you are not supposed to reach this
                                    break;
                            }
                            break;
                        case 2:
                            // droid moves in direction & finds oxygen
                            switch (readval)
                            {
                                case 1:
                                    //north
                                    // check if entry exists
                                    if (!Field.ContainsKey((posX, posY - 1)))
                                    {
                                        Field.Add((posX, posY - 1), 'O');
                                    }
                                    posY--;
                                    break;
                                case 2:
                                    //south
                                    if (!Field.ContainsKey((posX, posY + 1)))
                                    {
                                        Field.Add((posX, posY + 1), 'O');
                                    }
                                    posY++;
                                    break;
                                case 3:
                                    //west
                                    if (!Field.ContainsKey((posX - 1, posY)))
                                    {
                                        Field.Add((posX - 1, posY), 'O');
                                    }
                                    posX--;
                                    break;
                                case 4:
                                    //east
                                    if (!Field.ContainsKey((posX + 1, posY)))
                                    {
                                        Field.Add((posX + 1, posY), 'O');
                                    }
                                    posX++;
                                    break;
                                default:
                                    // error, you are not supposed to reach this
                                    break;
                            }
                            break;
                        default:
                            // error, you are not supposed to reach this
                            break;
                    }
                    // Print field
                    Helperfunctions.PrintField(Field, posX, posY);
                }  
            }
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
        public static bool PrintField (Dictionary<(int x , int y ), char > field, int pos_X, long pos_Y)
        {
            var xMax = field.Keys.Select(k => k.x).Max();
            var yMax = field.Keys.Select(k => k.y).Max();
            var xMin = field.Keys.Select(k => k.x).Min();
            var yMin = field.Keys.Select(k => k.y).Min();


            for (var i= yMin; i<=yMax; i++)
            {
                for (var j= xMin; j<=xMax; j++)
                {
                    if (field.ContainsKey((j, i)))
                    {
                        // add check if droid position
                        if(j == pos_X && i== pos_Y)
                        {
                            Console.Write("D");
                        }
                        else
                        {
                            Console.Write($"{field[(j, i)]}");
                        }
                    } else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }
            return true;
        }
    }
    public enum Directions
    {
        North = 1,
        South,
        West,
        East
    }
    public enum Environment
    {
        Wall,
        Move,
        Oxygen
    }
}
