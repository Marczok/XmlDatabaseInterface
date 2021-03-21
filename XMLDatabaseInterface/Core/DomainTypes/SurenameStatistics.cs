using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLDatabaseInterface.Core.DomainTypes
{
    public class SurenameStatistics
    {
        public SurenameStatistics(string surname, int count)
        {
            Surname = surname;
            Count = count;
        }

        public string Surname { get; }
        public int Count { get; }
    }
}
