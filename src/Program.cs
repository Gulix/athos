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
            string sActionCommand = args.FirstOrDefault(arg => arg.StartsWith("-ACTION="));
            if (!string.IsNullOrEmpty(sActionCommand))
            {
                string sAction = sActionCommand.Substring("-ACTION=".Length);
                switch(sAction)
                {
                    case "GLOSSARY":
                        Library.CommonGlossary.GenerateCommonGlossary();
                        break;
                    default:
                        Console.WriteLine("Action unknown : " + sAction);
                        break;
;                }
            }
            else if (!string.IsNullOrEmpty(sInputCommand))
            {
                sInputFile = sInputCommand.Substring("-IN=".Length);

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
            else
            {
                Console.WriteLine("An action is required");
            }

        }
    }
}
