using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace clReflect_automation
{
    class ExportDatabaseDirectoryList
    {
        static private string DEFAULT_DATABASE_DIRECTORY_FILE_NAME = "clReflect_Database_Direcotry_List";
        static public void WriteDatabasePathsListToText
        (
            in List<string> databaseDirecotryList, 
            in string dataDirectoryListTextFileName
        )
        {
            string databaseDirectoryListTextPath = Path.GetDirectoryName(Program.VCXPROJ_FILE_PATH) + "\\" + dataDirectoryListTextFileName + ".txt";

            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(databaseDirectoryListTextPath))
            {
                foreach (string databasePath in databaseDirecotryList)
                {
                    outputFile.WriteLine(databasePath);
                }
            }
        }

        static public void WriteDatabasePathsListToText
        (
            in List<string> databaseDirecotryList
        )
        {
            WriteDatabasePathsListToText(databaseDirecotryList, DEFAULT_DATABASE_DIRECTORY_FILE_NAME);
        }
    }
}
