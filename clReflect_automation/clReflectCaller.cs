using net.r_eg.Conari;
using net.r_eg.Conari.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace clReflect_automation
{
    class clReflectCaller
    {
        private static ConariL clScanConariL = null;
        private static ConariL clMergeConariL = null;
        private static ConariL clExportConariL = null;

        private static bool FindMapFileInProjectFolder(ref String mapFilePath)
        {
            string[] files = System.IO.Directory.GetFiles(Path.GetDirectoryName(Program.VCXPROJ_FILE_PATH), "*.map");
            if(files.Length > 0)
            {
                mapFilePath = DirectoryHelper.GetFileDirectoryInProjectFolder(files[0]);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string GetclExportArguments()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(DirectoryHelper.GetclMergeOutputPath());
            sb.Append(" -cpp ");
            sb.Append(DirectoryHelper.GetclExportOutputPath());

            String mapFilePath = "";
            if(FindMapFileInProjectFolder(ref mapFilePath) == true)
            {
                sb.Append(" -map ");
                sb.Append(mapFilePath);

                Console.WriteLine("Find .map File!!!! ( Map File Path : {0} )", mapFilePath);
            }
            else
            {
                Console.WriteLine(".map File not found");
            }


            return sb.ToString();
        }
        public static void clExport()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            int result = 1;

            DLLHelper.LoadDLLToConariL(ref clExportConariL, Program.CL_EXPORT_FILE_PATH);
            try
            {
                if (clExportConariL != null)
                {
                    string arvs = GetclExportArguments();
                    NativeString<CharPtr> unmanagedStringArgv = new NativeString<CharPtr>(arvs);

                    result = clExportConariL.DLR.c_clexport<int>(unmanagedStringArgv);

                    unmanagedStringArgv.Dispose();
                }
                else
                {
                    throw new Exception("clExportConariL is null");
                }
            }
            catch (Exception e)
            {
                ExceptionHelper.ShowExceptionMessageBox(e);
            }

            DLLHelper.UnLoadDLLFromConariL(ref clExportConariL);

            if (result != 0)
            {
                MessageBox.Show(String.Format("Fail to clexport file ( Error Code : {0} )", result), "FAIL clExport"); // fails here
            }
            else
            {
                Console.WriteLine("clExport Success!!! ( Exported Output File Path : {0} )", DirectoryHelper.GetclExportOutputPath());
            }

            stopWatch.Stop();
            Console.WriteLine("clScan takes {0}m {1}s", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds);
        }

        private static string GetclMergeArguments(in List<string> clScanOutputFilePathes)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(DirectoryHelper.GetclMergeOutputPath());
            sb.Append(" -cpp_codegen ");
            sb.Append(DirectoryHelper.GetFileDirectoryInProjectFolder(Program.DEFAULT_CL_COMPILETIME_GETTYPE_FILE_NAME));
            sb.Append(" ");

            for (int i = 0; i < clScanOutputFilePathes.Count; i++)
            {
                sb.Append(clScanOutputFilePathes[i]);
                sb.Append(' ');
            }

            sb.Length = sb.Length - 1;

            return sb.ToString();
        }

        public static void clMerge(in List<string> clScanOutputFilePathes)
        {
            if(clScanOutputFilePathes.Count == 0)
            {
                return;
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            int result = 1;

            DLLHelper.LoadDLLToConariL(ref clMergeConariL, Program.CL_MERGE_FILE_PATH);

            try
            {
                if (clMergeConariL != null)
                {
                    string arvs = GetclMergeArguments(clScanOutputFilePathes);
                    NativeString<CharPtr> unmanagedStringArgv = new NativeString<CharPtr>(arvs);

                    result = clMergeConariL.DLR.c_clmerge<int>(unmanagedStringArgv);

                    unmanagedStringArgv.Dispose();
                }
                else
                {
                    throw new Exception("clScanConariL is null");
                }
            }
            catch (Exception e)
            {
                ExceptionHelper.ShowExceptionMessageBox(e);
            }

            DLLHelper.UnLoadDLLFromConariL(ref clMergeConariL);

            if (result != 0)
            {
                MessageBox.Show(String.Format("Fail to clmerge file ( Error Code : {0} )", result), "FAIL clmerge"); // fails here
            }
            else
            {
                Console.WriteLine("clMerge Success!! ( Merged File Path : {0} )", DirectoryHelper.GetclMergeOutputPath());
            }

            stopWatch.Stop();
            Console.WriteLine("clScan takes {0}m {1}s", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds);
        }

        

        private static void GenerateClScanArguments(ref clScanParameter _clScanParameter)
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

            _clScanParameter.arguments = sb.ToString();

            //sb.Append(" -MD"); // clangtooling이 dependency file 관련된 argument를 무시하는 옵션을 강제로 넣는다. https://intel.github.io/llvm-docs/clang_doxygen/classclang_1_1tooling_1_1StandaloneToolExecutor.html
            /*
            sb.Append(" -MT");
            sb.Append(Path.GetDirectoryName(_clScanParameter.sourceFilePath));
            sb.Append('\\');
            sb.Append(Path.GetFileNameWithoutExtension(_clScanParameter.sourceFilePath));
            sb.Append(".mk");
            */
        }

        private class clScanParameter
        {
            public string sourceFilePath;
            public string outputFilePath;
            public string additionalDirectories;
            public string arguments;
        }

        private static object clscanLockObj = new object();
        private static void clScan_internal(in clScanParameter _clScanParameter, in int completedSourceFileCount, in int totalSourceFileCount)
        {
            
            int result = 1;

           
            try
            {
                if (clScanConariL != null)
                {
                    NativeString<CharPtr> unmanagedStringArgv = new NativeString<CharPtr>(_clScanParameter.arguments);
             
                    Console.WriteLine("Start to clScan ( Target SourceFile File Path : {0} )", _clScanParameter.sourceFilePath);
                    result = clScanConariL.DLR.c_clscan<int>(unmanagedStringArgv);

                    unmanagedStringArgv.Dispose();
                }
                else
                {
                    throw new Exception("clScanConariL is null");
                }
            }
            catch (Exception e)
            {
                ExceptionHelper.ShowExceptionMessageBox(e);
            }

            if (result != 0)
            {
                Monitor.Enter(clscanLockObj);
                MessageBox.Show(String.Format("Fail to clscan file ( Error Code : {0} )", result), "FAIL clscan"); // fails here
                Monitor.Exit(clscanLockObj);
            }
            else
            {
                Console.WriteLine
                    (
                    "Success to clScan! ( output File Path :  {0}, Completion : {1} / {2} )", 
                    _clScanParameter.outputFilePath, (completedSourceFileCount + 1).ToString(), totalSourceFileCount.ToString()
                    );
            }


        }



        public static int MAX_CLSCAN_THREAD_COUNT = Math.Max(8, Environment.ProcessorCount);

        static void clscan_multithread
        (
            in List<string> sourceFilePathList,
            in List<clScanParameter> clscanParameterList,
            ref int currentSourceFileCount, 
            ref int finishedSourceFileCount,
            in int totalSourceFileCount
        )
        {
            while (true)
            {
                int currentSourceFileIndex = Interlocked.Increment(ref currentSourceFileCount);
                if(currentSourceFileIndex >= totalSourceFileCount)
                {
                    break;
                }


                if (sourceFilePathList[currentSourceFileIndex] != "")
                {
                    Console.WriteLine(String.Format("\"{0}\" require regenerating reflection data ( *.csv file )", sourceFilePathList[currentSourceFileIndex]).ToString());
                    clScan_internal(clscanParameterList[currentSourceFileIndex], currentSourceFileIndex, sourceFilePathList.Count);
                }


                int currentFinishedSourceFileIndex = Interlocked.Increment(ref finishedSourceFileCount);
                if(currentFinishedSourceFileIndex >= totalSourceFileCount)
                {
                    break;
                }
            }
           
        }

        private static List<string> GenerateClScanOutputFilePathList(in List<string> sourceFilePathList)
        {
            List<string> clscanOutPutFiles = new List<string>();
            clscanOutPutFiles.Capacity = sourceFilePathList.Count;

            for (int i = 0; i < sourceFilePathList.Count; i++)
            {
                string clScanOutputPath = DirectoryHelper.GetclScanOutputPath(sourceFilePathList[i]);
                clscanOutPutFiles.Add(clScanOutputPath);
            }
            return clscanOutPutFiles;
        }

        public static List<string> clScanSourceFiles(List<string> sourceFilePathList, string additionalDirectories)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            DLLHelper.LoadDLLToConariL(ref clScanConariL, Program.CL_SCAN_FILE_PATH);

            List<string> clscanOutPutFiles = GenerateClScanOutputFilePathList(sourceFilePathList);

            List<string> clscanRegeneratedSourceFilePathes = new List<string>();
            clscanRegeneratedSourceFilePathes.Capacity = sourceFilePathList.Count;
            List<clScanParameter> clscanRegeneratedSourceFileParameterList = new List<clScanParameter>();
            clscanRegeneratedSourceFileParameterList.Capacity = sourceFilePathList.Count;

            int currentSourceFileCount = -1;
            int finishedSourceFileCount = 0;
            int totalRegeneratedSourceFileCount = 0;
            for(int currentSourceFileIndex = 0; currentSourceFileIndex < sourceFilePathList.Count; currentSourceFileIndex++)
            {
                if (ReflectionDataRegenreationSolver.CheckIsSourceFileRequireRegeneration(sourceFilePathList[currentSourceFileIndex]) == true)
                {
                    clscanRegeneratedSourceFilePathes.Add(sourceFilePathList[currentSourceFileIndex]);

                    clScanParameter _clScanParameter = new clScanParameter();
                    _clScanParameter.sourceFilePath = sourceFilePathList[currentSourceFileIndex];
                    _clScanParameter.outputFilePath = clscanOutPutFiles[currentSourceFileIndex];
                    _clScanParameter.additionalDirectories = additionalDirectories;
                    GenerateClScanArguments(ref _clScanParameter);

                    clscanRegeneratedSourceFileParameterList.Add(_clScanParameter);

                    totalRegeneratedSourceFileCount++;
                }
            }
           

            List<Thread> threadList = new List<Thread>();

            currentSourceFileCount++;
            finishedSourceFileCount++;

            clScan_internal(clscanRegeneratedSourceFileParameterList[0], 0, sourceFilePathList.Count);
            //https://reviews.llvm.org/D99652#2661979

            for (int i = 0; i < MAX_CLSCAN_THREAD_COUNT; i++)
            {
                Thread thread = new Thread(() 
                    => clscan_multithread
                        (
                        clscanRegeneratedSourceFilePathes,
                        clscanRegeneratedSourceFileParameterList,
                        ref currentSourceFileCount,
                        ref finishedSourceFileCount,
                        totalRegeneratedSourceFileCount
                        )
                    );

                thread.Start();

                threadList.Add(thread);
            }

            while(finishedSourceFileCount < totalRegeneratedSourceFileCount)
            {
                Thread.Sleep(2000);
            }

            foreach (Thread thread in threadList)
            {
                thread.Join();
            }


            Console.WriteLine("clscan is finished!!");

            DLLHelper.UnLoadDLLFromConariL(ref clScanConariL);

            stopWatch.Stop();
            Console.WriteLine("clScan takes {0}m {1}s", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds);

            return clscanOutPutFiles;
        }
    }
}
