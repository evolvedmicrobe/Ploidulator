using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ploidulator;
using Ploidulator.Metrics;
using Bio.IO.BAM;
using Bio.IO.SAM;

namespace ddradqc
{
    public class ddRADSeqQCFileGenerator
    {
        #region Public Fields

        /// <summary>
        /// Input file name.
        /// </summary>
        public string InputFilename;

        /// <summary>
        /// Output file name
        /// </summary>
        public string OutputFilename;


        public int NumberOfReadGroups { get; private set; }
        public int NumerOfSequences { get; private set; }


        #endregion
        string delimiter = ",";
        List<string> ReferenceSequences;
        List<IClusterMetric> metrics;
        List<string> sampleNames;
        StreamWriter outStream;
        SAMAlignmentHeader header = null;
        public void GenerateFile()
        {
           
            validateInputFileAndLoadSampleNames();
            setupOutputFile();
            //now setup metrics
            metrics = new List<IClusterMetric>() {
                new SampleCountMetric(),
                new ClusterDirtMetric(),
                new AverageMappingQualityMetric(),
                new AverageQualityMetric(),
                new MaxReadCountPerSample(),
                new MinReadCountForSample(),
                new TotalReadsMetric(),                
                new ReadsPerIndividual(sampleNames)
               };
            //now initialize the output file
            List<string> headerString=new List<string>();
            headerString.Add("ClusterID");
            headerString.Add("Reference");
            headerString.Add("Start");
            headerString.Add("End");
            headerString.AddRange(metrics.SelectMany(x=>x.GetHeaderFields()));
            var header = String.Join(delimiter, headerString);
            outStream.WriteLine(header);
            int curCluster = 0;
            ClusterGeneratorFromBAMFile clusterMaker = new ClusterGeneratorFromBAMFile(ReportClusterProgress);
            
            foreach(var cluster in clusterMaker.ProcessSequences(InputFilename))
            {
                curCluster++;
                List<string> outValues = new List<string>();
                outValues.Add(curCluster.ToString());
                outValues.Add(cluster.GenomeLocation.ID);
                outValues.Add(cluster.GenomeLocation.Start.ToString());
                outValues.Add(cluster.GenomeLocation.End.ToString());
                outValues.AddRange(metrics.SelectMany(x=>x.Calculate(cluster)).Select(y=>y.ToString()));
                var line=String.Join(delimiter,outValues);
                outStream.WriteLine(line);
            }
            outStream.Close();
        }
        public void ReportClusterProgress(ClusterProcessorUpdate update)
        {
            string outLine="["+DateTime.Now.ToString()+"] Current Ref = "+update.CurrentCluster+" Reads Processed="+
                update.ReadsProcessed.ToString()+" Unmapped Reads="+update.UnMappedReads.ToString();
            Console.WriteLine(outLine);
        }
        void validateInputFileAndLoadSampleNames()
        {
            if(!File.Exists(InputFilename))
            {
                throw new FileNotFoundException("Could not find file: "+InputFilename);
            }
            using (Stream stream = new FileStream(InputFilename, FileMode.Open, FileAccess.Read))
            {
                BAMParser bp = new BAMParser();
                header = bp.GetHeader(stream);
                var tmp = header.RecordFields.Where(x => x.Typecode == "RG").ToList();
                sampleNames=tmp.Select(z=>z.Tags.Where(p=>p.Tag=="ID").First()).Select(z=>z.Value).ToList();
                NumberOfReadGroups = sampleNames.Count;
                NumerOfSequences = header.ReferenceSequences.Count;
            }
            Console.WriteLine("Processing file with " + sampleNames.Count.ToString() + " samples and " + NumerOfSequences.ToString() + " reference sequences.");
        }

        void setupOutputFile()
        {
            if(File.Exists(OutputFilename))
            {
                Console.WriteLine("Deleting and replacing output file.");
                    File.Delete(OutputFilename);
                //throw new ArgumentException("Output file already exists.\nPlease delete file: " + OutputFilename);
            }
            outStream = new StreamWriter(OutputFilename);
        }

    }
}
