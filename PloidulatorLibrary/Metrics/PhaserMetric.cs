using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploidulator.Metrics
{

    //Currently superceded by the GATK haplotype caller
#if FALSE
    public class PhaserMetric : ClusterMetric
    {
        #region phase formatted strings

        /// <summary>
        /// Read only. Gets the formatted string as required as input by PHASE. For each allele position, 'S' indicates biallelic and
        /// 'M' indicates multiallelic.
        /// </summary>
        public string PhaseLoci { get { return loci; } }

        /// <summary>
        /// Read only. Gets the formatted string as required as input by PHASE. Multi-line string where each individual has three lines: 
        /// unique identifier, first allele at each position, second allele at each position (genotype).
        /// </summary>
        public string PhaseData { get { return phaseData; } }

        #endregion

        #region phase

        /// <summary>
        /// Formatted string as required as input by PHASE. For each allele position, 'S' indicates biallelic and
        /// 'M' indicates multiallelic 
        /// </summary>
        private string loci;

        /// <summary>
        /// Formatted string as required as input by PHASE. Multi-line string where each individual has three lines: 
        /// unique identifier, first allele at each position, second allele at each position (genotype)
        /// </summary>
        private string phaseData;

        #endregion
        /// <summary>
        /// For each individual, add their genotypes to the phase input string (phaseData)
        /// </summary>
        private void ConstructPhaseData(List<Dictionary<char, double>[]> dict)
        {
            int j = 0;
            foreach (Dictionary<string, List<SAMAlignedSequence>> seqList in sampleSequenceDict.Values)
            {
                string indivId = "#" + seqList.Values.ToArray()[0][0].QName;
                string chr1 = "", chr2 = "";
                int locusCount = 0;
                foreach (char allele in loci) // for each allele at this locus, where loci is in the format "S--SS-SM---MSSMMS-MMM--MSSSMS"
                // we will iterate through looking at one physical locus at a time and examining all sequences that align to that locus
                {
                    if (allele == 'S' || allele == 'M')
                    {
                        // Get an array of all alleles which appear at this locus for this individual
                        // If any of these allele characters are not in the dictionary of recognised alleles, replace them with '?' 
                        char[] allelesThisIndiv = GetAllelesAtLocusForIndiv(dict[j], locusCount);

                        // Adds an allele char to each chromosome (chr1 and chr2), in format required by PHASE for either
                        // biallelic or multiallelic
                        GetIndivBiAllelicLocusAlleles(allelesThisIndiv, seqList.Count, ref chr1, ref chr2, (allele == 'M'));
                    }
                    locusCount++;
                }
                // Add this individual's genotype information to the phase file data string
                phaseData += (indivId + "\r\n" + chr1 + "\r\n" + chr2 + "\r\n");
                j++;
            }
        }
        /// <summary>
        /// For any loci in the phase master string (loci) which do not have snps, remove that char in the string
        /// (postcondition: loci string should be of format SSSSSMSSSMMSSSSS with no '-' characters)
        /// </summary>
        private void TrimPhaseMasterString()
        {
            string locii = "";
            for (int k = 0; k < loci.Length; k++)
            {
                if (loci[k] != '-')
                {
                    locii += loci[k];
                }
            }
            loci = locii;
        }
        /// <summary>
        /// Construct component of the PHASE input string (for all individuals, including master string in format
        /// MSSSSSMMSSSSS and content string with genotypes of all individuals)
        /// </summary>
        private void ConstructPhaseSnpString()
        {
            Dictionary<char, double>[] alleleFxAllIndiv = new Dictionary<char, double>[referenceSequence.Length];
            List<Dictionary<char, double>[]> alleleFxAllIndivFull = new List<Dictionary<char, double>[]>();

            // Generates the data for each individual for the phase file
            phaseData = "";
            foreach (Dictionary<string, List<SAMAlignedSequence>> seqList in sampleSequenceDict.Values)
            {
                Dictionary<char, double>[] alleleFxThisIndiv = BaseFrequencies(seqList.Keys.ToArray(), ref alleleFxAllIndiv);
                alleleFxAllIndivFull.Add(alleleFxThisIndiv);
            }
            loci = "";
            string noSnp = "-";
            string biAllelic = "S";
            string multiAllelic = "M";
            foreach (Dictionary<char, double> i in alleleFxAllIndiv)
            {
                if (i.Keys.Count == 0 || i.Keys.Count == 1)
                {
                    loci += noSnp;
                }
                else if (i.Keys.Count == 2)
                {
                    loci += biAllelic;
                }
                else
                {
                    loci += multiAllelic;
                }
            }

            // Add individual genotypes to PHASE format input string
            ConstructPhaseData(alleleFxAllIndivFull);

            // Remove any non-snp chars from master string
            TrimPhaseMasterString();
        }

        /// <summary>
        /// Get an allele from the alleles dictionary, as a string, in the format required by PHASE input for either
        /// biallelic or multiallelic loci
        /// </summary>
        private static String GetAllele(char allele, bool isMultiAllelic = false)
        {
            if (!isMultiAllelic)
            {
                return allele.ToString();
            }
            else
            {
                return alleles[allele];
            }
        }

        /// <summary>
        /// For a single locus append a character onto each chromosome string (chr1 and chr2) representing the allele for 
        /// THIS INDIVIDUAL at this position, in the format required by PHASE for loci (either biallelic or multiallelic, 
        /// as specified)
        /// </summary>
        private static bool GetIndivBiAllelicLocusAlleles(char[] allelesThisIndividual, int numSeqsThisIndivHas,
            ref string chr1, ref string chr2, bool isMultiAllelic = false)
        {
            switch (allelesThisIndividual.Length)
            {
                case 0:
                    // This individual has no alleles at this position
                    chr1 += GetAllele('?', isMultiAllelic) + " ";
                    chr2 += GetAllele('?', isMultiAllelic) + " ";
                    break;

                case 1:
                    if (numSeqsThisIndivHas > 1)
                    {
                        // This individual has one distinct allele at this position, but more than one instance of it occurring,
                        // so we assume it may be on both chromosomes
                        chr1 += GetAllele(allelesThisIndividual[0], isMultiAllelic) + " ";
                        chr2 += GetAllele(allelesThisIndividual[0], isMultiAllelic) + " ";
                    }
                    else
                    {
                        // This individal only has one read in this cluster, therefore not enough information to infer that chr2 == chr1 
                        // at this position, therefore chr2 at this position will be called undefined
                        chr1 += GetAllele(allelesThisIndividual[0], isMultiAllelic) + " ";
                        chr2 += GetAllele('?', isMultiAllelic) + " ";
                    }
                    break;

                case 2:
                    // This individual has two sequences, both with different alleles at this position
                    chr1 += GetAllele(allelesThisIndividual[0], isMultiAllelic) + " ";
                    chr2 += GetAllele(allelesThisIndividual[1], isMultiAllelic) + " ";
                    break;

                default:
                    // This cannot happen. No individual has more than two alleles at any one position
                    Console.WriteLine(Properties.Resources.ALLELE_COUNT_ERROR);
                    return false;
            }
            return true;
        }



    }
#endif

}
