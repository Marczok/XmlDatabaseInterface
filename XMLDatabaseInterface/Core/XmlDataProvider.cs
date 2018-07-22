using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using XMLDatabaseInterface.Core.DomainTypes;
using XMLDatabaseInterface.ViewModel;

namespace XMLDatabaseInterface.Core
{
    public class XmlDataProvider :  IDataProvider, INotifyPropertyChanged
    {
        private readonly string[] _adresses =
        {
            "Brno, Ceska",
            "Brno, Olomoucka",
            "Sun city, Oak street",
            "Pitsburg, Wilnigton street",
            "Torronto, Cloak road",
            "Brno, Antoninska",
            "Praha, Husitska",
            "Olomouc, Boleslavska",
            "Trinec, Caslavska",
            "New York, Smith district",
            "Washington, Longs Island",
            "Los Angeles, Casino Palace",
            "Latain Sel, Gofrey Road",
            "Sanghay, Se lange shi",
            "Hong kong, Lange woshi",
            "Paris, Gofreys la Place",
            "Berlin, Schwarz Strasse",
            "Koln, Kasse Strasse"
        };

        private IEnumerable<Person> _database;

        public IEnumerable<Person> Database
        {
            get => _database;
            private set
            {
                _database = value;
                OnPropertyChanged(nameof(Database));
            }
        }

        public List<Person> GenerateDatabase(int size, IProgress<double> progress = null)
        {
            if (size < 1)
            {
                return null;
            }

            var firstNames = File.ReadAllLines("Resources/DataSources/FirstNames.txt");
            var lastNames = File.ReadAllLines("Resources/DataSources/Surnames.txt");

            var rnd = new Random();


            var persons = new List<Person>();
            for (var i = 0; i < size; i++)
            {
                var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(rnd.Next(1532031622)).ToLocalTime();

                string name = string.Empty, surename = string.Empty;
                while (name == string.Empty || surename == string.Empty)
                {
                    name = firstNames[rnd.Next(firstNames.Length - 1)];
                    surename = lastNames[rnd.Next(lastNames.Length - 1)];
                }

                var address = _adresses[rnd.Next(_adresses.Length - 1)] + " " + rnd.Next(128);

                persons.Add(new Person(name, surename, address, dateTime));
                progress?.Report((double)i / size);
            }

            return persons;
        }

        public bool SaveDatabase(IEnumerable<Person> data, string filename, IProgress<double> progress = null)
        {
            if (data == null || string.IsNullOrEmpty(filename))
            {
                return false;
            }

            var serializer = new XmlSerializer(typeof(List<Person>));
            var enumerable = data.ToList();
            using (var writer = new StreamWriter(filename))
            {
                ReportStreamPosition(writer.BaseStream, progress);
                serializer.Serialize(writer, enumerable);
            }

            Database = enumerable;
            return true;
        }

        public bool LoadDatabase(string filename, IProgress<double> progress = null)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
            {
                return false;
            }

            var serializer = new XmlSerializer(typeof(List<Person>));
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                var reader = XmlReader.Create(stream);
                if (serializer.CanDeserialize(reader))
                {
                    ReportStreamPosition(stream, progress);
                    Database = (List<Person>)serializer.Deserialize(reader);
                    return true;
                }

                return false;
            }
        }

        private static void ReportStreamPosition(Stream stream, IProgress<double> progress)
        {
            if (progress != null)
            {
                Task.Run(async () =>
                {
                    progress.Report(0);
                    while (stream.Position < stream.Length)
                    {
                        await Task.Delay(50).ConfigureAwait(false);
                        progress.Report((double)stream.Position / stream.Length);
                    }

                    progress.Report(1);
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}