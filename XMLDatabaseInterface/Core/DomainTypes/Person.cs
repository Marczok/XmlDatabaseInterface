using System.Xml.Serialization;

namespace XMLDatabaseInterface.Core.DomainTypes
{
    public class Person
    {
        [XmlAttribute] public string Name { get; set; }

        [XmlAttribute] public string Surename { get; set; }

        [XmlAttribute] public string Address { get; set; }

        [XmlAttribute] public string Birthdate { get; set; }

        protected bool Equals(Person other)
        {
            return string.Equals(Name, other.Name) && 
                   string.Equals(Surename, other.Surename) &&
                   string.Equals(Address, other.Address) && 
                   string.Equals(Birthdate, other.Birthdate);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Person) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Name != null ? Name.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Surename != null ? Surename.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Address != null ? Address.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Birthdate != null ? Birthdate.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}