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
using demchecker.Formatters;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpTestsEx;
using Moq;

namespace demchecker.tests.Formatters
{
    [TestFixture]
    class CSVFormatterTests
    {
        private IFormatter formatter;
        private string header;
        private string headerForAggregateResults;
        private const string OUT_OF_METHOD_BODY = "Out of a method body";

        [SetUp]
        public void SetUp()
        {
            formatter = new CSVFormatter(';');
            header = string.Join(";", new string[] { "Project Name", 
                                                     "File Path", 
                                                     "Class Name", 
                                                     "Method Name", 
                                                     "Expression", 
                                                     "Line Number" });

            headerForAggregateResults = string.Join(";", new string[] { "Solution", "Project", "Class", "Method", "Count" });
        }

        [Test]
        public void ShouldReportHeaderForFormattedNullSet()
        {
            formatter.Format(null).Should().Be(header);
        }

        [Test]
        public void ShouldReportHeaderForFormattedEmptyViolationSet()
        {
            formatter.Format(new List<IViolation>()).Should().Be(header);
        }

        [Test]
        public void ShouldReportedFormattedViolationSet()
        {
            var sb = new StringBuilder();
            sb.AppendLine(header);
            var violation1 = CreateMockViolation("C:\\MySolution.sln", "C:\\MyProject.csproj", "MyClass", "MyMethod", "new Class().DoSomething(3).GetResult()", "C:\\MyClass.cs", 75);
            var violation2 = CreateMockViolation("C:\\MySolution.sln", "C:\\MyProject.csproj", "MyClass2", "MyMethod", "new AnotherClass().DoSomething(3).GetResult()", "C:\\MyClass.cs", 26);
            var violation3 = CreateMockViolation("C:\\MySolution.sln", "C:\\MyProject.csproj", "MyClass2", null, "new YetAnotherClass().First().Second()", "C:\\MyClass.cs", 12);
            sb.AppendLine("C:\\MyProject.csproj;C:\\MyClass.cs;MyClass;MyMethod;new Class().DoSomething(3).GetResult();75");
            sb.AppendLine("C:\\MyProject.csproj;C:\\MyClass.cs;MyClass2;MyMethod;new AnotherClass().DoSomething(3).GetResult();26");
            sb.AppendLine("C:\\MyProject.csproj;C:\\MyClass.cs;MyClass2;" + OUT_OF_METHOD_BODY + ";new YetAnotherClass().First().Second();12");

            var formattedContent = formatter.Format(new List<IViolation>() { violation1, 
                                                                             violation2,
                                                                             violation3 
                                                                           });

            sb.ToString().TrimEnd(null).Should().Be(formattedContent);

        }

        [Test]
        public void ShouldReportHeaderForFormattedAggregateeNullViolationSet()
        {
            formatter.FormatAggregatedResult(null).Should().Be(headerForAggregateResults);
        }

        [Test]
        public void ShouldReportHeaderForFormattedAggregateeEmptyViolationSet()
        {
            formatter.FormatAggregatedResult(new List<IViolation>()).Should().Be(headerForAggregateResults);
        }

        [Test]
        public void ShouldReportAggregatedtViolationsOutOfMethodyBodies()
        {
            var violation = CreateMockViolation("C:\\MySolution.sln", "C:\\MyProject.csproj", "MyClass");
            var sb = new StringBuilder();
            sb.AppendLine(headerForAggregateResults);
            sb.AppendLine("C:\\MySolution.sln;C:\\MyProject.csproj;MyClass;Out of a method body;1");
            var formattedContent = formatter.FormatAggregatedResult(new IViolation[] { violation }.ToList());
            sb.ToString().TrimEnd(null).Should().Be(formattedContent);
        }

        [Test]
        public void ShouldReportAggregatedViolationsInsideMethodBodies()
        {
            var violation = CreateMockViolation("C:\\MySolution.sln", "C:\\MyProject.csproj", "MyClass", "MyMethod");
            var sb = new StringBuilder();
            sb.AppendLine(headerForAggregateResults);
            sb.AppendLine("C:\\MySolution.sln;C:\\MyProject.csproj;MyClass;MyMethod;1");
            var formattedContent = formatter.FormatAggregatedResult(new IViolation[] { violation }.ToList());
            sb.ToString().TrimEnd(null).Should().Be(formattedContent);
        }

