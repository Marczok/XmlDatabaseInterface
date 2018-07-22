using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using XMLDatabaseInterface.Core.DomainTypes;

namespace XMLDatabaseInterface.ViewModel
{
    public class BirthdayTodayViewModel : ViewModelBase
    {
        private ObservableCollection<Person> _birthdayCollection;

        public BirthdayTodayViewModel(IDataProvider provider)
        {
            Persons = provider.Persons;
            ProcessBirthdayCommand = new RelayCommand(async () =>
            {
                var today = DateTime.Now;
                IEnumerable<Person> birthday = null;

                await Task.Run(() =>
                {
                    birthday = Persons.Where(p => p.Birthdate.Day == today.Day && p.Birthdate.Month == today.Month);
                }).ConfigureAwait(true);

                BirthdayCollection = new ObservableCollection<Person>(birthday);
            }, () => Persons != null && Persons.Count > 0);
        }

        private IReadOnlyCollection<Person> Persons { get; }
        public RelayCommand ProcessBirthdayCommand { get; }

        public ObservableCollection<Person> BirthdayCollection
        {
            get => _birthdayCollection;
            set => Set(() => BirthdayCollection, ref _birthdayCollection, value);
        }
    }
}
