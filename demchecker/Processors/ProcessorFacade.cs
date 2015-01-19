using demchecker.domain;
using demchecker.Formatters;
using demchecker.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demchecker.Processors
{
    public enum OperationType
    {
        ProcessAndFormat,
        ProcessSumarizeAndFormat
    }

    public class ProcessorFacade
    {
        private string _solutionPath;
        private ParserType _parserType;
        private FormatterType _formatterType;
        private OperationType _operationType;
        private char _csvDelimiter;

       public ProcessorFacade(ParserType parserType, FormatterType formatterType, OperationType operationType, string solutionPath, char csvDelimiter = ';')
       {
           _parserType = parserType;
           _formatterType = formatterType;
           _operationType = operationType;
           _solutionPath = solutionPath;
           _csvDelimiter = csvDelimiter;
       }

       public string Process()
       {
           var parser = ParserFactory.CreateParser(_parserType);
           var formatter = FormatterFactory.CreateFormatter(_formatterType, _csvDelimiter);
           parser.Parse(_solutionPath);

           if (_operationType == OperationType.ProcessAndFormat)
           {
               return formatter.Format(DemeterAnalysis.Current.Violations);
           }
           else if (_operationType == OperationType.ProcessSumarizeAndFormat)
           {
               return formatter.FormatAggregatedResult(DemeterAnalysis.Current.Violations);
           }
           else
           {
               throw new ArgumentException("Invalid operation type");
           }

       }






    }
}
