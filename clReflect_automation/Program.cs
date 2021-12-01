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
        public struct ConfigureData
        {
            public string VCXPROJ_FILE_PATH;

            public string VCXPROJ_FILE_TEXT;

            public string TARGET_CONFIGURATION;

            public string TARGET_PLATFORM;

            public string CL_SCAN_FILE_PATH;

            public string CL_EXPORT_FILE_PATH;

            public string CL_MERGE_FILE_PATH;

            /// <summary>
            /// end with '\\'
            /// </summary>
            public string DEPENDENCY_FILES_FOLDER;

            public string ADDITIONAL_COMPILER_OPTION;


            public const string DEFAULT_COMPILER_OPTION = "-D__clcpp_parse__ -w -W0 -D_SCL_SECURE_NO_WARNINGS -D_CRT_SECURE_NO_WARNINGS";

            public string CL_SCAN_OUT_FILE_NAME;
            public const string DEFAULT_CL_SCAN_OUT_FILE_NAME = "clReflectCompialationData";

            public const string CL_COMPILETIME_GETTYPE_FILE_NAME = "clreflect_compiletime_gettype.cpp";

            public const string DEFAULT_SETTING_TEXT_FILENAME = "Setting.txt";

            public string ROOTCLASS_TYPENAME;
        }

        

        static private void Configure(ref ConfigureData configureData, string[] args)
        {
            configureData.CL_SCAN_FILE_PATH = args[0].Trim();
            Console.WriteLine("CL_SCAN_FILE_PATH : {0}", configureData.CL_SCAN_FILE_PATH);

            configureData.CL_MERGE_FILE_PATH = args[1].Trim();
            Console.WriteLine("CL_MERGE_FILE_PATH : {0}", configureData.CL_MERGE_FILE_PATH);

            configureData.CL_EXPORT_FILE_PATH = args[2].Trim();
            Console.WriteLine("CL_EXPORT_FILE_PATH : {0}", configureData.CL_EXPORT_FILE_PATH);

            configureData.VCXPROJ_FILE_PATH = args[3].Trim();
            configureData.TARGET_CONFIGURATION = args[4].Trim();
            configureData.TARGET_PLATFORM = args[5].Trim();

            for(int i = 0; i < args.Length; i++)
            {
                if(args[i].StartsWith("-SD"))
                {
                    configureData.DEPENDENCY_FILES_FOLDER = args[i].Substring("-SD".Length);
                    if (configureData.DEPENDENCY_FILES_FOLDER.EndsWith("\\") == false)
                    {
                        configureData.DEPENDENCY_FILES_FOLDER += "\\";
                    }

                    args[i] = "";
                    break;
                }
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-ROOTCLASS_TYPENAME"))
                {
                    configureData.ROOTCLASS_TYPENAME = args[i].Substring("-ROOTCLASS_TYPENAME".Length);

                    args[i] = "";
                    break;
                }
            }

            bool is_Found_REFLECTION_BINARY_FILENAME = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-REFLECTION_BINARY_FILENAME"))
                {
                    configureData.CL_SCAN_OUT_FILE_NAME = args[i].Substring("-REFLECTION_BINARY_FILENAME".Length);

                    is_Found_REFLECTION_BINARY_FILENAME = true;
                    args[i] = "";
                    break;
                }
            }
            if(is_Found_REFLECTION_BINARY_FILENAME == false)
            {
                configureData.CL_SCAN_OUT_FILE_NAME = ConfigureData.DEFAULT_CL_SCAN_OUT_FILE_NAME;
            }

            StringBuilder sb = new StringBuilder();
            if (configureData.TARGET_PLATFORM == "x64")
            {
                sb.Append("-D_WIN64 ");
                sb.Append("-DLP64 ");
                sb.Append("-m64 ");
            }
            else
            {
                sb.Append("-m32 ");
            }

            for (int i = 6; i < args.Length; i++)
            {
                if(args[i].Length != 0)
                {
                    sb.Append(args[i] + ' ');
                }
            }
            configureData.ADDITIONAL_COMPILER_OPTION = sb.ToString().Trim();

            configureData.VCXPROJ_FILE_TEXT = System.IO.File.ReadAllText(configureData.VCXPROJ_FILE_PATH);
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

            ConfigureData configureData = new ConfigureData();

            try
            {
                if (args.Length == 0 || args[0].Trim() == "")
                {
                    string setting_text_direcotry = Directory.GetCurrentDirectory() + "\\" + ConfigureData.DEFAULT_SETTING_TEXT_FILENAME;

                    string setting_text_string = System.IO.File.ReadAllText(setting_text_direcotry);
                    string[] setting_text_args = setting_text_string.Split(' ');
                    Configure(ref configureData, setting_text_args);
                }
                else
                {
                    Configure(ref configureData, args);
                }

                List<string> SourceFileDirectories = ParseSourceFileDirectories.GetSourceFileDirectories(configureData);
               
                if(SourceFileDirectories.Count == 0)
                {
                    return 0;
                }
                
                string additionalDirectories = ParseAdditionalDirectories.GetAdditionalPaths(configureData);

                clReflectCaller _clReflectCaller = new clReflectCaller();

                List<string> clScanOutFilePaths = _clReflectCaller.clScanSourceFiles(configureData, SourceFileDirectories, additionalDirectories);
                _clReflectCaller.clMerge(configureData, clScanOutFilePaths);

                _clReflectCaller.clExport(configureData);

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

            System.GC.Collect();
           
            return result;
        }


        static void Main(string[] args)
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            Console.Write("Arguments : ");
            for (int i = 0; i < args.Length; i++)
            {
                Console.Write("{0} ", args[i]);
            }
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
