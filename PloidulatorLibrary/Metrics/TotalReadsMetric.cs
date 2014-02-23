using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploidulator.Metrics
{
    public class TotalReadsMetric : IClusterMetric
    {
        List<string> Header = new List<string>() { "TotalReads" };

        public List<double> Calculate(Cluster cluster)
        {
            return new List<double>() {
                (double)cluster.TotalReadCount
            };
        }

        public IList<string> GetHeaderFields()
        {
            return Header.AsReadOnly();
        }
    }
}
