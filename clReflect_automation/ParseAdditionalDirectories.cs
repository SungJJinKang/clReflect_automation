using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


namespace clReflect_automation
{
    class ParseAdditionalDirectories
    {
        

        private static string ParseAdditionalPath(in string path)
        {
            string AdditionalPath_MacrosDetection_pattern = @"\$\(.*\)";
            Regex AdditionalPath_rgx = new Regex(AdditionalPath_MacrosDetection_pattern, RegexOptions.Singleline);
            Match AdditionalPath_match = AdditionalPath_rgx.Match(path);

            return AdditionalPath_match.Result(DirectoryHelper.ConvertPathMacros(AdditionalPath_match.Groups[0].ToString()));
        }

        public static string GetAdditionalPaths()
        {

            Debug.WriteLine("Detecting Additional Directories..");

            string ItemDefinitionGroup_pattern = @"<ItemDefinitionGroup(.*?)<\/ItemDefinitionGroup>";
            Regex ItemDefinitionGroup_rgx = new Regex(ItemDefinitionGroup_pattern, RegexOptions.Singleline);
            MatchCollection ItemDefinitionGroup_matches = ItemDefinitionGroup_rgx.Matches(Program.VCXPROJ_FILE_TEXT);



            string configuration_platform_patterns = @"\'\$\(Configuration\)\|\$\(Platform\)'=='(.*?)\|(.*?)'";
            Regex configuration_platform_rgx = new Regex(configuration_platform_patterns, RegexOptions.Singleline);

            bool isSuccessToParse = false;

            string resultStr = "";

            foreach (var itemDefinition in ItemDefinitionGroup_matches)
            {
                MatchCollection configuration_platform_matches = configuration_platform_rgx.Matches(itemDefinition.ToString());
                Debug.Assert(configuration_platform_matches[0].Groups.Count == 3, "configuration_platform_matches fail");

                if (
                    configuration_platform_matches[0].Groups[1].ToString() == Program.TARGET_CONFIGURATION
                    &&
                    configuration_platform_matches[0].Groups[2].ToString() == Program.TARGET_PATFORM
                )
                {
                    string AdditionalPaths_patterns = @"<AdditionalIncludeDirectories>(.*?)<\/AdditionalIncludeDirectories>";
                    Regex AdditionalPaths_rgx = new Regex(AdditionalPaths_patterns, RegexOptions.Singleline);
                    MatchCollection AdditionalPaths_matches = AdditionalPaths_rgx.Matches(itemDefinition.ToString());

                    Debug.Assert(AdditionalPaths_matches.Count == 1);

                    var sb = new System.Text.StringBuilder();

                    string[] additionalPathList = AdditionalPaths_matches[0].Groups[1].ToString().Split(';');
                    foreach (string addtionalPath in additionalPathList)
                    {
                        sb.Append("-I\"");
                        sb.Append(ParseAdditionalPath(addtionalPath));
                        sb.Append("\" ");
                    }

                    resultStr = sb.ToString();
                }

            }


            return resultStr;
        }
    }
}
