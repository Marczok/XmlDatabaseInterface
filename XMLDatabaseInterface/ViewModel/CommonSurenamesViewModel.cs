using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using XMLDatabaseInterface.Core.DomainTypes;

namespace XMLDatabaseInterface.ViewModel
{
    public class CommonSurenamesViewModel : ViewModelBase
    {
        private ObservableCollection<SurenameStatistics> _commonSurenames =
            new ObservableCollection<SurenameStatistics>();

        public CommonSurenamesViewModel(IDataProvider provider)
        {
            Persons = provider.Persons;
            ProcessCommonSurenamesCommand = new RelayCommand(async () =>
            {
                IEnumerable<IGrouping<string, Person>> selected = null;
                await Task.Run(() =>
                {
                    selected = Persons.GroupBy(p => p.Surename).OrderByDescending(grp => grp.Count()).Take(10);
                }).ConfigureAwait(true);

                CommonSurenames.Clear();
                foreach (var item in selected)
                {
                    CommonSurenames.Add(new SurenameStatistics(item.Key, item.Count()));
                }
            }, () => Persons != null && Persons.Count > 0);
        }

        private IReadOnlyCollection<Person> Persons { get; }
        public RelayCommand ProcessCommonSurenamesCommand { get; }

        public ObservableCollection<SurenameStatistics> CommonSurenames
        {
            get => _commonSurenames;
            set => Set(() => CommonSurenames, ref _commonSurenames, value);
        }
    }
}
