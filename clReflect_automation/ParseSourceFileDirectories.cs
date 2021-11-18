using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace clReflect_automation
{
    class ParseSourceFileDirectories
    {
        public readonly static String COMPILE_TIME_GETTYPE_FILE_NAME_WORD = "compiletime_gettype";

        public static List<string> GetSourceFileDirectories(in Program.ConfigureData configureData)
        {
#if DEBUG
            Console.WriteLine("Detecting SourceFile Directories..");
#endif
            string SourceFileDirectories_pattern = "<ClCompile Include=\"(.*?)\"";
            Regex SourceFileDirectories_rgx = new Regex(SourceFileDirectories_pattern, RegexOptions.Singleline);
            MatchCollection SourceFileDirectories_matches = SourceFileDirectories_rgx.Matches(configureData.VCXPROJ_FILE_TEXT);

#if DEBUG
            Console.WriteLine("SourceFile Directories List : ");
#endif

            List<string> sourceFileList = new List<string>();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < SourceFileDirectories_matches.Count; i++)
            {
                string sourceFileDirectory = SourceFileDirectories_matches[i].Groups[1].ToString();

                if(sourceFileDirectory.Contains(COMPILE_TIME_GETTYPE_FILE_NAME_WORD) == true)
                {
                    continue;
                }

                sourceFileDirectory = sourceFileDirectory.Trim();
                sourceFileDirectory.Replace("\n", "");

#if DEBUG
                Console.WriteLine(sourceFileDirectory);
#endif

                sb.Append(DirectoryHelper.ConvertPathMacros(configureData, "$(SolutionDir)"));
                sb.Append('\\');
                sb.Append(sourceFileDirectory);

                sourceFileList.Add(sb.ToString());

                sb.Clear();
            }

            return sourceFileList;
        }
    }
}
