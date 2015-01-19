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

using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lodred_runner
{
    class Options
    {
        [Option('s', "solution", Required = true, HelpText="The path for your solution file (.sln)")]
        public string SolutionPath { get; set; }

        [Option('o', "output", Required = true, HelpText="The path to the output of the processing")]
        public string OutputFile { get; set; }

        [Option('p', "parser", Required = true, HelpText = "The type of the parser used during analysis (Valid options are LoD and LoDE. Default is LoDE)", DefaultValue = "LoDE")]
        public string Parser { get; set; }

        [Option('f', "formatter", Required = true, HelpText = "The type of the formatter used to format the processing output (Currently only the CSV formatter is supported)", DefaultValue="CSV")]
        public string Formatter { get; set; }

        [Option('t', "type", Required = true, HelpText = "The type of the analysis that should be performed. Valid values are 'LIST' and 'SUMMARY'. Default value is 'LIST'", DefaultValue = "LIST")]
        public string OperationType { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
