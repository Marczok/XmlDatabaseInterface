using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using XMLDatabaseInterface.Core;
using XMLDatabaseInterface.Core.DomainTypes;
using XMLDatabaseInterface.ViewModel;
using Xunit;

namespace Tests.UnitTests.ViewModels
{
    public class CommonNamesViewModelTests
    {
        private readonly CommonNamesViewModel _vm;
        private readonly IDataProvider _provider;
        public CommonNamesViewModelTests()
        {
            _provider = new XmlDataProvider(); // TODO: Mock this provider
            _vm = new CommonNamesViewModel(_provider);
        }

        [Fact]
        public void InitializationIsCorrect()
        {
            Assert.Null(_vm.CommonNames);
        }

        [Fact]
        public async void CommmonNamesDisplayedProperly()
        {
            const string path = "test.xml";
            var testCollection = new List<Person>
            {
                new Person("x", "x", "x", DateTime.Today),
                new Person("x", "x", "x", DateTime.Today),
                new Person("x", "x", "x", DateTime.Today),
                new Person("y", "y", "y", DateTime.Today),
                new Person("y", "y", "y", DateTime.Today),
                new Person("z", "z", "z", DateTime.Today),
                new Person("z", "z", "z", DateTime.Today),
                new Person("z", "z", "z", DateTime.Today),
                new Person("z", "z", "z", DateTime.Today),
                new Person("z", "z", "z", DateTime.Today)
            };

            _provider.SaveDatabase(testCollection, path);
            await Task.Delay(1000).ConfigureAwait(true); // TODO: Wait for exact time of refreshing
            Assert.NotNull(_vm.CommonNames);
            Assert.Equal(3, _vm.CommonNames.Count);
            Assert.Equal("z", _vm.CommonNames[0].Name);
            Assert.Equal(5, _vm.CommonNames[0].Count);
            Assert.Equal("x", _vm.CommonNames[1].Name);
            Assert.Equal(3, _vm.CommonNames[1].Count);
            Assert.Equal("y", _vm.CommonNames[2].Name);
            Assert.Equal(2, _vm.CommonNames[2].Count);

            File.Delete(path);
        }

        [Fact]
        public async void JustTenNamesIsDisplayed()
        {
            const string path = "test.xml";
            var testCollection = new List<Person>
            {
                new Person("1", "x", "x", DateTime.Today),
                new Person("2", "x", "x", DateTime.Today),
                new Person("3", "x", "x", DateTime.Today),
                new Person("4", "y", "y", DateTime.Today),
                new Person("5", "y", "y", DateTime.Today),
                new Person("6", "z", "z", DateTime.Today),
                new Person("7", "z", "z", DateTime.Today),
                new Person("8", "z", "z", DateTime.Today),
                new Person("9", "z", "z", DateTime.Today),
                new Person("10", "z", "z", DateTime.Today),
                new Person("11", "z", "z", DateTime.Today)
            };

            _provider.SaveDatabase(testCollection, path);
            await Task.Delay(1000).ConfigureAwait(true); // TODO: Wait for exact time of refreshing
            Assert.NotNull(_vm.CommonNames);
            Assert.Equal(10, _vm.CommonNames.Count);

            File.Delete(path);

        }
    }
}
