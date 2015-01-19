using demchecker.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demchecker.Formatters
{
    internal interface IFormatter
    {
        string Format(IList<IViolation> violations);

        string FormatAggregatedResult(IList<IViolation> violations);
    }
}
