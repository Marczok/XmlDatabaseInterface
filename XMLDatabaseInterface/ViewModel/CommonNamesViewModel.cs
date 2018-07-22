using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using XMLDatabaseInterface.Core.DomainTypes;

namespace XMLDatabaseInterface.ViewModel
{
    public class CommonNamesViewModel : ViewModelBase
    {
        private ObservableCollection<NameStatistic> _commonNames;

        public CommonNamesViewModel(IDataProvider provider)
        {
            provider.PropertyChanged += async (sender, args) => 
            {
                switch (args.PropertyName)
                {
                    case var name when nameof(provider.Database) == name:
                        var statistics = new List<NameStatistic>();
                        await Task.Run(() =>
                        {
                            var selected = provider.Database.GroupBy(p => p.Name)
                                .OrderByDescending(grp => grp.Count()).Take(10);
                            statistics.AddRange(selected.Select(item => new NameStatistic(item.Key, item.Count())));
                        }).ConfigureAwait(true);

                        CommonNames = new ObservableCollection<NameStatistic>(statistics);
                        break;
                }
            };
        }

        public ObservableCollection<NameStatistic> CommonNames
        {
            get => _commonNames;
            set => Set(() => CommonNames, ref _commonNames, value);
        }
    }
}
