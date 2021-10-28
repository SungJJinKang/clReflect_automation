using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

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
        public static string CL_MERGE_FILE_PATH;

        public const string DEFAULT_COMPILER_OPTION = "-D__clcpp_parse__ -w";
        public static string ADDITIONAL_COMPILER_OPTION = "";
        public const string DEFAULT_CL_SCAN_OUT_FILE_NAME = "clReflectCompialationData";

        private const int DEFAULT_THREAD_COUNT = 4;

        private static string GetclScanOutputPath()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(Path.GetDirectoryName(VCXPROJ_FILE_PATH));
            sb.Append('\\');
            sb.Append(DEFAULT_CL_SCAN_OUT_FILE_NAME);
            sb.Append(".csv");
            return sb.ToString();
        }

        private static string GetclScanOutputPath(in int index)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(Path.GetDirectoryName(VCXPROJ_FILE_PATH));
            sb.Append('\\');
            sb.Append(DEFAULT_CL_SCAN_OUT_FILE_NAME);
            sb.Append(index.ToString());
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

        private static void clMerge(in int inputCount)
        {
            Process process = new Process();

            // Stop the process from opening a new window
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;

            // Setup executable and parameters
            process.StartInfo.FileName = CL_MERGE_FILE_PATH;

            var sb = new System.Text.StringBuilder();
            sb.Append(GetclScanOutputPath());
            sb.Append(' ');
            for(int i = 0; i < inputCount; i++)
            {
                sb.Append(GetclScanOutputPath(i));
                sb.Append(' ');
            }

            process.StartInfo.Arguments = sb.ToString();

            Console.WriteLine("Start to clMerge");

            // Go
            bool isProcessStarted = process.Start();
            if (isProcessStarted == false)
            {
                throw new Exception(String.Format("Fail to start clMerge ( File Path : {0} )", process.StartInfo.FileName));
            }

            process.WaitForExit();

            ChildProcessTracker.AddProcess(process);

            int result = process.ExitCode;
            if(result != 0)
            {
                throw new Exception(String.Format("Fail to start clMerge ( File Path : {0} )", process.StartInfo.FileName));
            }
            Console.WriteLine("clMerge is finished ( {0} )", GetclScanOutputPath());

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
                throw new Exception(String.Format("Fail to start clExport ( File Path : {0} )", process.StartInfo.FileName));
            }

            process.WaitForExit();

            ChildProcessTracker.AddProcess(process);

            int result = process.ExitCode;
            if (result != 0)
            {
                throw new Exception(String.Format("Fail to start clExport ( File Path : {0} )", process.StartInfo.FileName));
            }
            Console.WriteLine("clScan is finished ( {0} )", GetclExportOutputPath());

        }

        public struct clScanParameter
        {

            public string sourceFiles;
            public string additionalDirectories;
            public int index;

        }

        public static Process clScan_internal(clScanParameter _clScanParameter)
        {
            Process process = new Process();

            // Stop the process from opening a new window
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;

            // Setup executable and parameters
            process.StartInfo.FileName = CL_SCAN_FILE_PATH;

            var sb = new System.Text.StringBuilder();
            sb.Append(_clScanParameter.sourceFiles);
            sb.Append(" --output ");
            sb.Append(GetclScanOutputPath(_clScanParameter.index));
            sb.Append(" -- ");
            sb.Append(DEFAULT_COMPILER_OPTION);
            sb.Append(' ');
            sb.Append(_clScanParameter.additionalDirectories);
            sb.Append(' ');
            sb.Append(ADDITIONAL_COMPILER_OPTION);

            process.StartInfo.Arguments = sb.ToString();

            Console.WriteLine("Start to clScan");

            bool isProcessStarted = process.Start();

            ChildProcessTracker.AddProcess(process);


            if (isProcessStarted == false)
            {
                throw new Exception(String.Format("Fail to start clScan ( File Path : {0} )", process.StartInfo.FileName));
            }


            return process;

         
        }

        private static void clScanMultiProcess(in string[] sourceFiles, in string additionalDirectories)
        {
            Process[] t = new Process[sourceFiles.Length];

            for (int i = 0; i < sourceFiles.Length; i++)
            {
                clScanParameter _clScanParameter = new clScanParameter();
                _clScanParameter.sourceFiles = sourceFiles[i];
                _clScanParameter.additionalDirectories = additionalDirectories;
                _clScanParameter.index = i;

                t[i] = clScan_internal(_clScanParameter);
            }

            for (int i = 0; i < sourceFiles.Length; i++)
            {
                t[i].WaitForExit();

                int result = t[i].ExitCode;
                if (result != 0)
                {
                    throw new Exception(String.Format("Fail to start clScan ( File Path : {0} ) - indexNum : {1}", t[i].StartInfo.FileName, i));
                }

                Console.WriteLine("clScan is finished ( {0} )", i);
            }
        }




        static void Main(string[] args)
        {
            Program.CL_SCAN_FILE_PATH = args[0];
            Program.CL_MERGE_FILE_PATH = args[1];
            Program.CL_EXPORT_FILE_PATH = args[2];

            Program.VCXPROJ_FILE_PATH = args[3];
            Program.TARGET_CONFIGURATION = args[4];
            Program.TARGET_PATFORM = args[5];

            for (int i = 6; i < args.Length; i++)
            {
                Program.ADDITIONAL_COMPILER_OPTION += args[i] + ' ';
            }
            Program.ADDITIONAL_COMPILER_OPTION.Trim();

            Program.VCXPROJ_FILE_TEXT = System.IO.File.ReadAllText(Program.VCXPROJ_FILE_PATH);



            string[] SourceFileDirectories = ParseSourceFileDirectories.GetSourceFileDirectories(DEFAULT_THREAD_COUNT);
            string additionalDirectories = ParseAdditionalDirectories.GetAdditionalPaths();

            clScanMultiProcess(SourceFileDirectories, additionalDirectories);
            clMerge(SourceFileDirectories.Length);

            clExport();

            return;
        }
    }
}
