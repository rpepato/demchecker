using demchecker.Parsers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpTestsEx;
using SharpTestsEx.Assertions;

namespace demchecker.tests.Parsers
{
    [TestFixture]
    class ParserFactoryTests
    {
        [Test]
        public void ShouldReturnALoDParser()
        {
            ParserFactory.CreateParser(ParserType.LoD).Should().Be.OfType<LoDParser>();
        }

        [Test]
        public void ShouldReturnALoDEParser()
        {
            ParserFactory.CreateParser(ParserType.LoDE).Should().Be.OfType<LoDEParser>();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRaiseExceptionForAnUnknownParserType()
        {
            ParserFactory.CreateParser((ParserType)2);
        }
    }
}
