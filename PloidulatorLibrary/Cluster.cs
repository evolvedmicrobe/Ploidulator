using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bio;
using Bio.IO.SAM;

namespace Ploidulator
{
    using Sample = System.String;

    /// <summary>
    /// A collection of reads from one genomic location, contains a dictionary of 
    /// individuals to reads from that individual and genome location giving the 
    /// region of the genome the reads came from.
    /// 
    /// </summary>
    public class Cluster 
    {
        public ISequenceRange GenomeLocation;
        /// <summary>
        /// Gets a dictionary where each sample name is the key, and 
        /// a list of all  sequences for that individual is stored
        /// as the value.
        /// The value list is sorted in descending order of list size. Read only.
        /// </summary>
        public DictionaryOfIndividualsToReadsInCluster ReadsByIndividuals = new DictionaryOfIndividualsToReadsInCluster();
        public int TotalReadCount;
        public Cluster(string refGenome, int refStart)
        {
            GenomeLocation = new SequenceRange();
            GenomeLocation.ID = refGenome;
            GenomeLocation.Start = refStart;
        }
        public void AddRead(SAMAlignedSequence seq)
        {
            TotalReadCount++;
            ReadsByIndividuals.AddRead(seq);
            if (seq.RefEndPos > GenomeLocation.End) { GenomeLocation.End = seq.RefEndPos; }
        }
    }
}
