using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dag_14
{
    class Program
    {
        static void Main(string[] args)
        {
            var reactions = File
                   .ReadAllLines("Input.txt")
                   .Where(s => !string.IsNullOrWhiteSpace(s))
                   .ToList();

            var TheFactory = new Nanofactory(reactions);

            Console.WriteLine("Nanofactory reactions ingelezen.");
            //Opdracht 1
            var ore = TheFactory.CalculateOre();
            Console.WriteLine($"De totale hoeveelheid benodigde Ore is: {ore}");

            //Opdracht 2
            long fuel = 1;
            long firstRange = 10000;
            long maxOre = 1000000000000;           
            long usedOre = ore*fuel;
            int teller = 0;
            while (fuel!=firstRange)
            {
                long someOre = TheFactory.CalculateOre();
                usedOre += someOre;
                fuel++;
                if (teller==1000)
                {
                    Console.WriteLine($"Fuel made: {fuel}, Ore used {usedOre}");
                    teller = 0;
                }
                teller++;
            }
            Console.WriteLine($"Fuel made: {fuel}, Ore used {usedOre}");
            long firstFactor = 10;
            usedOre = firstFactor * usedOre;
            TheFactory.adjustStock(firstFactor);
            fuel = firstFactor*fuel;

            long secondRange = firstRange * firstFactor + 1000;
            teller = 0;
            while (fuel != secondRange)
            {
                long someOre = TheFactory.CalculateOre();
                usedOre += someOre;
                fuel++;
                if (teller == 1000)
                {
                    Console.WriteLine($"Fuel made: {fuel}, Ore used {usedOre}");
                    teller = 0;
                }
                teller++;
            }
            Console.WriteLine($"Fuel made: {fuel}, Ore used {usedOre}");
            long secondFactor = 40;
            usedOre = secondFactor * usedOre;
            TheFactory.adjustStock(secondFactor);
            fuel = secondFactor * fuel;
            while (usedOre <= maxOre)
            {
                long someOre = TheFactory.CalculateOre();
                usedOre += someOre;
                fuel++;
                if (teller == 1000)
                {
                    Console.WriteLine($"Fuel made: {fuel}, Ore used {usedOre}");
                    teller = 0;
                }
                teller++;
            }
            fuel--;
            Console.WriteLine($"De totale hoeveelheid te produceren Fuel is: {fuel}");
        }
    }
    public class Nanofactory
    {
        List<Reaction> Reactions { get; set; }
        public Dictionary <string , long> Stock { get; set; }
        // public List<Reaction> BasicReactions { get; set; }
        public Nanofactory(List<string> reactions)
        {
            Reactions = new List< Reaction >();
            // BasicReactions = new List<Reaction>();
            Stock = new Dictionary<string, long>();
            foreach(var line in reactions)
            {
                Reactions.Add(new Reaction(line));
                var name = Reactions.Last().Product_name;
                if (name != "FUEL") Stock.Add(name, 0);
            }
        }
        public bool adjustStock(long times)
        {
            List<string> keys = new List<string>(Stock.Keys);
            foreach (string key in keys)
            {
                long available = Stock[key] * times;
                Stock[key] = available;   
            }
            return true;
        }
        public bool stockEmpty()
        {
            long stock = 0;
            foreach (var item in Stock)
            {
                stock += item.Value;
            }
            if (stock != 0) return true; else return false;
        }
        public int CalculateOre()
        {
            //initialisatie
            var totalOre = 0;
            var Ingredients_list = new List<(string Ingredient, long Number)>();
            int counter = 0;
            Ingredients_list = Reactions.Where(x => x.Product_name == "FUEL").Select(x => x.Ingredients).Single().ToList();
            string wat = "";
            long hoeveel = 0;
            while (counter < Ingredients_list.Count())
            {
                // Te vervangen ingredient
                wat = Ingredients_list[counter].Ingredient;
                hoeveel = Ingredients_list[counter].Number;
                // als ingredient = ore: overslaan, counter verhogen
                if (wat != "ORE")
                {
                    // Nieuw ingredient
                    var Ingredients_list_add = Reactions.Where(x => x.Product_name == wat).Select(x => x.Ingredients).Single().ToList();
                    var Ingredients_list_count = Reactions.Where(x => x.Product_name == wat).Select(x => x.Product_number).Single();
                    // kijken hoeveel keer ingredient gemaakt moet worden,
                    // toevoegen aan lijst,
                    // wat over is aan stock toevoegen
                    var aanwezig = Stock[wat];
                    if (hoeveel <= aanwezig)
                    {
                        Stock[wat] = aanwezig - hoeveel;
                    }
                    else
                    {
                        if(hoeveel <= Ingredients_list_count+aanwezig)
                        {
                            // 1 maal maken en toevoegen, rest aan stock toevoegen
                            Ingredients_list.AddRange(Ingredients_list_add);
                            Stock[wat] = aanwezig + Ingredients_list_count - hoeveel;
                        }
                        else
                        {
                            // berekenen hoeveel er nodig is en toevoegen.
                            var maal = (hoeveel - aanwezig) / Ingredients_list_count;
                            var modval = (hoeveel - aanwezig) % Ingredients_list_count;
                            if (modval != 0) maal++;
                            foreach ( var item in Ingredients_list_add)
                            {
                                Ingredients_list.Add((item.Ingredient, item.Number * maal));
                            }
                            Stock[wat] = aanwezig + (maal * Ingredients_list_count) - hoeveel;
                        }
                    }
                }
                counter++;
            }
            
            totalOre = Ingredients_list.Where(x=> x.Ingredient == "ORE").Select(x => x.Number).ToList().Aggregate(0, (a, b) => (int)(a + b));
            return totalOre;
        }   
    }

    public class Reaction
    {
        public string Product_name { get; }
        public long Product_number { get; }
        public List<(string Ingredient,long Number)> Ingredients { get; }
        public Reaction(string line)
        {
            Ingredients = new List<(string, long)>();
            var parts = new List<string>(line.Split(" => "));
            var product = new List<string>(parts.Last().Split(" "));
            Product_name = product.Last();
            Product_number = long.Parse(product.First());
            var ingredients = new List<string>(parts.First().Split(", "));
            foreach(var item in ingredients)
            {
                var ingredient = new List<string>(item.Split(" "));
                Ingredients.Add((ingredient.Last(), long.Parse(ingredient.First())));
            }
        }
    }
}
