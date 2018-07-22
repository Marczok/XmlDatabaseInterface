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
    public class CommonSurenamesViewModelTests
    {
        private readonly CommonSurenamesViewModel _vm;
        private readonly IDataProvider _provider;
        public CommonSurenamesViewModelTests()
        {
            _provider = new XmlDataProvider(); // TODO: Mock this provider
            _vm = new CommonSurenamesViewModel(_provider);
        }

        [Fact]
        public void InitializationIsCorrect()
        {
            Assert.Null(_vm.CommonSurenames);
        }

        [Fact]
        public async void CommmonSurenamesDisplayedProperly()
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
            Assert.NotNull(_vm.CommonSurenames);
            Assert.Equal(3, _vm.CommonSurenames.Count);
            Assert.Equal("z", _vm.CommonSurenames[0].Surename);
            Assert.Equal(5, _vm.CommonSurenames[0].Count);
            Assert.Equal("x", _vm.CommonSurenames[1].Surename);
            Assert.Equal(3, _vm.CommonSurenames[1].Count);
            Assert.Equal("y", _vm.CommonSurenames[2].Surename);
            Assert.Equal(2, _vm.CommonSurenames[2].Count);

            File.Delete(path);
        }

        [Fact]
        public async void JustTenSurenamesIsDisplayed()
        {
            const string path = "test.xml";
            var testCollection = new List<Person>
            {
                new Person("1", "1", "x", DateTime.Today),
                new Person("2", "2", "x", DateTime.Today),
                new Person("3", "3", "x", DateTime.Today),
                new Person("4", "4", "y", DateTime.Today),
                new Person("5", "5", "y", DateTime.Today),
                new Person("6", "6", "z", DateTime.Today),
                new Person("7", "7", "z", DateTime.Today),
                new Person("8", "8", "z", DateTime.Today),
                new Person("9", "9", "z", DateTime.Today),
                new Person("10", "10", "z", DateTime.Today),
                new Person("11", "11", "z", DateTime.Today)
            };

            _provider.SaveDatabase(testCollection, path);
            await Task.Delay(1000).ConfigureAwait(true); // TODO: Wait for exact time of refreshing
            Assert.NotNull(_vm.CommonSurenames);
            Assert.Equal(10, _vm.CommonSurenames.Count);

            File.Delete(path);

        }
    }
}
