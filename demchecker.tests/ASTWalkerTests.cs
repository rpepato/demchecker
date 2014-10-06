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
            string[] fullQualifiedTypeNames = { "System.string"};
            string[] types = { "System.String", "System.Data.EntityClient.EntityTransaction" };
            DemeterAnalysis.Current.Classes.Where(c => c.Name == "Class1").Count().Should().Be(1);
            DemeterAnalysis.Current.Classes.First(c => c.Name == "Class1").DeclaredTypes.Should().Not.Contain("some_strange_class_name");
            DemeterAnalysis.Current.Classes.First(c => c.Name == "Class1").DeclaredTypes.Should().Have.SameSequenceAs(types);
        }

        [Test]
        public void ShouldIdentifyMethods()
        {
            _walker.Parse();
            DemeterAnalysis.Current.Classes.First(c => c.Name == "Class1").Methods.Count().Should().Be(1);
            DemeterAnalysis.Current.Classes.First(c => c.Name == "Class1").Methods.Any(m => m.Name == "MyFirstMethod").Should().Be.True();
        }

        [Test]
        public void ShouldIdentifyTypesDeclaredOnMethodParameters()
        {
            _walker.Parse();
            string[] types = { "System.Int32", "SimpleProject.Class1", "System.IO.StreamReader", "System.String", "System.Collections.Generic.Queue" };
            DemeterAnalysis.Current.Classes.First(c => c.Name == "Class1").Methods.First(m => m.Name == "MyFirstMethod").ParameterTypes.Should().Have.SameSequenceAs(types);
        }

        [Test]
        public void ShouldIdentityDuplicatedTypeAsSingleType()
        {
            _walker.Parse();
            DemeterAnalysis.Current.Classes.First(c => c.Name == "OneReferencePerType").DeclaredTypes.Count().Should().Be(1);
        }

        [Test]
        public void ShouldIdentifyVariablesDeclaredOnMethodBody()
        {
            _walker.Parse();
            string[] types = { "System.IO.StreamReader", "System.String", "System.Char[]", "System.Double", "System.Collections.Generic.List" };
            DemeterAnalysis.Current.Classes.First(c => c.Name == "Class1").Methods.First(m => m.Name == "MyFirstMethod").LocalVariables.Should().Have.SameSequenceAs(types);
        }

        [Test]
        public void ShouldReportExternalViolations()
        {
            _walker.Parse();
            DemeterAnalysis.Current.Violations.Where(v => v.Klass.Name == "ViolationsUsingExternalTypes" && v.Method.Name == "MyMethod").Count().Should().Be(3);
        }

        [Test]
        public void ShouldReportViolationsOnSameLine()
        {
            _walker.Parse();
            DemeterAnalysis.Current.Violations
                .GroupBy(g => new { g.ProjectName, g.FileName, g.LineNumber })
                .Where(grp => grp.Count() > 1).Count().Should().Be(1);
        }

        [Test]
        public void WouldCompileNancyFx()
        {
            var path = @"C:\projects\experimental\demchecker.tests\fixtures\Nancy\src\Nancy.sln";
            _walker = new ASTWalker(path);
            _walker.Parse();
            DemeterAnalysis.Current.Violations.Count().Should().Be.GreaterThan(0);
        }

        [Test]
        public void ShouldReportParameterizedTypeOnGenericConstructions()
        {
            _walker.Parse();
            string[] types = { "System.Int32", "System.Collections.Generic.Stack", "System.Collections.Generic.List", "System.Collections.Generic.LinkedList", "System.Object", "System.Collections.Generic.Dictionary" };
            DemeterAnalysis.Current.Classes.First(a => a.Name == "Generics").DeclaredTypes.Should().Have.SameValuesAs(types);
        }

        [Test]
        public void ShouldReportNoViolationsForACompliantSourceCodeFile()
        {
            _walker.Parse();
            DemeterAnalysis.Current.Violations.Where(v => v.Klass.Name == "Class1").Count().Should().Be(0);
        }
    }
}
