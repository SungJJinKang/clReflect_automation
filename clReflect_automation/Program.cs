using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

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


        private const string DEFAULT_COMPILER_DEBUG_LOG_OPTION = "-v";

        public const string DEFAULT_COMPILER_OPTION = "-D__clcpp_parse__ -v -w -W0 -D_SCL_SECURE_NO_WARNINGS -D_CRT_SECURE_NO_WARNINGS";
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

            StringBuilder sb = new StringBuilder();
            if (Program.TARGET_PATFORM == "x64")
            {

                sb.Append(@"-D_WIN64 ");
                sb.Append(@"-D__LP64__ ");
                sb.Append(@"-m64 ");
            }
            else
            {
                sb.Append("-m32 ");
            }

            for (int i = 6; i < args.Length; i++)
            {
                sb.Append(args[i] + ' ');
            }
            Program.ADDITIONAL_COMPILER_OPTION = sb.ToString().Trim();

            Program.VCXPROJ_FILE_TEXT = System.IO.File.ReadAllText(Program.VCXPROJ_FILE_PATH);
        }

        static private bool isInitialized = false;
        static private void InitializeProgram()
        {
            if(isInitialized == false)
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

                AppDomain.CurrentDomain.UnhandledException +=
                    (object sender, UnhandledExceptionEventArgs e)
                    => ExceptionHelper.ShowExceptionMessageBox(e);

                Application.ThreadException +=
                    (object sender, ThreadExceptionEventArgs e)
                    => ExceptionHelper.ShowExceptionMessageBox(e);
            }
        }


        static private int Generate_clReflect_data(string[] args)
        {
            InitializeProgram();

            try
            {
                if (args.Length == 0 || args[0].Trim() == "")
                {
                    string setting_text_direcotry = Directory.GetCurrentDirectory() + "\\" + DEFAULT_SETTING_TEXT_FILENAME;

                    string setting_text_string = System.IO.File.ReadAllText(setting_text_direcotry);
                    string[] setting_text_args = setting_text_string.Split(' ');
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

                return 0;
            }
            catch (Exception e)
            {
                ExceptionHelper.ShowExceptionMessageBox(e);
                return 1;
            }
        }


        [DllExport]
        static public int c_Generate_clReflect_data(IntPtr stringPtr) // pass wchar_t** to here
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            int result = 0;
            try
            {
                String args = Marshal.PtrToStringAuto(stringPtr);
                String[] splittedStr = args.Split(' ');

                result = Generate_clReflect_data(splittedStr);
            }
            catch (Exception e)
            {
                ExceptionHelper.ShowExceptionMessageBox(e);
                result = 1;
            }
           
            return result;
        }


        static void Main(string[] args)
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);


            try
            {
                InitializeProgram();

                Generate_clReflect_data(args);
            }
            catch (Exception e)
            {
                ExceptionHelper.ShowExceptionMessageBox(e);
            }
            return;
        }

       

    }
}
