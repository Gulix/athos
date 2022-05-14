﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athos.Library
{
    class CommonGlossary
    {
        public static void GenerateCommonGlossary()
        {
            List<Book> lsBooks = new List<Book>();

            // Getting the books we've already scrapped
            DirectoryInfo directory = new DirectoryInfo("generated");
            DirectoryInfo[] tBooksDir = directory.GetDirectories();
            foreach(DirectoryInfo dir in tBooksDir)
            {
                string sBookKey = dir.Name;
                Book book = new Book(sBookKey);
                lsBooks.Add(book);
            }

            // Getting all the scrapped data
            foreach(Book book in lsBooks)
            {
                book.LoadRawOutput();
            }

            // Processing all the data
            double _dWordsTakenIntoAccount = 0.001d; // If a book has 10.000 words, only the words that appear at least 10 (0.1%) are taken into account
            HashSet<string> _processedWords = new HashSet<string>();
            HashSet<string> _glossary = new HashSet<string>();
            foreach(Book book in lsBooks)
            {
                Console.WriteLine($"Processing words from {book._sTitle}");

                foreach(KeyValuePair<string, int> kvp in book._dicRawOutput)
                {
                    if (_processedWords.Contains(kvp.Key))
                        continue;

                    // Do we need to take this into account?
                    double dPresence = kvp.Value / book._rawTotal;
                    if (dPresence >= _dWordsTakenIntoAccount)
                    {
                        _processedWords.Add(kvp.Key);
                        Console.WriteLine($"--- Processing word from {book._sTitle} ... {kvp.Key}");

                        // Is this word present in at least another book ?
                        foreach(Book otherBook in lsBooks)
                        {
                            if (otherBook._sTitle != book._sTitle)
                            {
                                if (otherBook._dicRawOutput.ContainsKey(kvp.Key))
                                {
                                    double dPresenceInOther = otherBook._dicRawOutput[kvp.Key] / otherBook._rawTotal;
                                    if (dPresenceInOther >= _dWordsTakenIntoAccount)
                                    {
                                        Console.WriteLine($"----- Found in {otherBook._sTitle} and added !");
                                        _glossary.Add(kvp.Key);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Writing the glossary
            using (StreamWriter sw = new StreamWriter(@"generated\glossary.txt"))
            {
                sw.WriteLine("#Glossary generated by Athos");

                foreach (string sWord in _glossary)
                {
                    sw.WriteLine(sWord);
                }
            }
        }
    }
}
 