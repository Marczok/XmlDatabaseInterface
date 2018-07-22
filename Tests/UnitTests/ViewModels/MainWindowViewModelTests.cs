using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xceed.Wpf.Toolkit;
using XMLDatabaseInterface.Core;
using XMLDatabaseInterface.Core.DomainTypes;
using XMLDatabaseInterface.ViewModel;
using Xunit;

namespace Tests.UnitTests.ViewModels
{
    public class MainWindowViewModelTests
    {
        private readonly MainWindowViewModel _vm;
        private readonly IDataProvider _provider;

        public MainWindowViewModelTests()
        {
            _provider = new XmlDataProvider(); // TODO: Use Mock instead of real implementation
            _vm = new MainWindowViewModel(_provider);
        }

        [Fact]
        public void InitializationIsCorrect()
        {
            Assert.NotNull(_vm.GenerateDataCommand);
            Assert.NotNull(_vm.LoadDataCommand);
            Assert.NotNull(_vm.SaveDataCommand);
            Assert.NotNull(_vm.AddPersonCommand);
            Assert.NotNull(_vm.ClearAddPersonDataCommand);
            Assert.NotNull(_vm.OpenAddWindowCommand);
            Assert.Null(_vm.AddName);
            Assert.Null(_vm.AddSurename);
            Assert.Null(_vm.AddAddress);
            Assert.Equal(DateTime.Today, _vm.AddDate.Date);
            Assert.False(File.Exists(_vm.DataPath));
            Assert.Null(_vm.Database);
            Assert.Equal(WindowState.Closed, _vm.AddWindowState);
            Assert.Equal(WindowState.Closed, _vm.ProgressWindowState);
            Assert.Equal(WindowState.Open, _vm.DataSourceWindowState);
        }

        [Fact]
        public async void GenerateCommandGeneratesDataSourceCollection()
        {
            const int size = 100;
            _vm.DatabaseSize = size;
            _vm.GenerateDataCommand.Execute(null);
            await Task.Delay(5000).ConfigureAwait(true); //TODO: Wait for exact time of executing the command
            Assert.True(File.Exists(_vm.DataPath));
            Assert.NotNull(_vm.Database);
            Assert.Equal(size, _vm.Database.Count);
            File.Delete(_vm.DataPath);
        }

        [Fact]
        public void GenerateDataCommandCanBeExecutedJustWithMeaningfullSize()
        {
            _vm.DatabaseSize = 0;
            Assert.False(_vm.GenerateDataCommand.CanExecute(null));
            _vm.DatabaseSize = -1;
            Assert.False(_vm.GenerateDataCommand.CanExecute(null));
            _vm.DatabaseSize = int.MaxValue;
            Assert.False(_vm.GenerateDataCommand.CanExecute(null));
            _vm.DatabaseSize = 100;
            Assert.True(_vm.GenerateDataCommand.CanExecute(null));
        }

        [Fact]
        public void LoadDataCommandCannotBeExecutedWithoutData()
        {
            Assert.False(File.Exists(_vm.DataPath));
            Assert.False(_vm.LoadDataCommand.CanExecute(null));
        }

        [Fact]
        public async void LoadDataCommandLoadsDataProperly()
        {
            const int size = 100;
            var data = _provider.GenerateDatabase(size);
            _provider.SaveDatabase(data, _vm.DataPath);
            Assert.True(_vm.LoadDataCommand.CanExecute(null));
            _vm.LoadDataCommand.Execute(null);
            await Task.Delay(5000).ConfigureAwait(true); //TODO: Wait for exact time of executing the command
            Assert.NotNull(_vm.Database);
            Assert.Equal(size, _vm.Database.Count);
            File.Delete(_vm.DataPath);
        }

        [Fact]
        public async void SaveDataCommandSavesChanges()
        {
            const int size = 3;
            var data = _provider.GenerateDatabase(size);
            _provider.SaveDatabase(data, _vm.DataPath);
            _vm.LoadDataCommand.Execute(null);
            await Task.Delay(1000).ConfigureAwait(true); //TODO: Wait for exact time of executing the command
            File.Delete(_vm.DataPath);
            _vm.Database.Remove(_vm.Database[0]);
            _vm.Database.Remove(_vm.Database[0]);
            var newPerson = new Person("Adam", "Smith", "Street", DateTime.Today);
            _vm.Database.Add(newPerson);
            _vm.SaveDataCommand.Execute(null);
            await Task.Delay(1000).ConfigureAwait(true); //TODO: Wait for exact time of executing the command
            Assert.Equal(2, _provider.Database.Count());
            Assert.Contains(newPerson, _provider.Database);
            Assert.True(File.Exists(_vm.DataPath));
            File.Delete(_vm.DataPath);
        }

        [Fact]
        public async void DeleteCommandDeleteSelectedPerson()
        {
            const int size = 10;
            var data = _provider.GenerateDatabase(size);
            _provider.SaveDatabase(data, _vm.DataPath);
            _vm.LoadDataCommand.Execute(null);
            await Task.Delay(1000).ConfigureAwait(true); //TODO: Wait for exact time of executing the command
            Assert.Equal(data[0], _vm.Database[0]);
            var deleted = new List<Person>
            {
                data[0],
                data[1]
            };

            Assert.True(_vm.DeletePersonCommand.CanExecute(deleted));
            _vm.DeletePersonCommand.Execute(deleted);
            Assert.DoesNotContain(deleted[0], _vm.Database);
            Assert.DoesNotContain(deleted[1], _vm.Database);
            File.Delete(_vm.DataPath);
        }
    }
}