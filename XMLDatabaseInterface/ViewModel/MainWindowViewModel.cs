using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Xceed.Wpf.Toolkit;
using XMLDatabaseInterface.Core.DomainTypes;
using XMLDatabaseInterface.Properties;
using XmlDataProvider = XMLDatabaseInterface.Core.XmlDataProvider;

namespace XMLDatabaseInterface.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private int _databaseSize = 5000;
        private ObservableCollection<Person> _persons;
        private WindowState _dataSourceWindowState = WindowState.Open;
        private double _progress;
        private WindowState _progressWindowState;
        private string _progressMessage;

        public MainWindowViewModel()
        {
            GenerateDataCommand = new RelayCommand(async () =>
            {
                DataSourceWindowState = WindowState.Closed;
                ProgressMessage = Resources.GeneratingData;
                ProgressWindowState = WindowState.Open;
                await Task.Run(() =>
                {
                    var data = XmlDataProvider.GenerateDatabase(DatabaseSize,
                        new Progress<double>(progress => Progress = progress));
                    XmlDataProvider.WriteDatabase(data, DataPath);
                }).ConfigureAwait(true);
                ProgressWindowState = WindowState.Closed;
                await LoadDataAsync().ConfigureAwait(false);

            }, () => 0 < DatabaseSize && DatabaseSize <= 500000);

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

        public double Progress
        {
            get => _progress;
            set => Set(() => Progress, ref _progress, value);
        }

        public string ProgressMessage
        {
            get => _progressMessage;
            set => Set(() => ProgressMessage, ref _progressMessage, value);
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

        public WindowState ProgressWindowState
        {
            get => _progressWindowState;
            set => Set(() => ProgressWindowState, ref _progressWindowState, value);
        }

        private async Task LoadDataAsync()
        {
            IEnumerable<Person> data = null;
            await Task.Run(() => { data = XmlDataProvider.ReadDatabase(DataPath); }).ConfigureAwait(true);
            Persons = new ObservableCollection<Person>(data);
        }
    }
}