using NRefactoryCUBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demchecker.analysis_content
{
    public class Violation
    {
        internal Project Project { get; private set; }

        internal Solution Solution { get; private set; }

        internal File File { get; private set; }

        public Class Klass { get; private set; }

        public Method Method { get; private set; }

        public string ViolationExpression { get; private set; }

        public int LineNumber { get; private set; }

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

        public string ProjectName
        {
            get
            {
                return Project.Title;
            }
        }

        public string ProjectPath
        {
            get
            {
                return Project.FileName;
            }
        }

        public string SolutionName
        {
            get
            {
                return Solution.Name;
            }
        }

        public string SolutionPath
        {
            get
            {
                return Solution.Path;
            }
        }

        public string FileName
        {
            get
            {
                return File.Name;
            }
        }
    }
}
