using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using XMLDatabaseInterface.Core.DomainTypes;

namespace XMLDatabaseInterface.ViewModel
{
    public class CommonSurenamesViewModel : ViewModelBase
    {
        private ObservableCollection<SurenameStatistics> _commonSurenames;

        public CommonSurenamesViewModel(IDataProvider provider)
        {
            provider.PropertyChanged += async (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case var name when nameof(provider.Database) == name:
                        var statistics = new List<SurenameStatistics>();
                        await Task.Run(() =>
                        {
                            var selected = provider.Database.GroupBy(p => p.Surename)
                                .OrderByDescending(grp => grp.Count()).Take(10);
                            statistics.AddRange(selected.Select(item => new SurenameStatistics(item.Key, item.Count())));
                        }).ConfigureAwait(true);

                        CommonSurenames = new ObservableCollection<SurenameStatistics>(statistics);
                        break;
                }
            };
        }

        public ObservableCollection<SurenameStatistics> CommonSurenames
        {
            get => _commonSurenames;
            set => Set(() => CommonSurenames, ref _commonSurenames, value);
        }
    }
}