using System;
using System.Collections;
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
    public class MainWindowViewModel : ViewModelBase, IDataProvider
    {
        private int _databaseSize = 5000;
        private ObservableCollection<Person> _persons;
        private WindowState _dataSourceWindowState = WindowState.Open;
        private double _progress;
        private WindowState _progressWindowState;
        private string _progressMessage;

        
        private WindowState _addWindowState;
        private string _addName;
        private string _addSurename;
        private string _addAddress;
        private DateTime _addDate = DateTime.Now;

        public MainWindowViewModel()
        {
            GenerateDataCommand = new RelayCommand(async () =>
            {
                DataSourceWindowState = WindowState.Closed;

                ProgressWindowState = WindowState.Open;

                var data = await GenerateDataAsync().ConfigureAwait(true);
                await SaveDataAsync(data).ConfigureAwait(true);
                data = await LoadDataAsync().ConfigureAwait(true);
                Persons = new ObservableCollection<Person>(data);
                
                ProgressWindowState = WindowState.Closed;
            }, () => MinDatabaseSize < DatabaseSize && DatabaseSize <= MaxDatabaseSize);

            LoadDataCommand = new RelayCommand(async () =>
            {
                DataSourceWindowState = WindowState.Closed;

                ProgressWindowState = WindowState.Open;

                var data = await LoadDataAsync().ConfigureAwait(true);
                Persons = new ObservableCollection<Person>(data);

                ProgressWindowState = WindowState.Closed;
            }, () => File.Exists(DataPath));

            SaveDataCommand = new RelayCommand(async () =>
            {
                ProgressWindowState = WindowState.Open;
                await SaveDataAsync(Persons).ConfigureAwait(true);
                ProgressWindowState = WindowState.Closed;
            }, () => Persons != null && Persons?.Count > 0);

            DeletePersonCommand = new RelayCommand<IList>(selected =>
            {
                // Collection need to be copyed, other way we will change it during iterating, if we delete something, and it will throw exeption
                var remove = new List<Person>(selected.Count);
                foreach (var item in selected)
                {
                    if (item is Person person)
                    {
                        remove.Add(person);
                    }
                }

                foreach (var person in remove)
                {
                    Persons.Remove(person);
                }
            }, selected => selected != null && selected.Count > 0 && Persons != null && Persons.Count > 0);

            OpenAddWindowCommand = new RelayCommand(() => AddWindowState = WindowState.Open);

            AddPersonCommand = new RelayCommand(() =>
            {
                Persons.Add(new Person(AddName, AddSurename, AddAddress, AddDate));
                AddWindowState = WindowState.Closed;
            }, () => !string.IsNullOrEmpty(AddName) && !string.IsNullOrEmpty(AddSurename) && !string.IsNullOrEmpty(AddAddress));

            ClearAddPersonDataCommand = new RelayCommand(() =>
            {
                AddName = null;
                AddSurename = null;
                AddAddress = null;
                AddDate = DateTime.Now;
            });
        }

        public string DataPath { get; } = "Resources/DataSources/data.xml";
        public int MinDatabaseSize { get; } = 0;
        public int MaxDatabaseSize { get; } = 500000;

        public RelayCommand GenerateDataCommand { get; }
        public RelayCommand LoadDataCommand { get; }
        public RelayCommand SaveDataCommand { get; }
        
        public RelayCommand<IList> DeletePersonCommand { get; }
        public RelayCommand OpenAddWindowCommand { get; }
        public RelayCommand AddPersonCommand { get; }
        public RelayCommand ClearAddPersonDataCommand { get; }

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

        public WindowState AddWindowState
        {
            get => _addWindowState;
            set => Set(() => AddWindowState, ref _addWindowState, value);
        }

        public string AddName
        {
            get => _addName;
            set => Set(() => AddName, ref _addName, value);
        }

        public string AddSurename
        {
            get => _addSurename;
            set => Set(() => AddSurename, ref _addSurename, value);
        }

        public string AddAddress
        {
            get => _addAddress;
            set => Set(() => AddAddress, ref _addAddress, value);
        }

        public DateTime AddDate
        {
            get => _addDate;
            set => Set(() => AddDate, ref _addDate, value);
        }

        private Task<IEnumerable<Person>> GenerateDataAsync()
        {
            ProgressMessage = Resources.GeneratingData;
            return Task.Run(() =>
            {
                return XmlDataProvider.GenerateDatabase(
                    DatabaseSize,
                    new Progress<double>(progress => Progress = progress)
                );
            });
        }

        private Task SaveDataAsync(IEnumerable<Person> data)
        {
            ProgressMessage = Resources.SavingData;
            return Task.Run(() =>
            {
                ProgressMessage = Resources.SavingData;
                XmlDataProvider.WriteDatabase(
                    data,
                    DataPath,
                    new Progress<double>(progress => Progress = progress)
                );
            });
        }

        private Task<IEnumerable<Person>> LoadDataAsync()
        {
            ProgressMessage = Resources.ProcessingData;
            return Task.Run(() =>
            {
                return XmlDataProvider.ReadDatabase(
                    DataPath,
                    new Progress<double>(progress => Progress = progress)
                );
            });
        }
    }
}