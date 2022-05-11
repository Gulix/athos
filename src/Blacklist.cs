using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athos
{
    class Blacklist
    {
        #region Variables
        string _sBlacklistFile = "blacklist.txt";
        HashSet<string> _hsWords;
        #endregion

        #region Initialization
        public Blacklist()
        {
            Load();
        }

        private void Load()
        {
            _hsWords = new HashSet<string>();

            if (!File.Exists(_sBlacklistFile))
            {
                Console.WriteLine($"Blacklist file unknown : {_sBlacklistFile}");
            }
            else
            {
                try
                {
                    using (StreamReader sr = new StreamReader(_sBlacklistFile))
                    {
                        while (sr.Peek() >= 0)
                        {
                            _hsWords.Add(sr.ReadLine());
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The process failed: {0}", e.ToString());
                }
            }
        }
        #endregion

        public bool IsListed(string sWord)
        {
            return _hsWords.Contains(sWord);
        }
    }
}
