using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace clReflect_automation
{
    class DirectoryHelper
    {
        public static string GetFileDirectoryInProjectFolder(in Program.ConfigureData configureData, in string filename)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(Path.GetDirectoryName(configureData.VCXPROJ_FILE_PATH));
            sb.Append('\\');
            sb.Append(filename);
            return sb.ToString();
        }

        public static string GetFileDirectoryInProjectFolder(in Program.ConfigureData configureData, in StringBuilder filename)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(Path.GetDirectoryName(configureData.VCXPROJ_FILE_PATH));
            sb.Append('\\');
            sb.Append(filename);
            return sb.ToString();
        }

        //

        public static string ConvertPathMacros(in Program.ConfigureData configureData, in string macros)
        {
            string targetString = "";

            switch (macros)
            {
                case "$(VcpkgInstalledDir)":

                    string VcpkgInstalledDir_MacrosDetection_pattern = "<PropertyGroup Label=\"Vcpkg\" Condition=\"'\\$\\(Configuration\\)\\|\\$\\(Platform\\)\\'==\\'(.*?)\\|(.*?)'\\\">.*?\\<VcpkgInstalledDir\\>(.*?)<\\/VcpkgInstalledDir>.*?\\<\\/PropertyGroup\\>";
                    Regex VcpkgInstalledDir_rgx = new Regex(VcpkgInstalledDir_MacrosDetection_pattern, RegexOptions.Multiline | RegexOptions.Singleline);
                    MatchCollection AdditionalPath_matches = VcpkgInstalledDir_rgx.Matches(configureData.VCXPROJ_FILE_TEXT);

                    foreach (Match match in AdditionalPath_matches)
                    {
                        if (
                           match.Groups[1].ToString() == configureData.TARGET_CONFIGURATION
                           &&
                           match.Groups[2].ToString() == configureData.TARGET_PATFORM
                       )
                        {
                            targetString = match.Groups[3].ToString();
                        }
                    }

                    break;

                case "$(SolutionDir)":
                    //TODO : SolutionDirectory ( .sln direcoty ) can be different with ProjectDir ( .vcxproj ) 
                    targetString = Path.GetDirectoryName(configureData.VCXPROJ_FILE_PATH);

                    break;

                case "$(ProjectDir)":

                    targetString = Path.GetDirectoryName(configureData.VCXPROJ_FILE_PATH);

                    break;

                default:

                    throw new Exception(String.Format("Fail To Find Directory Macros ( {0} )", macros));
            }

            if(targetString == "")
            {
                throw new Exception(String.Format("Fail to Parse Direcotory Macros ( {0} )", macros));
            }

            return targetString;

        }

        public static string GetclMergeOutputPath(in Program.ConfigureData configureData)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(Program.ConfigureData.DEFAULT_CL_SCAN_OUT_FILE_NAME);
            sb.Append("_merged");
            sb.Append("_");
            sb.Append(configureData.TARGET_CONFIGURATION);
            sb.Append("_");
            sb.Append(configureData.TARGET_PATFORM);
            sb.Append(".csv");

            string returnPath = GetFileDirectoryInProjectFolder(configureData, sb);

            return returnPath;
        }

        public static string GetclScanOutputPath(in Program.ConfigureData configureData, in string path)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(Path.GetDirectoryName(path));
            sb.Append('\\');
            sb.Append(Path.GetFileNameWithoutExtension(path));
            sb.Append("_");
            sb.Append(configureData.TARGET_CONFIGURATION);
            sb.Append("_");
            sb.Append(configureData.TARGET_PATFORM);
            sb.Append(".csv");

            return sb.ToString();
        }

        public static string GetclExportOutputPath(in Program.ConfigureData configureData)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(Program.ConfigureData.DEFAULT_CL_SCAN_OUT_FILE_NAME);
            sb.Append("_");
            sb.Append(configureData.TARGET_CONFIGURATION);
            sb.Append("_");
            sb.Append(configureData.TARGET_PATFORM);
            sb.Append(".cppbin");

            string returnPath = GetFileDirectoryInProjectFolder(configureData, sb);

            return returnPath;
        }


    }
}
