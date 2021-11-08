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

        private struct clScanParameter
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



            //sb.Append(" -MD"); // clangtooling이 dependency file 관련된 argument를 무시하는 옵션을 강제로 넣는다. https://intel.github.io/llvm-docs/clang_doxygen/classclang_1_1tooling_1_1StandaloneToolExecutor.html
            /*
            sb.Append(" -MT");
            sb.Append(Path.GetDirectoryName(_clScanParameter.sourceFilePath));
            sb.Append('\\');
            sb.Append(Path.GetFileNameWithoutExtension(_clScanParameter.sourceFilePath));
            sb.Append(".mk");
            */
            return sb.ToString();
        }

        
       

        private static void clScan_internal(clScanParameter _clScanParameter, in int completedSourceFileCount, in int totalSourceFileCount)
        {
            
            int result = 1;

            DLLHelper.LoadDLLToConariL(ref clScanConariL, Program.CL_SCAN_FILE_PATH);
            try
            {
                if (clScanConariL != null)
                {
                    string arvs = GetClScanArgv(_clScanParameter);
                    NativeString<CharPtr> unmanagedStringArgv = new NativeString<CharPtr>(arvs);

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
                MessageBox.Show(String.Format("Fail to clscan file ( Error Code : {0} )", result), "FAIL clscan"); // fails here
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

        public static List<string> clScanSourceFiles(in List<string> sourceFiles, in string additionalDirectories)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            List<string> clscanOutPutFiles = new List<string>();

            for (int i = 0; i < sourceFiles.Count; i++)
            {
                if (sourceFiles[i] != "")
                {
                    string clScanOutputPath = DirectoryHelper.GetclScanOutputPath(sourceFiles[i]);
                    clscanOutPutFiles.Add(clScanOutputPath); // Even if source file isn't recompiled, it still need remerged and reexported

                    if (ReflectionDataRegenreationSolver.CheckIsSourceFileRequireRegeneration(sourceFiles[i]) == true)
                    {
                        clScanParameter _clScanParameter = new clScanParameter();
                        _clScanParameter.sourceFilePath = sourceFiles[i];
                        _clScanParameter.outputFilePath = clScanOutputPath;
                        _clScanParameter.additionalDirectories = additionalDirectories;

                        clscanOutPutFiles.Add(_clScanParameter.outputFilePath);

                        Console.WriteLine(String.Format("\"{0}\" require regenerating reflection data ( *.csv file )", sourceFiles[i]).ToString());
                        clScan_internal(_clScanParameter, i, sourceFiles.Count);

                        Console.WriteLine();
                    }


                }
            }

            Console.WriteLine("clscan is finished!!");

            DLLHelper.UnLoadDLLFromConariL(ref clScanConariL);

            stopWatch.Stop();
            Console.WriteLine("clScan takes {0}m {1}s", stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds);

            return clscanOutPutFiles;
        }
    }
}
