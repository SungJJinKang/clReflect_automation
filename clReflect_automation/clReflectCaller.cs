using net.r_eg.Conari;
using net.r_eg.Conari.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace clReflect_automation
{
    class clReflectCaller
    {
        public static void clExport()
        {
            Process process = new Process();

            // Stop the process from opening a new window
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;

            // Setup executable and parameters
            process.StartInfo.FileName = Program.CL_EXPORT_FILE_PATH;

            var sb = new System.Text.StringBuilder();
            sb.Append(DirectoryHelper.GetclMergeOutputPath());
            sb.Append(" -cpp ");
            sb.Append(DirectoryHelper.GetclExportOutputPath());

            process.StartInfo.Arguments = sb.ToString();

            Console.WriteLine("Start to clExport");

            // Go
            bool isProcessStarted = process.Start();
            if (isProcessStarted == false)
            {
                throw new Exception(String.Format("Fail to start clExport ( export.exe FilePath : {0} )", process.StartInfo.FileName));
            }
            ChildProcessTracker.AddProcess(process);

            while (process.StandardOutput.EndOfStream == false)
            {
                string line = process.StandardOutput.ReadLine();

                Console.WriteLine("clExport Log ( Target Database File Path : {0} ) : {1}", DirectoryHelper.GetclMergeOutputPath(), line);
           
                if (process.HasExited == true)
                {
                    Console.WriteLine("clExport Log ( Target Database File Path : {0} ) : {1}", DirectoryHelper.GetclMergeOutputPath(), line);
                    break;
                }
            }

            int result = process.ExitCode;
            if (result != 0)
            {
                throw new Exception(String.Format("clExport Fail! ( Exported DataBase File Path : {0} )", DirectoryHelper.GetclMergeOutputPath()));
            }
            else
            {
                Console.WriteLine("clExport Success!!! ( Exported Output File Path : {0} )", DirectoryHelper.GetclExportOutputPath());
            }

        }

        public static void clMerge(in List<string> clScanOutputFilePathes)
        {
            Process process = new Process();

            // Stop the process from opening a new window
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;

            // Setup executable and parameters
            process.StartInfo.FileName = Program.CL_MERGE_FILE_PATH;

            var sb = new System.Text.StringBuilder();
            sb.Append(DirectoryHelper.GetclMergeOutputPath());
            sb.Append(' ');
            for (int i = 0; i < clScanOutputFilePathes.Count; i++)
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

            while (process.StandardOutput.EndOfStream == false)
            {
                string line = process.StandardOutput.ReadLine();

                Console.WriteLine("clMerge Log : {0}", line);

                if (process.HasExited == true)
                {
                    Console.WriteLine("clMerge Log : {0}", line);
                    break;
                }
            }

            int result = process.ExitCode;
            if (result != 0)
            {
                throw new Exception("clMerge Fail!!");
            }
            else
            {
                Console.WriteLine("clMerge Success!! ( Merged File Path : {0} )", DirectoryHelper.GetclMergeOutputPath());
            }

        }

        public struct clScanParameter
        {
            public string sourceFilePath;
            public string outputFilePath;
            public string additionalDirectories;

        }

        private static String GetClScanArgv(clScanParameter _clScanParameter)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(_clScanParameter.sourceFilePath);
            sb.Append(" --output ");
            sb.Append(_clScanParameter.outputFilePath);
            sb.Append(" -- ");
            sb.Append(Program.DEFAULT_COMPILER_OPTION);
            sb.Append(' ');
            sb.Append(_clScanParameter.additionalDirectories);
            sb.Append(' ');
            sb.Append(Program.ADDITIONAL_COMPILER_OPTION);
            
            return sb.ToString();
        }

        public static ConariL clScanConariL = null;

       

        public static void clScan_internal(clScanParameter _clScanParameter)
        {
            DirectoryHelper.eClreflectFileExtension clScanFileExtension = DirectoryHelper.GetClreflectFileExtension(Program.CL_SCAN_FILE_PATH);

            Console.WriteLine("Start to clScan ( clScan File Path : {0} )", Program.CL_SCAN_FILE_PATH);

            int result = 1;
            
            switch (clScanFileExtension)
            {
                case DirectoryHelper.eClreflectFileExtension.EXE:

                    Process process = new Process();

                    // Stop the process from opening a new window
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = false;

                    // Setup executable and parameters
                    process.StartInfo.FileName = Program.CL_SCAN_FILE_PATH;
                    process.StartInfo.Arguments = GetClScanArgv(_clScanParameter);

                    bool isProcessStarted = process.Start();
                    if (isProcessStarted == false)
                    {
                        throw new Exception(String.Format("Fail to start clScan ( exScan.exe FilePath: {0} )", process.StartInfo.FileName));
                    }
                    ChildProcessTracker.AddProcess(process);


                    while (process.StandardOutput.EndOfStream == false)
                    {
                        string line = process.StandardOutput.ReadLine();

                        Console.WriteLine("clScan Log ( Target Source File Path : {0} ) : {1}", _clScanParameter.sourceFilePath, line);

                        if (process.HasExited == true)
                        {
                            Console.WriteLine("clScan Log ( Target Source File Path : {0} ) : {1}", _clScanParameter.sourceFilePath, line);
                            break;
                        }
                    }

                    result = process.ExitCode;
                   

                    break;


                    

                case DirectoryHelper.eClreflectFileExtension.DLL:

                    DLLHelper.LoadDLLToConariL(ref clScanConariL, Program.CL_SCAN_FILE_PATH);

                    byte[] argvASCIIStr = ASCIIEncoding.ASCII.GetBytes(GetClScanArgv(_clScanParameter));

                    try
                    {
                        result = clScanConariL.DLR.c_clscan<int>(ref argvASCIIStr);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(String.Format("Exception occurred for clscaning file ( File Path : {0}, Error Message : {1} )", _clScanParameter.sourceFilePath, e.Message), "Exception!!!!"); // fails here
                    }

                    break;


                default:
                    throw new Exception(String.Format("Fail To Find clscan file ( Path : {0} )", Program.CL_SCAN_FILE_PATH).ToString());
            }

            if (result != 0)
            {
                MessageBox.Show(String.Format("Fail to clsan file ( File Path : {0} )", _clScanParameter.sourceFilePath), "Exception!!!!"); // fails here
            }
            else
            {
                Console.WriteLine("Success to clScan! ( output File Path :  {0} )", _clScanParameter.outputFilePath);
            }


        }

        public static List<string> clScanSourceFiles(in List<string> sourceFiles, in string additionalDirectories)
        {
            List<string> clscanOutPutFiles = new List<string>();

            for (int i = 0; i < sourceFiles.Count; i++)
            {
                if (sourceFiles[i] != "")
                {

                    clScanParameter _clScanParameter = new clScanParameter();
                    _clScanParameter.sourceFilePath = sourceFiles[i];
                    _clScanParameter.outputFilePath = DirectoryHelper.GetclScanOutputPath(sourceFiles[i]);
                    _clScanParameter.additionalDirectories = additionalDirectories;

                    clscanOutPutFiles.Add(_clScanParameter.outputFilePath);

                    clScan_internal(_clScanParameter);


                }
            }

            DLLHelper.UnLoadDLLFromConariL(ref clScanConariL);

            return clscanOutPutFiles;
        }
    }
}
