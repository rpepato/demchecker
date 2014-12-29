using System;
namespace demchecker.analysis_content
{
    interface IViolation
    {
        string FileName { get; }
        Class Klass { get; }
        int LineNumber { get; }
        Method Method { get; }
        string ProjectName { get; }
        string ProjectPath { get; }
        string SolutionName { get; }
        string SolutionPath { get; }
        string ViolationExpression { get; }
    }
}
