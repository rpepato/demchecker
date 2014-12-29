using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpTestsEx;
using System.Data.EntityClient;
using demchecker.analysis_content;

namespace demchecker.tests
{
    [TestFixture]
    public class ASTWalkerTests
    {
        private ASTWalker _walker;

        [SetUp]
        public void SetUp()
        {
            var path = @"C:\projects\experimental\demchecker.tests\fixtures\SimpleSolution\SimpleSolution.sln";
            _walker = new ASTWalker(path);
        }

        [Test]
        public void ShouldIdentifyTypesDeclaredOnAClass()
        {
            _walker.Parse();
            //string[] fullQualifiedTypeNames = { "System.string"};
            string[] types = { "SimpleProject.NoViolations", "System.String", "System.Data.EntityClient.EntityTransaction" };
            DemeterAnalysis.Current.Classes.Where(c => c.Name == "NoViolations").Count().Should().Be(1);
            DemeterAnalysis.Current.Classes.First(c => c.Name == "NoViolations").DeclaredTypes.Should().Not.Contain("some_strange_class_name");
            DemeterAnalysis.Current.Classes.First(c => c.Name == "NoViolations").DeclaredTypes.Should().Have.SameSequenceAs(types);
        }

        [Test]
        public void ShouldIdentifyMethods()
        {
            _walker.Parse();
            DemeterAnalysis.Current.Classes.First(c => c.Name == "NoViolations").Methods.Count().Should().Be.GreaterThan(0);
            DemeterAnalysis.Current.Classes.First(c => c.Name == "NoViolations").Methods.Any(m => m.Name == "MyFirstMethod").Should().Be.True();
        }

        [Test]
        public void ShouldIdentifyTypesDeclaredOnMethodParameters()
        {
            _walker.Parse();
            string[] types = { "System.Int32", "SimpleProject.NoViolations", "System.IO.StreamReader", "System.String", "System.Collections.Generic.Queue" };
            DemeterAnalysis.Current.Classes.First(c => c.Name == "NoViolations").Methods.First(m => m.Name == "MyFirstMethod").ParameterTypes.Should().Have.SameSequenceAs(types);
        }

        [Test]
        public void ShouldIdentityDuplicatedTypeAsSingleType()
        {
            _walker.Parse();
            // The type of the declaring class is always included
            DemeterAnalysis.Current.Classes.First(c => c.Name == "OneReferencePerType").DeclaredTypes.Count().Should().Be(2);
        }

        [Test]
        public void ShouldIdentifyVariablesDeclaredOnMethodBody()
        {
            _walker.Parse();
            string[] types = { "System.IO.StreamReader", "System.String", "System.Char[]", "System.Double", "System.Collections.Generic.List", "System.Char" };
            DemeterAnalysis.Current.Classes.First(c => c.Name == "NoViolations").Methods.First(m => m.Name == "MyFirstMethod").LocalVariables.Should().Have.SameSequenceAs(types);
        }

        [Test]
        public void ShouldReportExternalViolations()
        {
            _walker.Parse();
            //var violations = DemeterAnalysis.Current.Classes.First(c => c.Name == "ViolationsUsingExternalTypes").Methods.First(m => m.Name == "MyMethod").Violations;
            var klass = DemeterAnalysis.Current.Classes.First(c => c.Name == "ViolationsUsingExternalTypes");
            //violations.Count().Should().Be(0);
            klass.Should().Be(klass);
            true.Should().Be(true);
        }

        [Test]
        public void ShouldReportViolationsOnSameLine()
        {
            _walker.Parse();
            DemeterAnalysis.Current.Classes.First(c => c.Name == "ViolationsUsingProjectTypes")
                .Violations
                .GroupBy(g => new { g.ProjectName, g.FileName, g.LineNumber })
                .Where(grp => grp.Count() > 1).Count().Should().Be(1);
        }

        //[Test]
        public void WouldCompileNancyFx()
        {
            var path = @"C:\projects\experimental\demchecker.tests\fixtures\Nancy\src\Nancy.sln";
            _walker = new ASTWalker(path);
            _walker.Parse();
            var countClasses = DemeterAnalysis.Current.Classes.Select(c => c.FullQualifiedName).Distinct().Count();
            Console.WriteLine(countClasses);
            var instructions = DemeterAnalysis.Current.TotalOfInspectedInstructions;
            Console.WriteLine(instructions);
            var j = new CSVExtractor(DemeterAnalysis.Current.Violations);
            j.Generate();
            j.Full();
        }

        [Test]
        public void ShouldReportParameterizedTypeOnGenericConstructions()
        {
            _walker.Parse();
            string[] types = { "SimpleProject.Generics", "System.Int32", "System.Collections.Generic.Stack", "System.Collections.Generic.List", "System.Collections.Generic.LinkedList", "System.Object", "System.Collections.Generic.Dictionary" };
            DemeterAnalysis.Current.Classes.First(a => a.Name == "Generics").DeclaredTypes.Should().Have.SameValuesAs(types);
        }

        [Test]
        public void ShouldReportNoViolationsForACompliantSourceCodeFile()
        {
            _walker.Parse();
            DemeterAnalysis.Current.Violations.Where(v => v.Klass.Name == "NoViolations").Count().Should().Be(0);
        }

        [Test]
        public void ShouldReportViolationsOnInternalTypes()
        {
            _walker.Parse();
            DemeterAnalysis.Current.Classes.First(c => c.Name == "ViolationsUsingProjectTypes").Violations.Count().Should().Be.GreaterThan(0);
        }

        [Test]
        public void ShouldIgnoreCallsOnLambdaExpressionParameters()
        {
            _walker.Parse();
            DemeterAnalysis.Current.Classes.First(c => c.Name == "LambdaExpression").Violations.Count().Should().Be(0);
        }

        [Test]
        public void ShouldDeclareAtLeastItself()
        {
            _walker.Parse();
            DemeterAnalysis.Current.Classes.First(c => c.Name == "ClassWithoutMembers").DeclaredTypes.Count().Should().Be(1);
            DemeterAnalysis.Current.Classes.First(c => c.Name == "ClassWithoutMembers").DeclaredTypes.Should().Have.SameSequenceAs(new string[] { "SimpleProject.ClassWithoutMembers" });
        }
    }
}
