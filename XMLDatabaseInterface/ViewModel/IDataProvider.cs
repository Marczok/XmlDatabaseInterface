using System;
using System.Collections.Generic;
using System.ComponentModel;
using XMLDatabaseInterface.Core.DomainTypes;

namespace XMLDatabaseInterface.ViewModel
{
    public interface IDataProvider
    {
        List<Person> GenerateDatabase(int size, IProgress<double> progress = null);
        bool SaveDatabase(IEnumerable<Person> data, string filename, IProgress<double> progress = null);
        bool LoadDatabase(string filename, IProgress<double> progress = null);
        IEnumerable<Person> Database { get; }
        event PropertyChangedEventHandler PropertyChanged;
    }
}