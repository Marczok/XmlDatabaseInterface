using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(200)]
        [InlineData(499)]
        public void GeneratedDataCouldBeDeserialized(int index)
        {
            const string filename = "data.xml";
            const int size = 500;
            XmlGenerator.GenerateDatabase(size, filename);
            Assert.True(File.Exists(filename));
            var data = XmlGenerator.ReadDatabase(filename);
            Assert.Equal(size, data.Count);
            var item = data[index];
            Assert.NotEmpty(item.Name);
            Assert.NotEmpty(item.Surename);
            Assert.NotEmpty(item.Address);
            Assert.NotEmpty(item.Birthdate);
            File.Delete(filename);
        }
    }
}
