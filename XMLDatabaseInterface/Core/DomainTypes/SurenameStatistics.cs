using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLDatabaseInterface.Core.DomainTypes
{
    public class SurenameStatistics
    {
        public SurenameStatistics(string surename, int count)
        {
            Surename = surename;
            Count = count;
        }

        public string Surename { get; }
        public int Count { get; }
    }
}
