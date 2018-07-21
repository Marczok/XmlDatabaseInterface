using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using System.Windows.Data;
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
            GenerateDataCommand = new RelayCommand(() =>
            {
                var data = XmlDataProvider.GenerateDatabase(DatabaseSize);
                XmlDataProvider.WriteDatabase(data, DataPath);
                LoadData();
            }, () => 0 < DatabaseSize && DatabaseSize < 1000000);

            LoadDataCommand = new RelayCommand(LoadData, () => File.Exists(DataPath));
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

        private void LoadData()
        {
            Persons = new ObservableCollection<Person>(XmlDataProvider.ReadDatabase(DataPath));
        }


    }
}