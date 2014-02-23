using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploidulator.Metrics
{
    public class MaxReadCountPerSample: IClusterMetric
    {
        List<string> Header = new List<string>() { "MaxReadCountForSample" };

        public List<double> Calculate(Cluster cluster)
        {
            return new List<double>() {
                (double)cluster.ReadsByIndividuals.Values.Select(x=>x.Count).Max()
            };
        }

        public IList<string> GetHeaderFields()
        {
            return Header.AsReadOnly();
        }
    }
}
