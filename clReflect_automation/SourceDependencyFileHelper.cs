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
    class SourceDependencyFileHelper
    {
        public enum eSourceDependencyFileType
        { 
            MAKEFILE_DEPENDENCY, // .d
            VISUAL_STUDIO_SOURCE_DEPENDENCIES // visual studio source dependency
        }


        private static Dictionary<String, List<String>> SourceFileDependencyList = new Dictionary<string, List<string>>();

        /// <summary>
        /// return SourceFileDependencyFile Path
        /// ex) c:\Source.cpp -> ~~(SourceFileDependencyFolder\Source.cpp.json
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        private static String ConvertSourceFilePathToSourceFileDependencyFile(in string sourceFilePath)
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

        private static eSourceDependencyFileType CheckSourceFileDependencyFileType(in string sourceFilePath)
        {
            return eSourceDependencyFileType.VISUAL_STUDIO_SOURCE_DEPENDENCIES;
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
            List<String> dependencyFilePathList = null;

            eSourceDependencyFileType detectedSourceDependencyFileType = CheckSourceFileDependencyFileType(sourceFilePath);

            switch (detectedSourceDependencyFileType)
            {
                case eSourceDependencyFileType.VISUAL_STUDIO_SOURCE_DEPENDENCIES:

                    String SourceFileDependencyFilePath = ConvertSourceFilePathToSourceFileDependencyFile(sourceFilePath);

                    using (StreamReader reader = File.OpenText(SourceFileDependencyFilePath))
                    {
                        JObject o = (JObject)JToken.ReadFrom(new JsonTextReader(reader));

                        JArray categories = (JArray)o["Data"]["Includes"];

                        dependencyFilePathList = categories.Select(c => (string)c).ToList();
                    }

                    break;

                case eSourceDependencyFileType.MAKEFILE_DEPENDENCY:

                    break;

                default:
                    throw new Exception("Wrong dependencyFileType");
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

        public static bool GetIsDependencyFolderEmpty(in eSourceDependencyFileType dependencyFileType)
        {
            bool isDependencyFolderEmpty = false;

            switch (dependencyFileType)
            {
                case eSourceDependencyFileType.VISUAL_STUDIO_SOURCE_DEPENDENCIES:

                    isDependencyFolderEmpty = Directory.GetFiles(Program.DEPENDENCY_FILES_FOLDER, "*.json", SearchOption.TopDirectoryOnly).Length > 0;

                    break;

                case eSourceDependencyFileType.MAKEFILE_DEPENDENCY:

                    isDependencyFolderEmpty = false;

                    break;

                default:
                    throw new Exception("Wrong dependencyFileType");
            }

            return isDependencyFolderEmpty;
        }


       
    }
}
