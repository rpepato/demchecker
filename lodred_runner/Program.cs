//The MIT License (MIT)

//Copyright (c) 2015 Roberto Pepato

//Permission is hereby granted, free of charge, to any person obtaining a copy of
//this software and associated documentation files (the "Software"), to deal in
//the Software without restriction, including without limitation the rights to
//use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//the Software, and to permit persons to whom the Software is furnished to do so,
//subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using demchecker.Formatters;
using demchecker.Parsers;
using demchecker.Processors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demckrunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                ParserType parserType;
                FormatterType formatterType;
                OperationType operationType;

                if (options.Parser == "LoDE")
                {
                    parserType = ParserType.LoDE;
                }
                else if (options.Parser == "LoD")
                {
                    parserType = ParserType.LoD;
                }
                else
                {
                    Console.WriteLine("Unsupported parser. Valid options are 'LoD' and 'LoDE'. Use the --help parameter to get additional help.");
                    return;
                }

                if (options.Formatter == "CSV")
                {
                    formatterType = FormatterType.CSV;
                }
                else
                {
                    Console.WriteLine("Unsupported formatter. Currently only the CSV formatter is supported. Use the --help parameter to get additional help.");
                    return;
                }

                if (options.OperationType == "LIST")
                {
                    operationType = OperationType.ProcessAndFormat;
                }
                else if (options.OperationType == "SUMMARY")
                {
                    operationType = OperationType.ProcessSumarizeAndFormat;
                }
                else
                {
                    Console.WriteLine("Unsupported operation type. Valid options are 'LIST' and 'SUMMARY'. Use the --help parameter to get additional help.");
                    return;
                }

                var processor = new ProcessorFacade(parserType, formatterType, operationType, options.SolutionPath);
                File.WriteAllText(options.OutputFile, processor.Process());
            }
        }
    }
}
