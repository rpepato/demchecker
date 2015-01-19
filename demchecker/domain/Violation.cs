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

using NRefactoryCUBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demchecker.domain
{
    public class Violation : IViolation
    {
        internal virtual Project Project { get; private set; }

        internal virtual Solution Solution { get; private set; }

        internal virtual File File { get; private set; }

        public virtual Class Klass { get; private set; }

        public virtual Method Method { get; private set; }

        public virtual string ViolationExpression { get; private set; }

        public virtual int LineNumber { get; private set; }

        public Violation(Solution solution,
                         Project project,
                         File file,
                         string violationExpression,
                         int lineNumber, 
                         Class klass,
                         Method method)
        {
            Solution = solution;
            Project = project;
            File = file;
            ViolationExpression = violationExpression;
            LineNumber = lineNumber;
            Klass = klass;
            Method = method;
        }

        public virtual string ProjectName
        {
            get
            {
                return Project.Title;
            }
        }

        public virtual string ProjectPath
        {
            get
            {
                return Project.FileName;
            }
        }

        public virtual string SolutionName
        {
            get
            {
                return Solution.Name;
            }
        }

        public virtual string SolutionPath
        {
            get
            {
                return Solution.Path;
            }
        }

        public virtual string FileName
        {
            get
            {
                return File.Name;
            }
        }
    }
}
