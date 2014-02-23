using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploidulator.Metrics
{
    public class SampleCountMetric : IClusterMetric
    {
        List<string> Header = new List<string>() { "NumSamplesWithReads" };

        public List<double> Calculate(Cluster cluster)
        {
            return new List<double>() {
                (double)cluster.ReadsByIndividuals.Count
            };
        }

        public IList<string> GetHeaderFields()
        {
            return Header.AsReadOnly();
        }
    }
}
