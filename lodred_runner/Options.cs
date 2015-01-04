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
