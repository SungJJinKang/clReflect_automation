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

            if(AdditionalPath_match.Captures.Count == 0)
            {
                return path;
            }
            else
            {
                string parsedAdditionalPath = path.Replace(AdditionalPath_match.Captures[0].ToString(), DirectoryHelper.ConvertPathMacros(AdditionalPath_match.Groups[0].ToString()) + '\\');
                return parsedAdditionalPath;
            }
          
        }

        public static string GetAdditionalPaths()
        {

#if DEBUG
            Console.WriteLine("Detecting Additional Directories..");
#endif

            string ItemDefinitionGroup_pattern = @"<ItemDefinitionGroup(.*?)<\/ItemDefinitionGroup>";
            Regex ItemDefinitionGroup_rgx = new Regex(ItemDefinitionGroup_pattern, RegexOptions.Singleline);
            MatchCollection ItemDefinitionGroup_matches = ItemDefinitionGroup_rgx.Matches(Program.VCXPROJ_FILE_TEXT);



            string configuration_platform_patterns = @"\'\$\(Configuration\)\|\$\(Platform\)'=='(.*?)\|(.*?)'";
            Regex configuration_platform_rgx = new Regex(configuration_platform_patterns, RegexOptions.Singleline);

            bool isSuccessToFindValidConfigurationAndPlatform = false;

            string resultStr = "";

            foreach (var itemDefinition in ItemDefinitionGroup_matches)
            {
                MatchCollection configuration_platform_matches = configuration_platform_rgx.Matches(itemDefinition.ToString());
                
                if (configuration_platform_matches[0].Groups.Count != 3)
                {
                    throw new Exception("Fail To Parse Configuration, Platform");
                }

                if (
                    configuration_platform_matches[0].Groups[1].ToString() == Program.TARGET_CONFIGURATION
                    &&
                    configuration_platform_matches[0].Groups[2].ToString() == Program.TARGET_PATFORM
                )
                {
                    isSuccessToFindValidConfigurationAndPlatform = true;

                    string AdditionalPaths_patterns = @"<AdditionalIncludeDirectories>(.*?)<\/AdditionalIncludeDirectories>";
                    Regex AdditionalPaths_rgx = new Regex(AdditionalPaths_patterns, RegexOptions.Singleline);
                    MatchCollection AdditionalPaths_matches = AdditionalPaths_rgx.Matches(itemDefinition.ToString());

                    if(AdditionalPaths_matches.Count != 1)
                    {
                        throw new Exception("Fail To Parse Addtional Paths");
                    }

                    var sb = new System.Text.StringBuilder();

                    string additionalPaths = AdditionalPaths_matches[0].Groups[1].ToString();
                    additionalPaths = additionalPaths.Trim();
                    additionalPaths.Replace("\n", "");

                    string[] additionalPathList = additionalPaths.Split(';');
                    foreach (string addtionalPath in additionalPathList)
                    {
                        if (addtionalPath != "")
                        {
                            sb.Append(@"-I");
                            string parsedAdditionalPath = ParseAdditionalPath(addtionalPath);
                            sb.Append(parsedAdditionalPath);
                            sb.Append(@" ");

#if DEBUG
                            Console.WriteLine(parsedAdditionalPath);
#endif

                        }
                    }
                    sb.Remove(sb.Length - 1, 1);
                    resultStr = sb.ToString();
                }

            }

            if(isSuccessToFindValidConfigurationAndPlatform == false)
            {
                throw new Exception(String.Format("Fail to Find Correct Configuration ( {0} ) And Platform ( {1} )", Program.TARGET_CONFIGURATION, Program.TARGET_PATFORM));
            }

            return resultStr;
        }
    }
}
