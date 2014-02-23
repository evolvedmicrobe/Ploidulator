using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bio.IO.SAM;
namespace Ploidulator
{
    using Sample = System.String;
    /// <summary>
    /// Dictionary that for each individual creates a list of reads that map to that individual from
    /// each cluster
    /// </summary>
    public class DictionaryOfIndividualsToReadsInCluster : IReadOnlyDictionary<Sample, List<SAMAlignedSequence>>
    {
        private Dictionary<Sample, List<SAMAlignedSequence>> readsFromIndividual;
        public DictionaryOfIndividualsToReadsInCluster() {
            readsFromIndividual = new Dictionary<Sample, List<SAMAlignedSequence>>();
        }
        public List<SAMAlignedSequence> this[Sample name] {
            get {
                return readsFromIndividual[name];
            }
            set {
                readsFromIndividual[name]=value;
            }
        }
        public void AddRead(SAMAlignedSequence read)
        {
            List<SAMAlignedSequence> data;
            string indiv=GetRgTag(read);            
            if(String.IsNullOrEmpty(indiv)) {
                throw new PloidulatorException("Reads in BAM File did not have Read Group Tags Assigned. Problem Read is:" +read.ToString());
            }
            bool groupIn = readsFromIndividual.TryGetValue(indiv,out data);
            if (groupIn)
            {
                data.Add(read);
            }
            else
            {
                data = new List<SAMAlignedSequence>();
                data.Add(read);
                readsFromIndividual[indiv] = data;
            }
        }
        private static int lastRGLocation = Int32.MaxValue;
        /// <summary>
        /// Given a SAMAlignedSequence, get the RG tag for that read
        /// </summary>
        private static string GetRgTag(SAMAlignedSequence seq)
        {
            var tags = seq.OptionalFields;
            if (tags.Count > lastRGLocation &&
                tags[lastRGLocation].Tag == "RG")
            {
                return tags[lastRGLocation].Value;
            }
            for (int i = 0; i < seq.OptionalFields.Count; i++)//SAMOptionalField field in seq.OptionalFields)
            {
                var field = seq.OptionalFields[i];
                // I iterate through to find RG each time in case the optional fields
                // do not have a consistent format. 
                if (field.Tag == "RG")
                {
                    lastRGLocation = i;
                    return field.Value;
                }
                i++;
            }
            return null;
        }



        public bool ContainsKey(Sample key)
        {
            return readsFromIndividual.ContainsKey(key);
        }

        public IEnumerable<Sample> Keys
        {
            get { return readsFromIndividual.Keys; }
        }

        public bool TryGetValue(Sample key, out List<SAMAlignedSequence> value)
        {
            return readsFromIndividual.TryGetValue(key, out value);
        }

        public IEnumerable<List<SAMAlignedSequence>> Values
        {
            get { return readsFromIndividual.Values; }
        }

        public int Count
        {
            get { return readsFromIndividual.Count; }
        }

        public IEnumerator<KeyValuePair<Sample, List<SAMAlignedSequence>>> GetEnumerator()
        {
            return readsFromIndividual.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return readsFromIndividual.GetEnumerator();
        }
    }
}
