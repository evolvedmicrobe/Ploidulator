using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bio;
using Bio.IO.SAM;

namespace Ploidulator.Metrics
{
    public class ReadsPerIndividual : IClusterMetric
    {
        List<string> header;
        List<string> SampleNames;
        public ReadsPerIndividual(List<string> sampleNames)
        {
            this.SampleNames=sampleNames;
            header=sampleNames.Select(x=>x+"_ReadCount").ToList();
        }
     
        public List<double> Calculate(Cluster cluster)
        {
            var results=new List<double>(SampleNames.Count);
            foreach(var samp in SampleNames)
            {
                List<SAMAlignedSequence> data;
                bool hasValue=cluster.ReadsByIndividuals.TryGetValue(samp,out data);
                if(hasValue){
                    results.Add((double)data.Count);
                }
                else
                {
                    results.Add(0.0);
                }
            }
            return results;
        }

        public IList<string> GetHeaderFields()
        {
            return header.AsReadOnly();
        }
    }
}
