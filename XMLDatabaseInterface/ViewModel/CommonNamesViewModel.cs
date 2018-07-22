﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using XMLDatabaseInterface.Core.DomainTypes;

namespace XMLDatabaseInterface.ViewModel
{
    public class CommonNamesViewModel : ViewModelBase
    {
        private ObservableCollection<NameStatistic> _commonNames =
            new ObservableCollection<NameStatistic>();

        
        public CommonNamesViewModel(IDataProvider provider)
        {
            Database = provider.Database;
            ProcessCommonNamesCommand = new RelayCommand(async () =>
            {
                IEnumerable<IGrouping<string, Person>> selected = null;
                await Task.Run(() =>
                {
                    selected = Database.GroupBy(p => p.Name).OrderByDescending(grp => grp.Count()).Take(10);
                }).ConfigureAwait(true);

                CommonNames.Clear();
                foreach (var item in selected)
                {
                    CommonNames.Add(new NameStatistic(item.Key, item.Count()));
                }
            }, () => Database != null && Database.Any());
        }

        private IEnumerable<Person> Database { get; }
        public RelayCommand ProcessCommonNamesCommand { get; }

        public ObservableCollection<NameStatistic> CommonNames
        {
            get => _commonNames;
            set => Set(() => CommonNames, ref _commonNames, value);
        }
    }
}
