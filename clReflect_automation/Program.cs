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

        private static string GetclMergeOutputPath()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(Path.GetDirectoryName(VCXPROJ_FILE_PATH));
            sb.Append('\\');
            sb.Append(DEFAULT_CL_SCAN_OUT_FILE_NAME);
            sb.Append("_merged");
            sb.Append(".csv");
            return sb.ToString();
        }

        private static string GetclScanOutputPath(in string path)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(Path.GetDirectoryName(path));
            sb.Append('\\');
            sb.Append(Path.GetFileNameWithoutExtension(path));
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

        private static void clMerge(in List<string> clScanOutputFilePathes)
        {
            Process process = new Process();

            // Stop the process from opening a new window
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;

            // Setup executable and parameters
            process.StartInfo.FileName = CL_MERGE_FILE_PATH;

            var sb = new System.Text.StringBuilder();
            sb.Append(GetclMergeOutputPath());
            sb.Append(' ');
            for(int i = 0; i < clScanOutputFilePathes.Count; i++)
            {
                sb.Append(clScanOutputFilePathes[i]);
                sb.Append(' ');
            }

            process.StartInfo.Arguments = sb.ToString();

            Console.WriteLine("Start to clMerge");

            // Go
            bool isProcessStarted = process.Start();
            if (isProcessStarted == false)
            {
                throw new Exception(String.Format("Fail to start clMerge ( clMerge.exe FilePath : {0} )", process.StartInfo.FileName));
            }
            ChildProcessTracker.AddProcess(process);

            while (process.HasExited == false)
            {
                if (process.StandardOutput.EndOfStream == false)
                {
                    string line = process.StandardOutput.ReadLine();

                    Console.WriteLine("clMerge Log : {0}", line);
                }
            }

            int result = process.ExitCode;
            if(result != 0)
            {
                throw new Exception("clMerge Fail!!");
            }
            else
            {
                Console.WriteLine("clMerge Success!! ( Merged File Path : {0} )", GetclMergeOutputPath());
            }

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
            sb.Append(GetclMergeOutputPath());
            sb.Append(" -cpp ");
            sb.Append(GetclExportOutputPath());

            process.StartInfo.Arguments = sb.ToString();

            Console.WriteLine("Start to clExport");

            // Go
            bool isProcessStarted = process.Start();
            if (isProcessStarted == false)
            {
                throw new Exception(String.Format("Fail to start clExport ( export.exe FilePath : {0} )", process.StartInfo.FileName));
            }
            ChildProcessTracker.AddProcess(process);

            while (process.HasExited == false)
            {
                if (process.StandardOutput.EndOfStream == false)
                {
                    string line = process.StandardOutput.ReadLine();

                    Console.WriteLine("clExport Log ( Target Database File Path : {0} ) : {1}", GetclMergeOutputPath(), line);
                }
            }

            int result = process.ExitCode;
            if (result != 0)
            {
                throw new Exception(String.Format("clExport Fail! ( Exported DataBase File Path : {0} )", GetclMergeOutputPath()));
            }
            else
            {
                Console.WriteLine("clExport Success!!! ( Exported Output File Path : {0} )", GetclExportOutputPath());
            }

        }

        public struct clScanParameter
        {
            public string sourceFilePath;
            public string outputFilePath;
            public string additionalDirectories;

        }

        public static void clScan_internal(clScanParameter _clScanParameter)
        {
            Process process = new Process();

            // Stop the process from opening a new window
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;

            // Setup executable and parameters
            process.StartInfo.FileName = CL_SCAN_FILE_PATH;

            var sb = new System.Text.StringBuilder();
            sb.Append(_clScanParameter.sourceFilePath);
            sb.Append(" --output ");
            sb.Append(_clScanParameter.outputFilePath);
            sb.Append(" -- ");
            sb.Append(DEFAULT_COMPILER_OPTION);
            sb.Append(' ');
            sb.Append(_clScanParameter.additionalDirectories);
            sb.Append(ADDITIONAL_COMPILER_OPTION);

            process.StartInfo.Arguments = sb.ToString();

            Console.WriteLine("Start to clScan ( {0} )", _clScanParameter.sourceFilePath);

            bool isProcessStarted = process.Start();
            if (isProcessStarted == false)
            {
                throw new Exception(String.Format("Fail to start clScan ( exScan.exe FilePath: {0} )", process.StartInfo.FileName));
            }
            ChildProcessTracker.AddProcess(process);

            while (process.HasExited == false)
            {
                if(process.StandardOutput.EndOfStream == false)
                {
                    string line = process.StandardOutput.ReadLine();

                    Console.WriteLine("clScan Log ( Target Source File Path : {0} ) : {1}", _clScanParameter.sourceFilePath, line);
                }
            }

            int result = process.ExitCode;
            if (result != 0)
            {
                throw new Exception(String.Format("clScane Fail! ( Parsed Source FilePath : {0} )", _clScanParameter.sourceFilePath));
            }
            else
            {

                Console.WriteLine("Success to clScan! ( output File Path :  {0} )", _clScanParameter.outputFilePath);
            }

        }

        private static List<string> clScanSourceFiles(in List<string> sourceFiles, in string additionalDirectories)
        {
            List<string> clscanOutPutFiles = new List<string>();

            for (int i = 0; i < sourceFiles.Count; i++)
            {
                if(sourceFiles[i] != "")
                {

                    clScanParameter _clScanParameter = new clScanParameter();
                    _clScanParameter.sourceFilePath = sourceFiles[i];
                    _clScanParameter.outputFilePath = GetclScanOutputPath(sourceFiles[i]);
                    _clScanParameter.additionalDirectories = additionalDirectories;

                    clscanOutPutFiles.Add(_clScanParameter.outputFilePath);

                    clScan_internal(_clScanParameter);

                    
                }
            }

            return clscanOutPutFiles;
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



            List<string> SourceFileDirectories = ParseSourceFileDirectories.GetSourceFileDirectories(DEFAULT_THREAD_COUNT);
            string additionalDirectories = ParseAdditionalDirectories.GetAdditionalPaths();

            List<string> clScanOutFilePathes = clScanSourceFiles(SourceFileDirectories, additionalDirectories);
            clMerge(clScanOutFilePathes);

            clExport();

            return;
        }
    }
}
