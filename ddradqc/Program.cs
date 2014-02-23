using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bio;
using Ploidulator;
using Ploidulator.Metrics;
using Bio.Util;
using Bio.Util.ArgumentParser;
using System.Diagnostics;
namespace ddradqc
{
    class Program
    {
        static readonly string splashScreen =
@"ddradqc - QC Tool for assessing the reliability of RAD clusters.";
        static readonly string noArgumentError = 
@"You did not specify any arguments, which you must do so. 
    -i Input file (BAM input file, e.g. myFile.bam, need not be indexed, must be sorted)
    -o Output file prefix (the prefix for the csv out file)
    Example:
        ddradqc.exe -i=myBam.exe -o=testOut";
        private const double KB = 1024;
        private const double MB = KB * KB;
        private const double GB = MB * KB;

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(splashScreen);
                Console.WriteLine();
                if (args.Length < 1)
                {
                    Console.WriteLine(noArgumentError);
                }
                else
                {
                   CommandLineArguments parser = new CommandLineArguments();

                    // Add the parameters
                    ddRADSeqQCFileGenerator generator=new ddRADSeqQCFileGenerator();
                    parser.Parameter(ArgumentType.Required, "InputFilename", ArgumentValueType.String, "i", "Input filename");
                    parser.Parameter(ArgumentType.Required, "OutputFilename", ArgumentValueType.String, "o", "Output file name");
                    parser.Parse(args, generator);
                    generator.GenerateFile();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Program failed to execute");
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
                else
                {
                    Console.WriteLine(ex.Message);
                }
            }
            Process p = Process.GetCurrentProcess();
            Console.WriteLine(PeakWorkingSet64, FormatMemorySize(p.PeakWorkingSet64));
            Console.WriteLine(TotalProcessorTime, p.TotalProcessorTime);
            Console.WriteLine(PeakVirtualMemorySize64, FormatMemorySize(p.PeakVirtualMemorySize64));
            Console.WriteLine(PeakPagedMemorySize64, FormatMemorySize(p.PeakPagedMemorySize64));

        }
        static string PeakWorkingSet64 = "Peak physical memory used: {0}";
        static string TotalProcessorTime = "Total CPU time taken: {0}";
        static string PeakVirtualMemorySize64= "Peak virtual memory used: {0}";
        static string PeakPagedMemorySize64 = "Peak memory in the virtual memory paging file used: {0}";
        /// <summary>
        /// Formats the specified memory in bytes to appropriate string.
        /// for example, 
        ///  if the value is less than one KB then it returns a string representing memory in bytes.
        ///  if the value is less than one MB then it returns a string representing memory in KB.
        ///  if the value is less than one GB then it returns a string representing memory in MB.
        ///  else it returns memory in GB.
        /// </summary>
        /// <param name="value">value in bytes</param>
        private static string FormatMemorySize(long value)
        {
            string result = null;
            if (value > GB)
            {
                result = (Math.Round(value / GB, 2)).ToString() + " GB";
            }
            else if (value > MB)
            {
                result = (Math.Round(value / MB, 2)).ToString() + " MB";
            }
            else if (value > KB)
            {
                result = (Math.Round(value / KB, 2)).ToString() + " KB";
            }
            else
            {
                result = value.ToString() + " Bytes";
            }

            return result;
        }

        
    }
}
