using System;
using System.IO;
using System.Linq;

namespace Athos
{
    class Program
    {
        static void Main(string[] args)
        {
            string sInputFile = "";

            // Reading args
            string sInputCommand = args.FirstOrDefault(arg => arg.StartsWith("-IN="));
            if (!string.IsNullOrEmpty(sInputCommand))
            {
                sInputFile = sInputCommand.Substring("-IN=".Length);
            } 
            else
            {
                sInputFile = "input.txt";
            }

            if (!File.Exists(sInputFile))
            {
                Console.WriteLine(@"ERROR: Input file is required in arguments. -IN=c:\filename.txt");
                return;
            }


            Console.WriteLine("Athos will scrap your files!");

            Scrapper scrapper = new Scrapper(sInputFile);
            scrapper.Launch();
            
            Console.WriteLine("Data has been generated");

        }
    }
}
