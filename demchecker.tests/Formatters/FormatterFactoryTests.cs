using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpTestsEx;
using demchecker.Formatters;

namespace demchecker.tests.Formatters
{
    [TestFixture]
    class FormatterFactoryTests
    {
        [Test]
        public void ShouldReturnACSVFormatter()
        {
            FormatterFactory.CreateFormatter(FormatterType.CSV, ';').Should().Be.OfType<CSVFormatter>();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRaiseExceptionForAnUnknownFormatterType()
        {
            FormatterFactory.CreateFormatter((FormatterType)2, ';');
        }
    }
}
