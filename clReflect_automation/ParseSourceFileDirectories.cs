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
            Debug.WriteLine("Detecting SourceFile Directories..");

            string SourceFileDirectories_pattern = "<ClCompile Include=\"(.*?)\"";
            Regex SourceFileDirectories_rgx = new Regex(SourceFileDirectories_pattern, RegexOptions.Singleline);
            MatchCollection SourceFileDirectories_matches = SourceFileDirectories_rgx.Matches(Program.VCXPROJ_FILE_TEXT);

            Debug.WriteLine("SourceFile Directories List : ");

            var sb = new System.Text.StringBuilder();

            foreach (Match SourceFileDirectories_match in SourceFileDirectories_matches)
            {
                Debug.WriteLine(SourceFileDirectories_match.Groups[1].ToString());
                sb.Append(DirectoryHelper.ConvertPathMacros("$(SolutionDir)"));
                sb.Append(SourceFileDirectories_match.Groups[1].ToString());
                sb.Append(' ');
            }

            return sb.ToString();
        }
    }
}
