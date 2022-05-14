using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Athos.Library
{
    public class Book
    {
        #region Variables
        public string _sTitle;

        public Dictionary<string, int> _dicRawOutput;
        public double _rawTotal;
        #endregion

        public Book(string sTitle)
        {
            _sTitle = sTitle;
        }

        #region Raw output
        /* Each book is stored in the 'generated' directory.
         * Under the 'Title'/output.raw.txt file is found all the scrapped data from the book
         * */
        
        public void LoadRawOutput()
        {
            string sFileName = @"generated\" + _sTitle + @"\output.raw.txt";
            if (File.Exists(sFileName))
            {
                _dicRawOutput = new Dictionary<string, int>();

                try
                {
                    using (StreamReader sr = new StreamReader(sFileName))
                    {
                        bool bFirstLineDone = false;

                        _rawTotal = 0;

                        while (sr.Peek() >= 0)
                        {
                            string sLine = sr.ReadLine();

                            // First line contains metadata
                            if (!bFirstLineDone)
                            {
                                bFirstLineDone = true;
                                continue;
                            }

                            // Line is of format : Some text 1234
                            Regex regex = new Regex(@"(.*) ([0-9]*)");
                            MatchCollection matches = regex.Matches(sLine);
                            if (matches.Count == 1)
                            {
                                string sKey = matches[0].Groups[1].Value;
                                int iValue = 0;
                                if (int.TryParse(matches[0].Groups[2].Value, out iValue))
                                {
                                    _dicRawOutput.Add(sKey, iValue);
                                    _rawTotal += iValue;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The process failed: {0}", e.ToString());
                }
            }
            else
            {
                Console.WriteLine($"ERROR - File unknow {sFileName}");
            }
        }
        #endregion
    }
}
