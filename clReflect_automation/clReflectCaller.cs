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
        private ConariL clScanConariL = null;
        private ConariL clMergeConariL = null;
        private ConariL clExportConariL = null;

        private bool FindMapFileInProjectFolder(in Program.ConfigureData configureData, ref String mapFilePath)
        {
            string[] files = System.IO.Directory.GetFiles(Path.GetDirectoryName(configureData.VCXPROJ_FILE_PATH), "*.map");
            if(files.Length > 0)
            {
                mapFilePath = files[0];
                return true;
            }
            else
            {
                return false;
            }
        }

        private string GetclExportArguments(in Program.ConfigureData configureData)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(DirectoryHelper.GetclMergeOutputPath(configureData));
            sb.Append(" -cpp ");
            sb.Append(DirectoryHelper.GetclExportOutputPath(configureData));

            String mapFilePath = "";
            if(FindMapFileInProjectFolder(configureData, ref mapFilePath) == true)
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
        public void clExport(in Program.ConfigureData configureData)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            int result = 1;

            DLLHelper.LoadDLLToConariL(ref clExportConariL, configureData.CL_EXPORT_FILE_PATH);
            try
            {
                if (clExportConariL != null)
                {
                    string arvs = GetclExportArguments(configureData);
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
                Console.WriteLine("clExport Success!!! ( Exported Output File Path : {0} )", DirectoryHelper.GetclExportOutputPath(configureData));
            }

            stopWatch.Stop();
            Console.WriteLine("clExport takes {0}m {1}s", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds);
        }

        private string GetclMergeArguments(in Program.ConfigureData configureData, in List<string> clScanOutputFilePathes)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(DirectoryHelper.GetclMergeOutputPath(configureData));
            sb.Append(" -cpp_codegen ");
            sb.Append(DirectoryHelper.GetFileDirectoryInProjectFolder(configureData, Program.ConfigureData.CL_COMPILETIME_GETTYPE_FILE_NAME));
            sb.Append(" ");

            for (int i = 0; i < clScanOutputFilePathes.Count; i++)
            {
                sb.Append(clScanOutputFilePathes[i]);
                sb.Append(' ');
            }

            sb.Length = sb.Length - 1;

            return sb.ToString();
        }

        public void clMerge(in Program.ConfigureData configureData, in List<string> clScanOutputFilePathes)
        {
            if(clScanOutputFilePathes.Count == 0)
            {
                return;
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            int result = 1;

            DLLHelper.LoadDLLToConariL(ref clMergeConariL, configureData.CL_MERGE_FILE_PATH);

            try
            {
                if (clMergeConariL != null)
                {
                    string arvs = GetclMergeArguments(configureData, clScanOutputFilePathes);
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
                Console.WriteLine("clMerge Success!! ( Merged File Path : {0} )", DirectoryHelper.GetclMergeOutputPath(configureData));
            }

            stopWatch.Stop();
            Console.WriteLine("clMerge takes {0}m {1}s", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds);
        }

        

        private void GenerateClScanArguments(in Program.ConfigureData configureData, ref clScanParameter _clScanParameter)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(_clScanParameter.sourceFilePath);
            sb.Append(" --output ");
            sb.Append(_clScanParameter.outputFilePath);
            if(configureData.ROOTCLASS_TYPENAME != null && configureData.ROOTCLASS_TYPENAME.Length > 0)
            {
                sb.Append(" --rootClass_typeName ");
                sb.Append(configureData.ROOTCLASS_TYPENAME);
            }
            if (configureData.VCXPROJ_FILE_PATH != null && configureData.VCXPROJ_FILE_PATH.Length > 0)
            {
                sb.Append(" --project_folder_path ");
                sb.Append(Path.GetDirectoryName(configureData.VCXPROJ_FILE_PATH));
            }
            sb.Append(" -- ");
            sb.Append(Program.ConfigureData.DEFAULT_COMPILER_OPTION);
            sb.Append(' ');
            sb.Append(_clScanParameter.additionalDirectories);
            sb.Append(' ');
            sb.Append(configureData.ADDITIONAL_COMPILER_OPTION);

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

        private object clscanLockObj = new object();
        private int clScan_internal(in clScanParameter _clScanParameter, in int completedSourceFileCount, in int totalSourceFileCount)
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

            Console.Out.Flush();

            return result;
        }



        public int MAX_CLSCAN_THREAD_COUNT = Math.Min(4, Environment.ProcessorCount / 2);

        void clscan_multithread
        (
            in List<string> sourceFilePathList,
            in List<clScanParameter> clscanParameterList,
            ref int currentSourceFileCount, 
            ref int finishedSourceFileCount,
            in int totalSourceFileCount,
            in int threadIndex
        )
        {
            while (true)
            {
                int currentSourceFileIndex = Interlocked.Increment(ref currentSourceFileCount);
                if(currentSourceFileIndex >= totalSourceFileCount)
                {
                    break;
                }

                int result = 1;

                if (sourceFilePathList[currentSourceFileIndex] != "")
                {
                    Console.WriteLine(String.Format("\"{0}\" require regenerating reflection data ( *.csv file )", sourceFilePathList[currentSourceFileIndex]).ToString());
                    result = clScan_internal(clscanParameterList[currentSourceFileIndex], currentSourceFileIndex, sourceFilePathList.Count);
                }


                int currentFinishedSourceFileIndex = Interlocked.Increment(ref finishedSourceFileCount);

                if(result == 0)
                {
                    Console.WriteLine
                    (
                    "Success to clScan! ( Thread Index : {0} ) ( output File Path :  {1}, Completion : {2} / {3} )",
                    threadIndex.ToString(),
                    clscanParameterList[currentSourceFileIndex].outputFilePath,
                    currentFinishedSourceFileIndex.ToString(),
                    totalSourceFileCount.ToString()
                    );
                }
                else 
                {
                    Monitor.Enter(clscanLockObj);
                    MessageBox.Show(String.Format("Fail to clscan file ( Error Code : {0} )", result), "FAIL clscan"); // fails here
                    Monitor.Exit(clscanLockObj);
                }
                

                if (currentFinishedSourceFileIndex >= totalSourceFileCount)
                {
                    break;
                }
            }
           
        }

        private List<string> GenerateClScanOutputFilePathList(in Program.ConfigureData configureData, in List<string> sourceFilePathList)
        {
            List<string> clscanOutPutFiles = new List<string>();
            clscanOutPutFiles.Capacity = sourceFilePathList.Count;

            for (int i = 0; i < sourceFilePathList.Count; i++)
            {
                string clScanOutputPath = DirectoryHelper.GetclScanOutputPath(configureData, sourceFilePathList[i]);
                clscanOutPutFiles.Add(clScanOutputPath);
            }
            return clscanOutPutFiles;
        }

        public List<string> clScanSourceFiles(in Program.ConfigureData configureData, List<string> sourceFilePathList, string additionalDirectories)
        {
            List<string> clscanOutPutFiles = GenerateClScanOutputFilePathList(configureData, sourceFilePathList);

            List<string> clscanRegeneratedSourceFilePathes = new List<string>();
            clscanRegeneratedSourceFilePathes.Capacity = sourceFilePathList.Count;
            List<clScanParameter> clscanRegeneratedSourceFileParameterList = new List<clScanParameter>();
            clscanRegeneratedSourceFileParameterList.Capacity = sourceFilePathList.Count;

            int currentSourceFileCount = -1;
            int finishedSourceFileCount = 0;

            ReflectionDataRegenreationSolver _ReflectionDataRegenreationSolver = new ReflectionDataRegenreationSolver();

            for (int currentSourceFileIndex = 0; currentSourceFileIndex < sourceFilePathList.Count; currentSourceFileIndex++)
            {
                if (_ReflectionDataRegenreationSolver.CheckIsSourceFileRequireRegeneration(configureData, sourceFilePathList[currentSourceFileIndex]) == true)
                {
                    clscanRegeneratedSourceFilePathes.Add(sourceFilePathList[currentSourceFileIndex]);

                    clScanParameter _clScanParameter = new clScanParameter();
                    _clScanParameter.sourceFilePath = sourceFilePathList[currentSourceFileIndex];
                    _clScanParameter.outputFilePath = clscanOutPutFiles[currentSourceFileIndex];
                    _clScanParameter.additionalDirectories = additionalDirectories;
                    GenerateClScanArguments(configureData, ref _clScanParameter);

                    clscanRegeneratedSourceFileParameterList.Add(_clScanParameter);

                }
            }

            Console.WriteLine
                    (
                    "{0} Source Files require reflection data regeneration",
                    clscanRegeneratedSourceFilePathes.Count.ToString()
                    );

            if (clscanRegeneratedSourceFilePathes.Count > 0)
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                Console.WriteLine
                        (
                        "clScan Thread Count : {0}",
                        MAX_CLSCAN_THREAD_COUNT.ToString()
                        );

                DLLHelper.LoadDLLToConariL(ref clScanConariL, configureData.CL_SCAN_FILE_PATH);

                List<Thread> threadList = new List<Thread>();

                for (int i = 0; i < MAX_CLSCAN_THREAD_COUNT && i < clscanRegeneratedSourceFilePathes.Count; i++)
                {
                    int threadIndex = i;
                    Thread thread = new Thread(()
                        => clscan_multithread
                            (
                            clscanRegeneratedSourceFilePathes,
                            clscanRegeneratedSourceFileParameterList,
                            ref currentSourceFileCount,
                            ref finishedSourceFileCount,
                            clscanRegeneratedSourceFilePathes.Count,
                            threadIndex
                            )
                        );

                    thread.Start();

                    threadList.Add(thread);
                }

                while (finishedSourceFileCount < clscanRegeneratedSourceFilePathes.Count)
                {
                    Thread.Sleep(3000);
                }

                foreach (Thread thread in threadList)
                {
                    thread.Join();
                }

                DLLHelper.UnLoadDLLFromConariL(ref clScanConariL);

                Console.Out.Flush();
                Console.WriteLine("clscan is finished!!");


                stopWatch.Stop();
                Console.WriteLine("clScan takes {0}m {1}s", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds);
            }

            

            return clscanOutPutFiles;
        }
    }
}
