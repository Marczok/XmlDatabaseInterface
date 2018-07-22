using System.Collections.ObjectModel;
using XMLDatabaseInterface.Core.DomainTypes;

namespace XMLDatabaseInterface.ViewModel
{
    public interface IDataProvider
    {
        ObservableCollection<Person> Persons { get; }
    }
}