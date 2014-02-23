using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploidulator.Metrics
{
    /// <summary>
    /// Calculates the average quality score in bases for all reads, and returns the average of that.
    /// </summary>
    public class AverageQualityMetric : IClusterMetric 
    {
        List<string> Header = new List<string>() { "AverageofAverageReadQuality" };

        public List<double> Calculate(Cluster cluster)
        {
            return new List<double>() {
                cluster.ReadsByIndividuals.SelectMany(x => x.Value).Select(y => y.GetQualityScores().Average()).Average()
            };
        }

        public IList<string> GetHeaderFields()
        {
            return Header.AsReadOnly();
        }
    }
}
