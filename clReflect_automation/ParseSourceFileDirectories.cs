using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace clReflect_automation
{
    class ParseSourceFileDirectories
    {
        public static string GetSourceFileDirectories()
        {
            Console.WriteLine("Detecting SourceFile Directories..");

            string SourceFileDirectories_pattern = "<ClCompile Include=\"(.*?)\"";
            Regex SourceFileDirectories_rgx = new Regex(SourceFileDirectories_pattern, RegexOptions.Singleline);
            MatchCollection SourceFileDirectories_matches = SourceFileDirectories_rgx.Matches(Program.VCXPROJ_FILE_TEXT);

            Console.WriteLine("SourceFile Directories List : ");

            var sb = new System.Text.StringBuilder();

            foreach (Match SourceFileDirectories_match in SourceFileDirectories_matches)
            {
                string sourceFileDirectory = SourceFileDirectories_match.Groups[1].ToString();
                sourceFileDirectory = sourceFileDirectory.Trim();
                sourceFileDirectory.Replace("\n", "");

                Console.WriteLine(sourceFileDirectory);
                sb.Append(DirectoryHelper.ConvertPathMacros("$(SolutionDir)"));
                sb.Append('\\');
                sb.Append(sourceFileDirectory);
                sb.Append(' ');
            }

            return sb.ToString();
        }
    }
}
