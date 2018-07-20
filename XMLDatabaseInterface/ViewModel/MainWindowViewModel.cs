using System;
using System.Collections.ObjectModel;
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
            GenerateData = new RelayCommand(() =>
            {
                var data = XmlDataProvider.GenerateDatabase(DatabaseSize);
                XmlDataProvider.WriteDatabase(data, DataPath);
                LoadData.Execute(null);
            }, () => 0 < DatabaseSize && DatabaseSize < 1000000);

            LoadData = new RelayCommand(() => { Persons = new ObservableCollection<Person>(XmlDataProvider.ReadDatabase(DataPath)); });
        }

        public RelayCommand GenerateData { get; }
        public RelayCommand LoadData { get; }
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
    }
}