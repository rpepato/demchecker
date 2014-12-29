using NRefactoryCUBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demchecker.analysis_content
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
