using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demchecker.Formatters
{
    public enum FormatterType
    {
        CSV
    }

    static class FormatterFactory
    {
        public static IFormatter CreateFormatter(FormatterType type, char delimiter = ';')
        {
            switch(type)
            {
                case FormatterType.CSV:
                    {
                        return new CSVFormatter(delimiter);
                    }
                default:
                    {
                        var validFormatters = new string[] { "CSV" };
                        throw new ArgumentException(string.Format("The required formatter is not valid. Valid formatters are {0}", string.Join(", ", validFormatters)));
                    }
            }
        }
    }
}
