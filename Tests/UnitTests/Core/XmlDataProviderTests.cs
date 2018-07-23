using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using XMLDatabaseInterface.Core;
using XMLDatabaseInterface.Core.DomainTypes;
using Xunit;

namespace Tests.UnitTests.Core
{
    public class XmlDataProviderTests
    {
        private readonly XmlDataProvider _provider;
        public XmlDataProviderTests()
        {
            _provider = new XmlDataProvider();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(99)]
        public void ProviderGenerateDataProperly(int index)
        {
            const int size = 100;
            var data = _provider.GenerateDatabase(size).ToList();
            Assert.Equal(size, data.Count);
            var item = data[index];
            Assert.NotEmpty(item.Name);
            Assert.NotEmpty(item.Surename);
            Assert.NotEmpty(item.Address);
            Assert.NotEmpty(item.Birthdate.ToShortDateString());
        }

        [Fact]
        public void NoDataAreGeneratedWithWrongArgument()
        {
            var result = _provider.GenerateDatabase(0);
            Assert.Null(result);

            result = _provider.GenerateDatabase(-1);
            Assert.Null(result);
        }

        [Fact]
        public void GeneratedDataCouldBeSaved()
        {
            const int size = 100000;
            const string filename = "data.xml";
            var data = _provider.GenerateDatabase(size);
            var success = _provider.SaveDatabase(data, filename);
            Assert.True(success);
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
            var data = _provider.GenerateDatabase(size).ToList();
            var generatedItem = data[index];
            _provider.SaveDatabase(data, filename);
            Assert.True(File.Exists(filename));

            var success = _provider.LoadDatabase(filename);
            Assert.True(success);
            data = _provider.Database.ToList();
            Assert.Equal(size, data.Count);
            var deserializedItem = data[index];
            Assert.NotEmpty(deserializedItem.Name);
            Assert.NotEmpty(deserializedItem.Surename);
            Assert.NotEmpty(deserializedItem.Address);
            Assert.NotEmpty(deserializedItem.Birthdate.ToShortDateString());
            Assert.Equal(generatedItem, deserializedItem);

            File.Delete(filename);
        }

        [Fact]
        public void SaveDataFailWithNoData()
        {
            var success = _provider.SaveDatabase(null, "a");
            Assert.False(success);
        }

        [Fact]
        public void SaveDataFailsWithWrongPath()
        {
            var success = _provider.SaveDatabase(new List<Person>(), null);
            Assert.False(success);
            success = _provider.SaveDatabase(new List<Person>(), "");
            Assert.False(success);
        }

        [Fact]
        public void SaveDataRaisePropertyChanged()
        {
            PropertyChangedEventArgs testArgs = null;
            var counter = 0;
            const string filename = "test6.xml";

            _provider.PropertyChanged += (sender, args) =>
            {
                testArgs = args;
                counter++;
            };

            _provider.SaveDatabase(new List<Person>(), filename);
            Assert.NotNull(testArgs);
            Assert.Equal(nameof(_provider.Database), testArgs.PropertyName);
            Assert.Equal(1, counter);

            testArgs = null;
            _provider.SaveDatabase(null, null);
            Assert.Null(testArgs);
            Assert.Equal(1, counter);

            _provider.SaveDatabase(new List<Person>(), filename);
            Assert.NotNull(testArgs);
            Assert.Equal(nameof(_provider.Database), testArgs.PropertyName);
            Assert.Equal(2, counter);

            File.Delete(filename);
        }

        [Fact]
        public void LoadDatabaseFailsWithWrongName()
        {
            var success = _provider.LoadDatabase(null);
            Assert.False(success);

            success = _provider.LoadDatabase("");
            Assert.False(success);
        }

        [Fact]
        public void LoadDatabaseFailsWithoutData()
        {
            var success = _provider.LoadDatabase("random/path/without/data.xml");
            Assert.False(success);
        }

        [Fact]
        public void LoadDataRaisePropertyChanged()
        {
            PropertyChangedEventArgs testArgs = null;
            var counter = 0;
            const string filename = "test7.xml";
            _provider.SaveDatabase(new List<Person>(), filename);

            _provider.PropertyChanged += (sender, args) =>
            {
                testArgs = args;
                counter++;
            };

            _provider.LoadDatabase(filename);
            Assert.NotNull(testArgs);
            Assert.Equal(nameof(_provider.Database), testArgs.PropertyName);
            Assert.Equal(1, counter);

            testArgs = null;
            _provider.LoadDatabase(null);
            Assert.Null(testArgs);
            Assert.Equal(1, counter);

            _provider.LoadDatabase(filename);
            Assert.NotNull(testArgs);
            Assert.Equal(nameof(_provider.Database), testArgs.PropertyName);
            Assert.Equal(2, counter);

            File.Delete(filename);
        }
    }
}
