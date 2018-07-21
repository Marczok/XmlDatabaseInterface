using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Xceed.Wpf.Toolkit;
using XMLDatabaseInterface.Core.DomainTypes;
using XmlDataProvider = XMLDatabaseInterface.Core.XmlDataProvider;

namespace XMLDatabaseInterface.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {

        private int _databaseSize = 3000;
        private ObservableCollection<Person> _persons;
        private WindowState _dataSourceWindowState = WindowState.Open;

        public MainWindowViewModel()
        {
            GenerateDataCommand = new RelayCommand(async () =>
            {
                await Task.Run(() =>
                {
                    var data = XmlDataProvider.GenerateDatabase(DatabaseSize, new Progress<double>(Console.WriteLine));
                    XmlDataProvider.WriteDatabase(data, DataPath);
                }).ConfigureAwait(true);
                await LoadDataAsync().ConfigureAwait(false);
                DataSourceWindowState = WindowState.Closed;
            }, () => 0 < DatabaseSize && DatabaseSize < 1000000);

            LoadDataCommand = new RelayCommand(async () =>
            {
                await LoadDataAsync().ConfigureAwait(false);
                DataSourceWindowState = WindowState.Closed;
            }, () => File.Exists(DataPath));
        }

        public string DataPath { get; } = $"Resources{Path.PathSeparator}DataSources{Path.PathSeparator}data.xml";

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

        public WindowState DataSourceWindowState
        {
            get => _dataSourceWindowState;
            set => Set(() => DataSourceWindowState, ref _dataSourceWindowState, value);
        }

        private async Task LoadDataAsync()
        {
            IEnumerable<Person> data = null;
            await Task.Run(() => { data = XmlDataProvider.ReadDatabase(DataPath); }).ConfigureAwait(true);
            Persons = new ObservableCollection<Person>(data);
        }
    }
}