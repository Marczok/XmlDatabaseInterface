using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using XMLDatabaseInterface.Core.DomainTypes;
using XmlDataProvider = XMLDatabaseInterface.Core.XmlDataProvider;

namespace XMLDatabaseInterface.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const string DataPath = "Resources/data.xml";
        private int _databaseSize = 3000;
        private ObservableCollection<Person> _persons;

        public MainWindowViewModel()
        {
            GenerateDataCommand = new RelayCommand(async () =>
            {
                await Task.Run(() =>
                {
                    var data = XmlDataProvider.GenerateDatabase(DatabaseSize);
                    XmlDataProvider.WriteDatabase(data, DataPath);
                }).ConfigureAwait(true);
                await LoadDataAsync().ConfigureAwait(false);
            }, () => 0 < DatabaseSize && DatabaseSize < 1000000);

            LoadDataCommand = new RelayCommand(async () =>
                    await LoadDataAsync().ConfigureAwait(false),
                () => File.Exists(DataPath));
        }

        public RelayCommand GenerateDataCommand { get; }
        public RelayCommand LoadDataCommand { get; }

        public int DatabaseSize
        {
            get => _databaseSize;
            set => Set(() => DatabaseSize, ref _databaseSize, value);
        }

        public ObservableCollection<Person> Persons
        {
            get => _persons;
            private set => Set(() => Persons, ref _persons, value);
        }

        private async Task LoadDataAsync()
        {
            IEnumerable<Person> data = null;
            await Task.Run(() => { data = XmlDataProvider.ReadDatabase(DataPath); }).ConfigureAwait(true);
            Persons = new ObservableCollection<Person>(data);
        }
    }
}