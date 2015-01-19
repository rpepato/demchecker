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
