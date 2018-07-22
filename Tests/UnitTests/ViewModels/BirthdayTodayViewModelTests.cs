using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XMLDatabaseInterface.Core;
using XMLDatabaseInterface.Core.DomainTypes;
using XMLDatabaseInterface.ViewModel;
using Xunit;

namespace Tests.UnitTests.ViewModels
{
    public class BirthdayTodayViewModelTests
    {
        private readonly BirthdayTodayViewModel _vm;
        private readonly IDataProvider _provider;
        public BirthdayTodayViewModelTests()
        {
            _provider = new XmlDataProvider(); // TODO: Mock this provider
            _vm = new BirthdayTodayViewModel(_provider);
        }

        [Fact]
        public void InitializationIsCorrect()
        {
            Assert.Null(_vm.BirthdayCollection);
        }

        [Fact]
        public async void BirthdayDisplayedProperly()
        {
            const string path = "test.xml";
            var testCollection = new List<Person>
            {
                new Person("x", "x", "x", DateTime.Today),
                new Person("y", "y", "y", DateTime.Today),
                new Person("z", "z", "z", DateTime.MaxValue)
            };

            _provider.SaveDatabase(testCollection, path);
            await Task.Delay(1000).ConfigureAwait(true); // TODO: Wait for exact time of refreshing
            Assert.NotNull(_vm.BirthdayCollection);
            Assert.Equal(2, _vm.BirthdayCollection.Count);
            Assert.Equal("x", _vm.BirthdayCollection[0].Name);
            Assert.Equal("x", _vm.BirthdayCollection[0].Surename);
            Assert.Equal("y", _vm.BirthdayCollection[1].Name);
            Assert.Equal("y", _vm.BirthdayCollection[1].Surename);
        }
    }
}
