using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploidulator.Metrics
{
    public class MinReadCountForSample : IClusterMetric
    {
        List<string> Header = new List<string>() { "MinReadCountForSample" };

        public List<double> Calculate(Cluster cluster)
        {
            return new List<double>() {
                (double)cluster.ReadsByIndividuals.Values.Select(x=>x.Count).Min()
            };
        }

        public IList<string> GetHeaderFields()
        {
            return Header.AsReadOnly();
        }
    }
}
