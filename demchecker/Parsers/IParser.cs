using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demchecker.Parsers
{
    interface IParser
    {
        void Parse(string solutionPath);
    }
}
