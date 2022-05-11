using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athos
{
    class ScrappedData
    {
        string _sData;
        int _iOccurences;

        public string Data
        {
            get { return _sData; }
        }

        public int Occurences
        {
            get { return _iOccurences; }
        }

        public ScrappedData(string sData, int iOccurences)
        {
            _sData = sData;
            _iOccurences = iOccurences;
        }
    }
}
