using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace clReflect_automation
{
    class ParseSourceFileDirectories
    {
        public static string[] GetSourceFileDirectories(in int count)
        {
            Console.WriteLine("Detecting SourceFile Directories..");

            string SourceFileDirectories_pattern = "<ClCompile Include=\"(.*?)\"";
            Regex SourceFileDirectories_rgx = new Regex(SourceFileDirectories_pattern, RegexOptions.Singleline);
            MatchCollection SourceFileDirectories_matches = SourceFileDirectories_rgx.Matches(Program.VCXPROJ_FILE_TEXT);

            Console.WriteLine("SourceFile Directories List : ");

            System.Text.StringBuilder[] sb = new System.Text.StringBuilder[count];
            for(int i = 0; i < count; i++)
            {
                sb[i] = new System.Text.StringBuilder();
            }

            for (int i = 0; i < SourceFileDirectories_matches.Count; i++)
            {
                string sourceFileDirectory = SourceFileDirectories_matches[i].Groups[1].ToString();
                sourceFileDirectory = sourceFileDirectory.Trim();
                sourceFileDirectory.Replace("\n", "");

                Console.WriteLine(sourceFileDirectory);

                int sbIndex = i % count;

                sb[sbIndex].Append(DirectoryHelper.ConvertPathMacros("$(SolutionDir)"));
                sb[sbIndex].Append('\\');
                sb[sbIndex].Append(sourceFileDirectory);
                sb[sbIndex].Append(' ');
            }

            string[] sourceFileDirectories = new string[count];
            for(int i = 0; i < count; i++)
            {
                sourceFileDirectories[i] = sb[i].ToString();
            }
            return sourceFileDirectories;
        }
    }
}
