using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demchecker.Parsers
{
    public enum ParserType
    {
        LoD,
        LoDE
    }
    static class ParserFactory
    {
        public static IParser CreateParser(ParserType type)
        {
            switch (type)
            {
                case ParserType.LoD:
                    {
                        return new LoDParser();
                    }
                case ParserType.LoDE:
                    {
                        return new LoDEParser();
                    }
                default:
                    {
                        var validParsers = new string[] { "LoD", "LoDE" };
                        throw new ArgumentException(string.Format("The required parser is not valid. Valid parsers are {0}", string.Join(", ", validParsers)));
                    }
            }
        }
    }
}
