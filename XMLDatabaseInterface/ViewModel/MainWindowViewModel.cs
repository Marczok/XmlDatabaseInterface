using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using XMLDatabaseInterface.Core.DomainTypes;
using XMLDatabaseInterface.Properties;
using WindowState = Xceed.Wpf.Toolkit.WindowState;

namespace XMLDatabaseInterface.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private int _databaseSize = 5000;
        private ObservableCollection<Person> _database = new ObservableCollection<Person>();
        private WindowState _dataSourceWindowState = WindowState.Open;
        private double _progress;
        private WindowState _progressWindowState;
        private string _progressMessage;
        private WindowState _addWindowState;
        private string _addName;
        private string _addSurename;
        private string _addAddress;
        private DateTime _addDate = DateTime.Now;
        private readonly IDataProvider _provider;
        private WindowState _closingDialogState;

        public MainWindowViewModel(IDataProvider provider)
        {
            _provider = provider;

            GenerateDataCommand = new RelayCommand(async () =>
            {
                DataSourceWindowState = WindowState.Closed;

                ProgressWindowState = WindowState.Open;

                var data = await GenerateDataAsync().ConfigureAwait(true);
                await SaveDataAsync(data).ConfigureAwait(true);
                Database = new ObservableCollection<Person>(_provider.Database);

                ProgressWindowState = WindowState.Closed;
            }, () => MinDatabaseSize < DatabaseSize && DatabaseSize <= MaxDatabaseSize);

            LoadDataCommand = new RelayCommand(async () =>
            {
                DataSourceWindowState = WindowState.Closed;

                ProgressWindowState = WindowState.Open;

                await LoadDataAsync().ConfigureAwait(true);
                Database = new ObservableCollection<Person>(_provider.Database);

                ProgressWindowState = WindowState.Closed;
            }, () => File.Exists(DataPath));

            SaveDataCommand = new RelayCommand(async () =>
            {
                ProgressWindowState = WindowState.Open;
                await SaveDataAsync(Database).ConfigureAwait(true);
                ProgressWindowState = WindowState.Closed;
            }, () => Database != null && Database?.Count > 0);

            ResetDataCommand = new RelayCommand(
                () => Database = new ObservableCollection<Person>(_provider.Database),
                () => Database != null && Database?.Count > 0);

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
                    Database.Remove(person);
                }
            }, selected => selected != null && selected.Count > 0 && Database != null && Database.Count > 0);

            OpenAddWindowCommand = new RelayCommand(() => AddWindowState = WindowState.Open);

            AddPersonCommand = new RelayCommand(() =>
                {
                    Database.Add(new Person(AddName, AddSurename, AddAddress, AddDate));
                    AddWindowState = WindowState.Closed;
                },
                () => !string.IsNullOrEmpty(AddName) && !string.IsNullOrEmpty(AddSurename) &&
                      !string.IsNullOrEmpty(AddAddress));

            ClearAddPersonDataCommand = new RelayCommand(() =>
            {
                AddName = null;
                AddSurename = null;
                AddAddress = null;
                AddDate = DateTime.Now;
            });

            WindowClosingCommand = new RelayCommand<CancelEventArgs>(args =>
            {
                args.Cancel = true;
                ClosingDialogState = WindowState.Open;
            }, args => DataSourceWindowState == WindowState.Closed);

            SaveAndCloseCommand = new RelayCommand(async () =>
            {
                SaveDataCommand.Execute(null);
                await Task.Delay(3000).ConfigureAwait(true);
                Application.Current.Shutdown(0);
            });

            CloseCommand = new RelayCommand(() => Application.Current.Shutdown(0));
        }

        public string DataPath { get; } = "Resources/DataSources/data.xml";
        public int MinDatabaseSize { get; } = 0;
        public int MaxDatabaseSize { get; } = 500000;

        public RelayCommand GenerateDataCommand { get; }
        public RelayCommand LoadDataCommand { get; }
        public RelayCommand SaveDataCommand { get; }
        public RelayCommand ResetDataCommand { get; }

        public RelayCommand<IList> DeletePersonCommand { get; }
        public RelayCommand OpenAddWindowCommand { get; }
        public RelayCommand AddPersonCommand { get; }
        public RelayCommand ClearAddPersonDataCommand { get; }

        public RelayCommand<CancelEventArgs> WindowClosingCommand { get; }
        public RelayCommand SaveAndCloseCommand { get; }
        public RelayCommand CloseCommand { get; }

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

        public ObservableCollection<Person> Database
        {
            get => _database;
            private set => Set(() => Database, ref _database, value);
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

        public WindowState ClosingDialogState
        {
            get => _closingDialogState;
            set => Set(() => ClosingDialogState, ref _closingDialogState, value);
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

        private Task<List<Person>> GenerateDataAsync()
        {
            ProgressMessage = Resources.GeneratingData;
            return Task.Run(() =>
            {
                return _provider.GenerateDatabase(
                    DatabaseSize,
                    new Progress<double>(progress => Progress = progress)
                );
            });
        }

        private Task<bool> SaveDataAsync(IEnumerable<Person> data)
        {
            ProgressMessage = Resources.SavingData;
            return Task.Run(() =>
            {
                return _provider.SaveDatabase(
                    data,
                    DataPath,
                    new Progress<double>(progress => Progress = progress)
                );
            });
        }

        private Task<bool> LoadDataAsync()
        {
            ProgressMessage = Resources.ProcessingData;
            return Task.Run(() =>
            {
                return _provider.LoadDatabase(
                    DataPath,
                    new Progress<double>(progress => Progress = progress)
                );
            });
        }
    }
}