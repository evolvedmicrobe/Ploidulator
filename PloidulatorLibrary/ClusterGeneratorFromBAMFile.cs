using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bio;
using Bio.IO.SAM;
using Bio.IO.BAM;


namespace Ploidulator
{
    /// <summary>
    /// A class that produces clusters from the original 
    /// </summary>
    public class ClusterGeneratorFromBAMFile
    {
        /// <summary>
        /// Consumer of this class can subscribe to this event to be notified of parsing/analysis progress.
        /// </summary>
        public event ClusterProcessorUpdateHandler UpdateHandler;

        private int updateInterval;
        public int ReadsProcessed;
        public int UnmappedReadsProcessed;
        public int ClustersProcessed;
        
        public ClusterGeneratorFromBAMFile(ClusterProcessorUpdateHandler update=null,int updateInterval=100000)
        {
            if (update != null)
            {
                UpdateHandler += update;
            }
            this.updateInterval = updateInterval;
            ReadsProcessed = 0;
            UnmappedReadsProcessed = 0;
            ClustersProcessed = 0;
        }
        
        /// <summary>
        /// Processess a bam file dividing it into Cluster that it then yields back.
        /// </summary>
        public IEnumerable<Cluster> ProcessSequences(string filename)
        {
            //keep track of last cluster and last position
            Cluster curCluster = null;
            //also make sure things are sorted
            int lastPos=int.MinValue;
            int lastEnd = int.MinValue;
            BAMParser bp = new BAMParser();
            foreach (SAMAlignedSequence seq in bp.ParseSequenceAsEnumerable(filename))
            {
                ReadsProcessed++;
                if (seq.Pos < 1 || seq.CIGAR == "*" || seq.Flag.HasFlag(SAMFlags.UnmappedQuery))
                {
                    UnmappedReadsProcessed++;
                    continue;
                }
                //SKIP POSITION AND GET 
                if (seq.Pos > 0 && curCluster!=null &&
                    seq.RName == curCluster.GenomeLocation.ID && seq.Pos < lastPos)
                {
                    throw new PloidulatorException("BAM file is not in sorted order.  Problem appeared at read: " + seq.ToString());
                }
                if(seq.Pos>lastEnd || seq.RName!=curCluster.GenomeLocation.ID)
                {
                    if (curCluster != null) { yield return curCluster; }
                    curCluster = new Cluster(seq.RName, seq.Pos);
                }
                curCluster.AddRead(seq);
                lastPos = seq.Pos;
                lastEnd = seq.RefEndPos;
                if (ReadsProcessed % updateInterval == 0)
                {
                    if (UpdateHandler != null && curCluster!=null)
                    {
                        ClusterProcessorUpdate cu = new ClusterProcessorUpdate() 
                        {CurrentCluster=curCluster.GenomeLocation.ID,
                            ReadsProcessed = ReadsProcessed, 
                            UnMappedReads = UnmappedReadsProcessed };
                        UpdateHandler(cu);
                    }
                }
            }
            if (curCluster != null)
            {
                yield return curCluster;
            }
        }

        /// <summary>
        /// Dictionary of all possible allele values
        /// </summary>
        private static Dictionary<char, string> alleles = new Dictionary<char, string>()
        {
            {'A', "1"},
            {'T', "2"},
            {'C', "3"},
            {'G', "4"},
            {'?', "-1"}
        };
    }
}
