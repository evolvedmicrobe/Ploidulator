using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bio;
using Bio.IO.SAM;

namespace Ploidulator.Metrics
{
    public class ClusterDirtMetric : IClusterMetric
    {
        List<string> Header = new List<string>() { "ClusterDirt" };
        int ploidy;
        public ClusterDirtMetric(int ploidy=2)
        {
            this.ploidy = ploidy;
        }
        /// <summary>
        /// Gets the percentage of reads greater than the ploidy for this individual.
        /// </summary>
        /// <param name="seqs"></param>
        /// <returns></returns>
        private double getIndividualDirtValue(List<SAMAlignedSequence> seqs)
        {
            var seqsCopy = seqs.Select(z=>new ComparableSAMSequence(z)).ToList();
            seqsCopy.Sort();
            if(seqsCopy.Count<=ploidy)
            {
                return 0;
            }
            else
            {
                List<int> counts = new List<int>() { 1 };
                for(int i=1;i<seqsCopy.Count;i++)
                {
                    if (seqsCopy[i].CompareTo(seqsCopy[i - 1])==0) { 
                        counts.Add(1);
                    }
                    else
                    {
                        counts[counts.Count - 1]++;
                    }
                }
                counts.Sort();
                counts.Reverse();
                return counts.Skip(ploidy).Sum() / (double)counts.Sum();
            }
        }
        public List<double> Calculate(Cluster cluster)
        {
            var dirtPerIndividual = cluster.ReadsByIndividuals.Values.Select(x => getIndividualDirtValue(x)).ToList();
            return new List<double>() { dirtPerIndividual.Average() };
        }

        public IList<string> GetHeaderFields()
        {
            return Header.AsReadOnly();
        }
        /// <summary>
        /// Class is tightened up to make string comparisons faster.
        /// </summary>
        internal class ComparableSAMSequence : IComparable<ComparableSAMSequence>
        {
            internal byte[] data;
            internal ComparableSAMSequence(SAMAlignedSequence seq)
            {
                data = seq.QuerySequence.Select(x => (byte)x).ToArray();
            }
            public int CompareTo(ComparableSAMSequence other)
            {

                var s1q = this.data ;
                var s2q = other.data;
                var lengthCompare = s1q.Length.CompareTo(s2q.Length);
                if (lengthCompare != 0) { return lengthCompare; }
                for (int i = 0; i < s1q.Length; i++)
                {
                    var b1 = s1q[i];
                    var b2 = s2q[i];
                    var cmp = b1.CompareTo(b2);
                    if (cmp != 0) { return cmp; }
                }
                return 0;
            }
        }

    }
}
