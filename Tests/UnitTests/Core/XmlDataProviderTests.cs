using System;
using System.IO;
using System.Linq;
using XMLDatabaseInterface.Core;
using Xunit;

namespace Tests.UnitTests.Core
{
    public class XmlDataProviderTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(99)]
        public void ProviderGenerateDataProperly(int index)
        {
            const int size = 100;
            var data = XmlDataProvider.GenerateDatabase(size).ToList();
            Assert.Equal(size, data.Count);
            var item = data[index];
            Assert.NotEmpty(item.Name);
            Assert.NotEmpty(item.Surename);
            Assert.NotEmpty(item.Address);
            Assert.NotEmpty(item.Birthdate.ToShortDateString());
        }

        [Fact]
        public void GeneratedDataCouldBeSaved()
        {
            const int size = 100000;
            const string filename = "data.xml";
            var data = XmlDataProvider.GenerateDatabase(size);
            XmlDataProvider.WriteDatabase(data, filename);
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
            var data = XmlDataProvider.GenerateDatabase(size).ToList();
            var generatedItem = data[index];
            XmlDataProvider.WriteDatabase(data, filename);
            Assert.True(File.Exists(filename));

            data = XmlDataProvider.LoadDatabase(filename).ToList();
            Assert.Equal(size, data.Count);
            var deserializedItem = data[index];
            Assert.NotEmpty(deserializedItem.Name);
            Assert.NotEmpty(deserializedItem.Surename);
            Assert.NotEmpty(deserializedItem.Address);
            Assert.NotEmpty(deserializedItem.Birthdate.ToShortDateString());
            Assert.Equal(generatedItem, deserializedItem);
            
            File.Delete(filename);
        }
    }
}
