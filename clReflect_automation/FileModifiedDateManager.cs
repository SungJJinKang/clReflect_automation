using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace clReflect_automation
{
    class FileModifiedDateManager
    {
        private Dictionary<String, DateTime> FileModifiedDateTimeCache = new Dictionary<string, DateTime>();

        public DateTime GetFileModifiedData(in string filePath)
        {
            DateTime fileModifiedDateTime;

            if (FileModifiedDateTimeCache.ContainsKey(filePath) == true)
            {
                fileModifiedDateTime = FileModifiedDateTimeCache[filePath];
            }
            else
            {
                fileModifiedDateTime = File.GetLastWriteTime(filePath);
                FileModifiedDateTimeCache.Add(filePath, fileModifiedDateTime);
            }

            return fileModifiedDateTime;
        }

        public bool CheckIsFileModified(in string filePath, in DateTime compareDate)
        {
            DateTime fileModifiedDate = GetFileModifiedData(filePath);

            //Is "File's stored modified date" is later than "compareData"
            bool isModified = fileModifiedDate.CompareTo(compareDate) > 0;

            return isModified;
        }
    }
}
