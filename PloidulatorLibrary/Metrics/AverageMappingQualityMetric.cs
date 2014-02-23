using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploidulator.Metrics
{
    public class AverageMappingQualityMetric : IClusterMetric
    {

        List<string> Header = new List<string>() { "AverageAlignmentQuality" };
        
        public List<double> Calculate(Cluster cluster)
        {
            return new List<double>() {
                cluster.ReadsByIndividuals.SelectMany(x => x.Value).Select(y => y.MapQ).Average()
            };
        }

        public IList<string> GetHeaderFields()
        {
            return Header.AsReadOnly();            
        }
    }
}
