﻿//The MIT License (MIT)

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
