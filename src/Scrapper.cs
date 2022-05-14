using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Athos
{
    class Scrapper
    {
        #region Variables
        string _sFilename;
        Dictionary<string, int> _dicWords;

        int _iExpressionLength;
        List<string> _lsPreviousWords;

        Blacklist _blackList;        
        #endregion

        #region Constructor
        public Scrapper(string sInputFile)
        {
            _sFilename = sInputFile;
            _iExpressionLength = 4;
            _dicWords = new Dictionary<string, int>();
            _lsPreviousWords = new List<string>();

            _blackList = new Blacklist();
        }
        #endregion

        public void Launch()
        {
            Console.WriteLine("Reading file...");
            ReadFile();
            Console.WriteLine("Generating output file...");
            OutputData();
        }

        #region File access and read
        private void ReadFile()
        {
            if (!File.Exists(_sFilename))
            {
                Console.WriteLine($"File unknown : {_sFilename}");
                return;
            }
            
            try
            {
                using (StreamReader sr = new StreamReader(_sFilename))
                {
                    bool bFirstLineDone = false;

                    while (sr.Peek() >= 0)
                    {
                        string sLine = sr.ReadLine();
                        
                        // First line can contain metadata
                        if (!bFirstLineDone)
                        {
                            bFirstLineDone = true;
                            if (sLine.StartsWith("#ATHOS:"))
                            {
                                GetMetadata(sLine.Substring("#ATHOS:".Length));
                                
                                continue;
                            }
                        }

                        ScrapText(sr.ReadLine());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }
        #endregion

        #region Metadata
        string _sTitle;

        private void GetMetadata(string sData)
        {
            GetMetadata_Title(sData);
        }

        private void GetMetadata_Title(string sData)
        {
            Regex regex = new Regex(@"TITLE=(.*);");
            MatchCollection matches = regex.Matches(sData);
            if (matches.Count == 1)
            {
                _sTitle = matches[0].Groups[1].Value;
            }
        }
        #endregion

        #region Text scrapping
        private void ScrapText(string sText)
        {
            // Cleaning lines of specific characters
            string sCleanedLine = Regex.Replace(sText, @"[\.,\!\?_»«\:;]", string.Empty);
            sCleanedLine = Regex.Replace(sCleanedLine, @"--", string.Empty);

            
            
            string[] tWords = sCleanedLine.Split(' ');

            foreach(string sWord in tWords)
            {
                // Removing break-line characters
                string sWordLower = Regex.Replace(sWord, @"\s+", string.Empty);
                sWordLower = sWordLower.ToLower();

                if (sWordLower.Length == 0)
                    continue;

                // Single words dictionary
                // 1-character words are ignored
                if (sWordLower.Length > 1)
                {
                    AddWordToDictionary(sWordLower);
                }

                // Multiple-words
                if (_lsPreviousWords.Any())
                {
                    for(int iExpressionSize=1; iExpressionSize <= _iExpressionLength; iExpressionSize++)
                    {
                        if (iExpressionSize <= _lsPreviousWords.Count)
                        {
                            string sExpression = string.Empty;
                            for(int iIndex = iExpressionSize - 1; iIndex < _lsPreviousWords.Count; iIndex++)
                            {
                                sExpression += _lsPreviousWords[iIndex] + " ";
                            }
                            sExpression += sWordLower;

                            AddWordToDictionary(sExpression);
                        }
                    }
                }

                // Storing the word for expression
                if (_lsPreviousWords.Count == _iExpressionLength)
                {
                    _lsPreviousWords.RemoveRange(0, 1);
                }
                _lsPreviousWords.Add(sWordLower);
            }
        }

        private void AddWordToDictionary(string sWord)
        {
            if (!_blackList.IsListed(sWord))
            {
                if (_dicWords.ContainsKey(sWord))
                {
                    _dicWords[sWord]++;
                }
                else
                {
                    _dicWords.Add(sWord, 1);
                }
            }
        }
        #endregion

        #region Output scrapped data
        private void OutputData()
        {
            string sOutputFile = GenerateOutuptFileName();

            List<ScrappedData> lsData = new List<ScrappedData>();
            foreach (KeyValuePair<string, int> kvp in _dicWords)
            {
                if (kvp.Value > 1)
                {
                    lsData.Add(new ScrappedData(kvp.Key, kvp.Value));
                }
            }

            try
            {
                if (File.Exists(sOutputFile))
                {
                    File.Delete(sOutputFile);
                }
                string sDirectory = Path.GetDirectoryName(sOutputFile);
                if (!Directory.Exists(sDirectory))
                {
                    Directory.CreateDirectory(sDirectory);
                }

                using (StreamWriter sw = new StreamWriter(sOutputFile))
                {
                    sw.WriteLine("#Scrapped data generated by Athos");
                    
                    foreach(ScrappedData scData in lsData.OrderByDescending(d => d.Occurences))
                    {
                        sw.WriteLine($"{scData.Data} {scData.Occurences}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        private string GenerateOutuptFileName()
        {
            if (!string.IsNullOrEmpty(_sTitle))
            {
                return @"generated\" + _sTitle + @"\output.raw.txt";
            }
            else
            {
                return _sFilename + ".output";
            }
        }
        #endregion
    }
}
