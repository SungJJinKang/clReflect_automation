using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;

namespace clReflect_automation
{
    class Program
    {
        public static string VCXPROJ_FILE_PATH;
        public static string VCXPROJ_FILE_TEXT;
        public static string TARGET_CONFIGURATION;
        public static string TARGET_PATFORM;
        public static string CL_SCAN_FILE_PATH;
        public static string CL_EXPORT_FILE_PATH;

        public const string DEFAULT_COMPILER_OPTION = "-D__clcpp_parse__ -w";
        public static string ADDITIONAL_COMPILER_OPTION = "";
        public const string DEFAULT_CL_SCAN_OUT_FILE_NAME = "clReflectCompialationData";



        private static string GetclScanOutputPath()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(Path.GetDirectoryName(VCXPROJ_FILE_PATH));
            sb.Append('\\');
            sb.Append(DEFAULT_CL_SCAN_OUT_FILE_NAME);
            sb.Append(".csv");
            return sb.ToString();
        }

        private static string GetclExportOutputPath()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(Path.GetDirectoryName(VCXPROJ_FILE_PATH));
            sb.Append('\\');
            sb.Append(DEFAULT_CL_SCAN_OUT_FILE_NAME);
            sb.Append(".cppbin");
            return sb.ToString();
        }

        private static void clExport()
        {
            Process process = new Process();

            // Stop the process from opening a new window
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;

            // Setup executable and parameters
            process.StartInfo.FileName = CL_EXPORT_FILE_PATH;

            var sb = new System.Text.StringBuilder();
            sb.Append(GetclScanOutputPath());
            sb.Append(" -cpp ");
            sb.Append(GetclExportOutputPath());

            process.StartInfo.Arguments = sb.ToString();

            Console.WriteLine("Start to clExport");

            // Go
            bool isProcessStarted = process.Start();
            if (isProcessStarted == false)
            {
                throw new Exception(String.Format("Fail to start clExport ( File Path : {0} )", CL_EXPORT_FILE_PATH));
            }

            process.WaitForExit();

            Console.WriteLine("clScan is finished ( {0} )", GetclExportOutputPath());

        }

        private static void clScan(in string sourceFiles, in string additionalDirectories)
        {
            Process process = new Process();

            // Stop the process from opening a new window
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;

            // Setup executable and parameters
            process.StartInfo.FileName = CL_SCAN_FILE_PATH;

            var sb = new System.Text.StringBuilder();
            sb.Append(sourceFiles);
            sb.Append(" --output ");
            sb.Append(GetclScanOutputPath());
            sb.Append(" -- ");
            sb.Append(DEFAULT_COMPILER_OPTION);
            sb.Append(' ');
            sb.Append(additionalDirectories);
            sb.Append(' ');
            sb.Append(ADDITIONAL_COMPILER_OPTION);

            process.StartInfo.Arguments = sb.ToString();

            Console.WriteLine("Start to clScan");

            bool isProcessStarted = process.Start();
            if(isProcessStarted == false)
            {
                throw new Exception(String.Format("Fail to start clScan ( File Path : {0} )", CL_SCAN_FILE_PATH));
            }

            process.WaitForExit();
                
            Console.WriteLine("clScan is finished ( {0} )", GetclScanOutputPath());
        }

        



      


        static void Main(string[] args)
        {
            Program.CL_SCAN_FILE_PATH = args[0];
            Program.CL_EXPORT_FILE_PATH = args[1];

            Program.VCXPROJ_FILE_PATH = args[2];
            Program.TARGET_CONFIGURATION = args[3];
            Program.TARGET_PATFORM = args[4];

            for (int i = 5; i < args.Length; i++)
            {
                Program.ADDITIONAL_COMPILER_OPTION += args[i] + ' ';
            }
            Program.ADDITIONAL_COMPILER_OPTION.Trim();

            Program.VCXPROJ_FILE_TEXT = System.IO.File.ReadAllText(Program.VCXPROJ_FILE_PATH);



            string SourceFileDirectories = ParseSourceFileDirectories.GetSourceFileDirectories();
            string additionalDirectories = ParseAdditionalDirectories.GetAdditionalPaths();

            clScan(SourceFileDirectories, additionalDirectories);
            clExport();

            return;
        }
    }
}
