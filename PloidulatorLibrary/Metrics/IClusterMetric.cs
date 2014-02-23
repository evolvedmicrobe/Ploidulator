using Bio.IO.SAM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploidulator.Metrics
{
    /// <summary>
    /// An IMetric calculates various metric values from a list of SAMAlignedSequences
    /// </summary>
    public interface IClusterMetric
    {
        
        /// <summary>
        /// Calculate metric values from the given list of sequences.
        /// Returns the metrics as a list of strings
        /// </summary>
        List<double> Calculate(Cluster cluster);

        /// <summary>
        /// Gets the headers for the fields returned by calculate
        /// </summary>
        /// <typeparam name="?"></typeparam>
        /// <param name="?"></param>
        IList<string> GetHeaderFields();

     

    }
}
