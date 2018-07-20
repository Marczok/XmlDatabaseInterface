using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLDatabaseInterface.Core;
using Xunit;

namespace Tests.UnitTests.Core
{
    public class XmlGeneratorTests
    {
        [Fact]
        public void GeneratorGenerateDataProperly()
        {
            const string filename = "data.xml";
            const int size = 100;
            XmlGenerator.GenerateDatabase(size, filename);
            Assert.True(File.Exists(filename));
            File.Delete(filename);
        }
    }
}