        [Test]
        public void ShouldGroupAggregatedViolationsByMethods()
        {
            var violation1 = CreateMockViolation("C:\\MySolution.sln", "C:\\MyProject.csproj", "MyClass");
            var violation2 = CreateMockViolation("C:\\MySolution.sln", "C:\\MyProject.csproj", "MyClass");
            var violation3 = CreateMockViolation("C:\\MySolution.sln", "C:\\MyProject.csproj", "MyClass");

            var violation4 = CreateMockViolation("C:\\MySolution.sln", "C:\\MyProject.csproj", "MyClass", "MyMethod");
            var violation5 = CreateMockViolation("C:\\MySolution.sln", "C:\\MyProject.csproj", "MyClass", "MyMethod");

            var violation6 = CreateMockViolation("C:\\MySolution.sln", "C:\\MyProject.csproj", "MyClass", "MyMethod2");

            var sb = new StringBuilder();
            sb.AppendLine(headerForAggregateResults);
            sb.AppendLine("C:\\MySolution.sln;C:\\MyProject.csproj;MyClass;Out of a method body;3");
            sb.AppendLine("C:\\MySolution.sln;C:\\MyProject.csproj;MyClass;MyMethod;2");
            sb.AppendLine("C:\\MySolution.sln;C:\\MyProject.csproj;MyClass;MyMethod2;1");
            var formattedContent = formatter.FormatAggregatedResult(new IViolation[] { violation1,
                                                                                       violation2,
                                                                                       violation3,
                                                                                       violation4,
                                                                                       violation5, 
                                                                                       violation6 }.ToList());
            sb.ToString().TrimEnd(null).Should().Be(formattedContent);
        }

        [Test]
        public void ShouldAggregateViolationsByClass()
        {
            var violation1 = CreateMockViolation("C:\\MySolution.sln", "C:\\MyProject.csproj", "MyClass", "MyMethod");
            var violation2 = CreateMockViolation("C:\\MySolution.sln", "C:\\MyProject.csproj", "MyClass2", "MyMethod");
            var violation3 = CreateMockViolation("C:\\MySolution.sln", "C:\\MyProject.csproj", "MyClass2", "MyMethod");


            var sb = new StringBuilder();
            sb.AppendLine(headerForAggregateResults);
            sb.AppendLine("C:\\MySolution.sln;C:\\MyProject.csproj;MyClass;MyMethod;1");
            sb.AppendLine("C:\\MySolution.sln;C:\\MyProject.csproj;MyClass2;MyMethod;2");
            var formattedContent = formatter.FormatAggregatedResult(new IViolation[] { violation1,
                                                                                       violation2,
                                                                                       violation3
                                                                                      }.ToList());
            sb.ToString().TrimEnd(null).Should().Be(formattedContent);

        }

        [Test]
        public void ShouldAggregateViolationsByProject()
        {
            var violation1 = CreateMockViolation("C:\\MySolution.sln", "C:\\MyProject.csproj", "MyClass", "MyMethod");
            var violation2 = CreateMockViolation("C:\\MySolution.sln", "C:\\MyProject.csproj", "MyClass", "MyMethod");
            var violation3 = CreateMockViolation("C:\\MySolution.sln", "C:\\MyProject2.csproj", "MyClass", "MyMethod");


            var sb = new StringBuilder();
            sb.AppendLine(headerForAggregateResults);
            sb.AppendLine("C:\\MySolution.sln;C:\\MyProject.csproj;MyClass;MyMethod;2");
            sb.AppendLine("C:\\MySolution.sln;C:\\MyProject2.csproj;MyClass;MyMethod;1");
            var formattedContent = formatter.FormatAggregatedResult(new IViolation[] { violation1,
                                                                                       violation2,
                                                                                       violation3
                                                                                      }.ToList());
            sb.ToString().TrimEnd(null).Should().Be(formattedContent);

        }



        private IViolation CreateMockViolation(string solutionName, string projectName, string className, string methodName = null, string violationExpression = null, string fileName = null, int? lineNumber = null)
        {
            var klass = new Class() { Name = className };
            var mock = new Mock<IViolation>();
            mock.Setup(v => v.SolutionName).Returns(solutionName);
            mock.Setup(v => v.ProjectName).Returns(projectName);
            mock.Setup(v => v.Klass).Returns(klass);
            if (methodName != null)
            {
                mock.Setup(v => v.Method).Returns(new Method(klass, methodName));
            }

            if (violationExpression != null)
            {
                mock.Setup(v => v.ViolationExpression).Returns(violationExpression);
            }

            if (fileName != null)
            {
                mock.Setup(v => v.FileName).Returns(fileName);
            }

            if (lineNumber.HasValue)
            {
                mock.Setup(v => v.LineNumber).Returns(lineNumber.Value);
            }

            return mock.Object;
        }
        
    }
}
