using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpTestsEx;
using demchecker.Parsers;
using demchecker.domain;

namespace demchecker.tests.Parsers
{
    [TestFixture]
    class LoDParserTests
    {
        private IParser _parser;
        private string _solutionPath;

        [SetUp]
        public void SetUp()
        {
            _solutionPath = @"C:\projects\experimental\demchecker.tests\fixtures\SimpleSolution\SimpleSolution.sln";
        }

        [Test]
        public void ShouldCollectClassPropertiesAsRegisteredTypes()
        {
            _parser = new LoDParser();
            _parser.Parse(_solutionPath);
            DemeterAnalysis.Current.Classes.First(c => c.Name == "ClassWithProperties").DeclaredTypes.Should().Contain("System.String").And.Contain("System.Int32");
        }

        [Test]
        public void ShouldNotReportViolationsOnStaticMethodCalls()
        {
            _parser = new LoDParser();
            _parser.Parse(_solutionPath);
            DemeterAnalysis.Current.Classes.First(c => c.Name == "ClassWithACallOnStaticMethod").Violations.Count().Should().Be(0);
        }
    }
}
