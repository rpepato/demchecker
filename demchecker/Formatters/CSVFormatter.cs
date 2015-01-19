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

using demchecker.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demchecker.Formatters
{
    internal class CSVFormatter : IFormatter
    {
        private char traillingCharacter;
        private const string OUT_OF_METHOD_BODY = "Out of a method body";

        public CSVFormatter(char traillingCharacter)
        {
            this.traillingCharacter = traillingCharacter;
        }

        public string Format(IList<IViolation> violations)
        {
            var header = string.Join(traillingCharacter.ToString(), new string[] { "Project Name", 
                                                         "File Path", 
                                                         "Class Name", 
                                                         "Method Name", 
                                                         "Expression", 
                                                         "Line Number" });
            var builder = new StringBuilder();
            builder.AppendLine(header);

            if (violations != null)
            {
                foreach (var violation in violations)
                {
                    builder.AppendLine(string.Format("{0}" + traillingCharacter +
                                                     "{1}" + traillingCharacter +
                                                     "{2}" + traillingCharacter +
                                                     "{3}" + traillingCharacter +
                                                     "{4}" + traillingCharacter +
                                                     "{5}",
                                                     violation.ProjectName,
                                                     violation.FileName,
                                                     violation.Klass.Name,
                                                     (violation.Method == null ? OUT_OF_METHOD_BODY : violation.Method.Name),
                                                     violation.ViolationExpression,
                                                     violation.LineNumber
                                                     ));
                }
            }
            return builder.ToString().TrimEnd(null);  
        }

        public string FormatAggregatedResult(IList<IViolation> violations)
        {
            var header = string.Join(";", new string[] { "Solution", "Project", "Class", "Method", "Count" });
            var builder = new StringBuilder();
            
            builder.AppendLine(header);
            if (violations != null)
            {
                var projects = violations.Select(v => v.ProjectName).Distinct();
                var solutionName = violations.Count() > 0 ? violations.First().SolutionName : "";

                foreach (var project in projects)
                {
                    foreach (var klass in violations.Where(v => v.ProjectName == project).Select(v => v.Klass.Name).Distinct())
                    {
                        // extract all expressions declared outside a method body
                        if (violations.Where(v => v.Method == null & v.ProjectName == project && v.Klass.Name == klass).Count() > 0)
                        {
                            var count = violations.Where(v => v.Method == null &&
                                                              v.ProjectName == project
                                                              && v.Klass.Name == klass).Count();

                            builder.AppendLine(string.Format("{0}" + traillingCharacter +
                                                             "{1}" + traillingCharacter +
                                                             "{2}" + traillingCharacter +
                                                             "{3}" + traillingCharacter +
                                                             "{4}",
                                                             solutionName,
                                                             project,
                                                             klass,
                                                             OUT_OF_METHOD_BODY,
                                                             count));
                        }

                        foreach (var method in violations.Where(v => v.Method != null && v.ProjectName == project && v.Klass.Name == klass).Select(v => v.Method.Name).Distinct())
                        {
                            var count = violations.Count(v => v.Method != null &&
                                                              v.ProjectName == project &&
                                                              v.Klass.Name == klass &&
                                                              v.Method.Name == method);

                            builder.AppendLine(string.Format("{0}" + traillingCharacter +
                                                             "{1}" + traillingCharacter +
                                                             "{2}" + traillingCharacter +
                                                             "{3}" + traillingCharacter +
                                                             "{4}",
                                                             solutionName,
                                                             project,
                                                             klass,
                                                             method,
                                                             count));
                        }
                    }
                }
            }

            return builder.ToString().TrimEnd(null);
        
        }
    }
}
