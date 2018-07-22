using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using XMLDatabaseInterface.Core.DomainTypes;

namespace XMLDatabaseInterface.ViewModel
{
    public class BirthdayTodayViewModel : ViewModelBase
    {
        private ObservableCollection<Person> _birthdayCollection;

        public BirthdayTodayViewModel(IDataProvider provider)
        {
            provider.PropertyChanged += async (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case var name when nameof(provider.Database) == name:
                        var today = DateTime.Now;
                        IEnumerable<Person> birthday = null;

                        await Task.Run(() =>
                        {
                            birthday = provider.Database.Where(p => p.Birthdate.Day == today.Day && p.Birthdate.Month == today.Month);
                        }).ConfigureAwait(true);

                        BirthdayCollection = new ObservableCollection<Person>(birthday);
                        break;
                }
            };
        }

        public ObservableCollection<Person> BirthdayCollection
        {
            get => _birthdayCollection;
            set => Set(() => BirthdayCollection, ref _birthdayCollection, value);
        }
    }
}
