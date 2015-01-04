using demchecker.Formatters;
using demchecker.Parsers;
using demchecker.Processors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lodred_runner
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
