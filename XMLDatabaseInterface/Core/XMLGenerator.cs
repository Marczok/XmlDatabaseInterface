using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using XMLDatabaseInterface.Core.DomainTypes;

namespace XMLDatabaseInterface.Core
{
    public class XMLGenerator
    {
        private static readonly string[] Adresses =
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

        public static void GenerateDatabase(int size, string filename)
        {
            var firstNames = File.ReadAllLines("Resources/DataSources/FirstNames.txt");
            var lastNames = File.ReadAllLines("Resources/DataSources/Surnames.txt");

            var rnd = new Random();


            var persons = new List<Person>();
            for (var i = 0; i < size; i++)
            {
                var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(rnd.Next(1532031622)).ToLocalTime();

                persons.Add(new Person
                {
                    Name = firstNames[rnd.Next(firstNames.Length - 1)],
                    Surename = lastNames[rnd.Next(lastNames.Length - 1)],
                    Address = Adresses[rnd.Next(Adresses.Length - 1)] + " " + rnd.Next(128),
                    Birthdate = dateTime.ToShortDateString()
                });
            }

            var serializer = new XmlSerializer(typeof(List<Person>));
            using (var writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, persons);
            }
        }

    }
}
