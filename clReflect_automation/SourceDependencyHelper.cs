using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clReflect_automation
{
    class SourceDependencyHelper
    {
        private static Dictionary<String, List<String>> SourceFileDependencyList = new Dictionary<string, List<string>>();

        /// <summary>
        /// return SourceFileDependencyFile Path
        /// ex) c:\Source.cpp -> ~~(SourceFileDependencyFolder\Source.cpp.json
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        private static String GetConvertSourceFilePathToSourceFileDependencyFile(in string sourceFilePath)
        {
            if(Program.DEPENDENCY_FILES_FOLDER.Length == 0 || Program.DEPENDENCY_FILES_FOLDER.Length == 1)
            {
                throw new Exception("DEPENDENCY_FILES_FOLDER is empty");
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(Program.DEPENDENCY_FILES_FOLDER);
            sb.Append(Path.GetFileName(sourceFilePath));
            sb.Append(".json");

            return sb.ToString();
        }

        /// <summary>
        /// https://www.newtonsoft.com/json/help/html/ParsingLINQtoJSON.htm
        /// https://www.newtonsoft.com/json/help/html/QueryingLINQtoJSON.htm
        /// https://www.newtonsoft.com/json/help/html/SelectToken.htm
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        private static List<String> ParseSourceFileDependencyFile(in string sourceFilePath)
        {
            String SourceFileDependencyFilePath = GetConvertSourceFilePathToSourceFileDependencyFile(sourceFilePath);

            List<String> dependencyFilePathList;

            using (StreamReader reader = File.OpenText(SourceFileDependencyFilePath))
            {
                JObject o = (JObject)JToken.ReadFrom(new JsonTextReader(reader));

                JArray categories = (JArray)o["Data"]["Includes"];

                dependencyFilePathList = categories.Select(c => (string)c).ToList();
            }

            return dependencyFilePathList;

        }

        public static List<String> GetSourceFileDependencyList(in string sourceFilePath)
        {
            if(SourceFileDependencyList.ContainsKey(sourceFilePath) == false)
            {
                SourceFileDependencyList[sourceFilePath] = ParseSourceFileDependencyFile(sourceFilePath);
            }

            return SourceFileDependencyList[sourceFilePath];
        }
    }
}
