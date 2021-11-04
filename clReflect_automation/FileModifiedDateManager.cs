using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace clReflect_automation
{
    class FileModifiedDateManager
    {
        private static Dictionary<String, DateTime> FileModifiedDate;

        private static DateTime GetFileModifiedData(in string filePath)
        {
            return File.GetLastWriteTime(filePath);
        }

        public static bool CheckIsFileModified(in string filePath, in DateTime compareDate)
        {
            bool isModified = true;
            if(FileModifiedDate.ContainsKey(filePath) == false)
            {
                FileModifiedDate.Add(filePath, compareDate);
            }
            else
            {
                DateTime fileModifiedDate = GetFileModifiedData(filePath);

                //Is "File's stored modified date" is later than "compareData"
                isModified = fileModifiedDate.CompareTo(compareDate) > 0; 
            }

            return isModified;
        }
    }
}
