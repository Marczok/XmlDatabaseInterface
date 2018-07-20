using System.Xml.Serialization;

namespace XMLDatabaseInterface.Core.DomainTypes
{
    public class Person
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Surename { get; set; }

        [XmlAttribute]
        public string Address { get; set; }

        [XmlAttribute]
        public string Birthdate { get; set; }
    }
}
