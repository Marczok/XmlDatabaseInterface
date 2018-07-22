using System.IO;
using System.Threading.Tasks;
using Moq;
using XMLDatabaseInterface.Core;
using XMLDatabaseInterface.ViewModel;
using Xunit;

namespace Tests.UnitTests.ViewModels
{
    public class MainWindowViewModelTests
    {
        private readonly MainWindowViewModel _vm;
        private Mock<IDataProvider> _mock;
        private IDataProvider _provider;

        public MainWindowViewModelTests()
        {
            _mock = new Mock<IDataProvider>();
            _provider = _mock.Object;
            _vm = new MainWindowViewModel(_provider);
        }

        [Fact]
        public void InitializationIsCorrect()
        {
            Assert.NotNull(_vm.GenerateDataCommand);
            Assert.NotNull(_vm.LoadDataCommand);
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
    }
}
