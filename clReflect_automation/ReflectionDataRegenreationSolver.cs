using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clReflect_automation
{
    class ReflectionDataRegenreationSolver
    {
        FileModifiedDateManager mFileModifiedDateManager = new FileModifiedDateManager();
        SourceDependencyFileHelper mSourceDependencyFileHelper = new SourceDependencyFileHelper();

        public bool CheckIsSourceFileRequireRegeneration(in Program.ConfigureData configureData, in string sourceFilePath)
        {

            //Stage 1 : If "~*.csv" file doesn't exist, Regenerate!
            string csvFilePath = DirectoryHelper.GetclScanOutputPath(configureData, sourceFilePath);
            if (File.Exists(csvFilePath) == false)
            {
                return true; // Require regenerating reflection data file
            }


            //Stage 2 : If Source File's modified data is later than "~*.csv" file's modified date, Regenerate!
            DateTime csvFileModified = mFileModifiedDateManager.GetFileModifiedData(csvFilePath);
            bool isSourceFileModified = mFileModifiedDateManager.CheckIsFileModified(sourceFilePath, csvFileModified);
            if (isSourceFileModified == true)
            {
                return true; // Require regenerating reflection data file
            }


            //Stage 3 : If Dependency File of source file doesn't exist
            //          or If Any Dependent file of source File's modified data is later than "~*.csv" file's modified date,
            //          Regenerate!
            List<String> sourceFile_s_DependentFilePathList = mSourceDependencyFileHelper.GetSourceFile_s_DependentFilePathList(configureData, sourceFilePath);
            
            if (sourceFile_s_DependentFilePathList == null || sourceFile_s_DependentFilePathList.Count == 0)
            {
                //If Dependency File of source file doesn't exist
                return true;
            }

            //If Any Dependent file of source File's modified data is later than "~*.csv" file's modified date,
            foreach (String dependenFilePath in sourceFile_s_DependentFilePathList)
            {
                if(mFileModifiedDateManager.CheckIsFileModified(dependenFilePath, csvFileModified) == true)
                {
                    return true;
                }
            }


            return false;
        }
    }
}
