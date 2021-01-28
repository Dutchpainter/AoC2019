using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dag_08
{
    class Program
    {
        static void Main(string[] args)
        {
            using var file = new FileStream("Input.txt", FileMode.Open);
            using var reader = new StreamReader(file);
            // Reading input
            var input = reader.ReadLine();

            var width = 25;
            var height = 6;
            var picture = new Picture(width, height, input);

            var minOccurences = picture.MinOccurencesInLayers(0);
            var digits1 = picture.CountOccurencesInLayer(1, minOccurences);
            var digits2 = picture.CountOccurencesInLayer(2, minOccurences);
            Console.WriteLine($"De oplossing op vraag 1: {digits1 * digits2}");
            Console.WriteLine();

            picture.PrintRenderedPicture();

            Console.WriteLine("De opdracht is afgerond");
        }
    }
    public class Picture
    {
        int Width { get; }
        int Height { get; }
        int Layers { get; }
        string Input { get; }
        public List<int[,]> PictureLayers { get; }
        public int[,] RenderedPicture { get; }
        public Picture(int width, int height, string input)
        {
            Width = width;
            Height = height;
            Input = input;
            Layers = Input.Length / (Width * Height);

            PictureLayers = new List<int[,]>();
            var index = 0;
            for (var l = 0; l < Layers; l++)
            {
                int [,] pictureLayer  =  new int[Width,Height];
                for (var h = 0; h < Height; h++)
                {
                    for (var w = 0; w < Width; w++)
                    {
                        pictureLayer[w,h] = int.Parse(Input[index].ToString());
                        index++;
                    }
                }
                PictureLayers.Add(pictureLayer);
            }

            RenderedPicture = new int[Width, Height];
            RenderPicture();
        }
        bool RenderPicture()
        {
            for (var h = 0; h < Height; h++)
            {
                for (var w = 0; w < Width; w++)
                {
                    //0 is black, 1 is white, and 2 is transparent.
                    bool transparent = true;
                    var layer = 0;
                    while (transparent && layer< Layers)
                    {
                        RenderedPicture[w, h] = PictureLayers[layer][w, h];
                        transparent = (PictureLayers[layer][w, h] == 2) ? true : false;
                        layer++;
                    }       
                }
            }
            return true;
        }
        public bool PrintRenderedPicture()
        {
            for (var h = 0; h < Height; h++)
            {
                for (var w = 0; w < Width; w++)
                {
                    var dot = (RenderedPicture[w, h]== 1) ? "*" : " ";
                    Console.Write(dot);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            return true;
        }
        public int MinOccurencesInLayers(int search)
        {
            var occurences = new List<int>();
            for (var l = 0; l < Layers; l++)
            {
                occurences.Add(CountOccurencesInLayer(search,l));
            }
            var lowest = occurences.IndexOf(occurences.Min());
            return lowest;
        }
        public int CountOccurencesInLayer(int search, int layer)
        {
            var occurences = (from int item in PictureLayers[layer]
                             select item).Where(x => x== search).Count();
            return occurences;
        }
    }
}
