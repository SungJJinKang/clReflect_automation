using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace clReflect_automation
{
    class ParseSourceFileDirectories
    {
        public static List<string> GetSourceFileDirectories()
        {
            Console.WriteLine("Detecting SourceFile Directories..");

            string SourceFileDirectories_pattern = "<ClCompile Include=\"(.*?)\"";
            Regex SourceFileDirectories_rgx = new Regex(SourceFileDirectories_pattern, RegexOptions.Singleline);
            MatchCollection SourceFileDirectories_matches = SourceFileDirectories_rgx.Matches(Program.VCXPROJ_FILE_TEXT);

            Console.WriteLine("SourceFile Directories List : ");

            List<string> sourceFileList = new List<string>();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < SourceFileDirectories_matches.Count; i++)
            {
                string sourceFileDirectory = SourceFileDirectories_matches[i].Groups[1].ToString();
                sourceFileDirectory = sourceFileDirectory.Trim();
                sourceFileDirectory.Replace("\n", "");

                Console.WriteLine(sourceFileDirectory);

                sb.Append(DirectoryHelper.ConvertPathMacros("$(SolutionDir)"));
                sb.Append('\\');
                sb.Append(sourceFileDirectory);

                sourceFileList.Add(sb.ToString());

                sb.Clear();
            }

            return sourceFileList;
        }
    }
}
