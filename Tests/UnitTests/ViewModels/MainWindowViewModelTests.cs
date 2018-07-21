using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLDatabaseInterface.Core;
using XMLDatabaseInterface.ViewModel;
using Xunit;

namespace Tests.UnitTests.ViewModels
{
    public class MainWindowViewModelTests
    {
        private readonly MainWindowViewModel _vm;

        public MainWindowViewModelTests()
        {
            _vm = new MainWindowViewModel();
        }

        [Fact]
        public void InitializationIsCorrect()
        {
            Assert.NotNull(_vm.GenerateDataCommand);
            Assert.NotNull(_vm.LoadDataCommand);
            Assert.NotNull(_vm.ProcessBirthdayCommand);
            Assert.NotNull(_vm.ProcessCommonNamesCommand);
            Assert.NotNull(_vm.ProcessCommonSurenamesCommand);
            Assert.False(File.Exists(_vm.DataPath));
        }

        [Fact]
        public async void GenerateCommandGeneratesDataSourceCollection()
        {
            const int size = 100;
            _vm.DatabaseSize = size;
            _vm.GenerateDataCommand.Execute(null);
            await Task.Delay(5000).ConfigureAwait(true); //TODO: Wait for exact time of executing the command
            Assert.True(File.Exists(_vm.DataPath));
            Assert.NotNull(_vm.Persons);
            Assert.Equal(size, _vm.Persons.Count);
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
            var data = XmlDataProvider.GenerateDatabase(size);
            XmlDataProvider.WriteDatabase(data, _vm.DataPath);
            Assert.True(_vm.LoadDataCommand.CanExecute(null));
            _vm.LoadDataCommand.Execute(null);
            await Task.Delay(5000).ConfigureAwait(true); //TODO: Wait for exact time of executing the command
            Assert.NotNull(_vm.Persons);
            Assert.Equal(size, _vm.Persons.Count);
            File.Delete(_vm.DataPath);
        }
    }
}
