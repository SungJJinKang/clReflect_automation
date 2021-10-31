using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

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

        public const string DEFAULT_COMPILER_OPTION = "-D__clcpp_parse__ -w -D_SCL_SECURE_NO_WARNINGS -D_CRT_SECURE_NO_WARNINGS";
        public static string ADDITIONAL_COMPILER_OPTION = "";
        public const string DEFAULT_CL_SCAN_OUT_FILE_NAME = "clReflectCompialationData";

        private const string DEFAULT_SETTING_TEXT_FILENAME = "Setting.txt";
        static private void Configure(string[] args)
        {
            Program.CL_SCAN_FILE_PATH = args[0].Trim();
            Program.CL_MERGE_FILE_PATH = args[1].Trim();
            Program.CL_EXPORT_FILE_PATH = args[2].Trim();

            Program.VCXPROJ_FILE_PATH = args[3].Trim();
            Program.TARGET_CONFIGURATION = args[4].Trim();
            Program.TARGET_PATFORM = args[5].Trim();

            if (Program.TARGET_PATFORM == "x64")
            {

                Program.ADDITIONAL_COMPILER_OPTION += @"-D""_WIN64"" ";
                Program.ADDITIONAL_COMPILER_OPTION += @"-D""__LP64__"" ";
                Program.ADDITIONAL_COMPILER_OPTION += @"-m64 ";
            }
            else
            {
                Program.ADDITIONAL_COMPILER_OPTION += "-m32 ";
            }

            for (int i = 6; i < args.Length; i++)
            {
                Program.ADDITIONAL_COMPILER_OPTION += args[i] + ' ';
            }
            Program.ADDITIONAL_COMPILER_OPTION.Trim();

            Program.VCXPROJ_FILE_TEXT = System.IO.File.ReadAllText(Program.VCXPROJ_FILE_PATH);
        }


        static private void Generate_clReflect_data(string[] args)
        {
            if (args.Length == 0 || args[0].Trim() == "")
            {
                string setting_text_direcotry = Directory.GetCurrentDirectory() + "\\" + DEFAULT_SETTING_TEXT_FILENAME;

                string setting_text_string = System.IO.File.ReadAllText(setting_text_direcotry);
                string[] setting_text_args = setting_text_string.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Configure(setting_text_args);
            }
            else
            {
                Configure(args);
            }

            List<string> SourceFileDirectories = ParseSourceFileDirectories.GetSourceFileDirectories();
            string additionalDirectories = ParseAdditionalDirectories.GetAdditionalPaths();

            List<string> clScanOutFilePaths = clReflectCaller.clScanSourceFiles(SourceFileDirectories, additionalDirectories);
            clReflectCaller.clMerge(clScanOutFilePaths);
            ExportDatabaseDirectoryList.WriteDatabasePathsListToText(clScanOutFilePaths);

            clReflectCaller.clExport();
        }

        [DllExport]
        static public void c_Generate_clReflect_data(IntPtr stringPtr) // pass wchar_t** to here
        {
            String args = Marshal.PtrToStringAuto(stringPtr);
            String[] splittedStr = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Generate_clReflect_data(splittedStr);
        }



        static void Main(string[] args)
        {
            Generate_clReflect_data(args);

            return;
        }
        
    }
}
