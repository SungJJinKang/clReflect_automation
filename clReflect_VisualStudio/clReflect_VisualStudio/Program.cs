using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace clReflect_VisualStudio
{
    class Program
    {

        static List<string> GetAdditionalDirectories
            (
            in string vcxprojPath,
            in string Configuration, 
            in string Platform
            )
        {
            string text = System.IO.File.ReadAllText(vcxprojPath);

            string pattern = @"<ItemDefinitionGroup(.*?)\/ItemDefinitionGroup>";
            Regex rgx = new Regex(pattern, RegexOptions.Singleline);
            MatchCollection matches = rgx.Matches(text);

            foreach(Match match in matches)
            {
                string itemDefGroup = match.Value;
            }

            List<string> additionalDirectories = new List<string>();

            return additionalDirectories;
        }

        static void Main(string[] args)
        {
            GetAdditionalDirectories("C:/Voxel_Doom3_From_Scratch/Doom3/Doom3.vcxproj", "", "");
        }
    }
}
