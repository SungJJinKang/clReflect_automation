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

        public string DEFAULT_COMPILER_OPTION = "-D__clcpp_parse__ ";

        void StartProcess_clScan(in List<string> sourcefile_file_directory, in string destinationDirectory, in string compiler_option)
        {
            Process process = new Process();

            // Stop the process from opening a new window
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // Setup executable and parameters
            //process.StartInfo.FileName = clscan_exe_path;

            var sb = new System.Text.StringBuilder();
            foreach (string sourceFilePath in sourcefile_file_directory)
            {
                sb.Append(sourceFilePath + ' ');
            }

            process.StartInfo.Arguments = sb.ToString() + "--output " + destinationDirectory
                + " -- " + DEFAULT_COMPILER_OPTION + compiler_option;
            
            // Go
            process.Start();

            process.WaitForExit();
        }



        void clScan()
        {
            /*
            Thread thread = new Thread(
                () => StartProcess_clScan("", "")
            );

            thread.Start();
            */



            Process process = new Process();

            // Stop the process from opening a new window
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // Setup executable and parameters
            process.StartInfo.FileName = @"c:\test.exe";
            process.StartInfo.Arguments = "--test";

            // Go
            process.Start();

        }

        



      


        static void Main(string[] args)
        {
            Program.VCXPROJ_FILE_PATH = args[0];
            Program.TARGET_CONFIGURATION = args[1];
            Program.TARGET_PATFORM = args[2];

            Program.VCXPROJ_FILE_TEXT = System.IO.File.ReadAllText(Program.VCXPROJ_FILE_PATH);

            string additionalDirectories = ParseAdditionalDirectories.GetAdditionalPaths();
            string SourceFileDirectories = ParseSourceFileDirectories.GetSourceFileDirectories();

            return;
        }
    }
}
