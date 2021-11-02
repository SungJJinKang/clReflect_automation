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
        public static string ConvertPathMacros(in string macros)
        {
            string targetString = "";

            switch (macros)
            {
                case "$(VcpkgInstalledDir)":

                    string VcpkgInstalledDir_MacrosDetection_pattern = "<PropertyGroup Label=\"Vcpkg\" Condition=\"'\\$\\(Configuration\\)\\|\\$\\(Platform\\)\\'==\\'(.*?)\\|(.*?)'\\\">.*?\\<VcpkgInstalledDir\\>(.*?)<\\/VcpkgInstalledDir>.*?\\<\\/PropertyGroup\\>";
                    Regex VcpkgInstalledDir_rgx = new Regex(VcpkgInstalledDir_MacrosDetection_pattern, RegexOptions.Multiline | RegexOptions.Singleline);
                    MatchCollection AdditionalPath_matches = VcpkgInstalledDir_rgx.Matches(Program.VCXPROJ_FILE_TEXT);

                    foreach (Match match in AdditionalPath_matches)
                    {
                        if (
                           match.Groups[1].ToString() == Program.TARGET_CONFIGURATION
                           &&
                           match.Groups[2].ToString() == Program.TARGET_PATFORM
                       )
                        {
                            targetString = match.Groups[3].ToString();
                        }
                    }

                    break;

                case "$(SolutionDir)":
                    //TODO : SolutionDirectory ( .sln direcoty ) can be different with ProjectDir ( .vcxproj ) 
                    targetString = Path.GetDirectoryName(Program.VCXPROJ_FILE_PATH);

                    break;

                case "$(ProjectDir)":

                    targetString = Path.GetDirectoryName(Program.VCXPROJ_FILE_PATH);

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

        public static string GetclMergeOutputPath()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(Path.GetDirectoryName(Program.VCXPROJ_FILE_PATH));
            sb.Append('\\');
            sb.Append(Program.DEFAULT_CL_SCAN_OUT_FILE_NAME);
            sb.Append("_merged");
            sb.Append("_");
            sb.Append(Program.TARGET_CONFIGURATION);
            sb.Append("_");
            sb.Append(Program.TARGET_PATFORM);
            sb.Append(".csv");
            return sb.ToString();
        }

        public static string GetclScanOutputPath(in string path)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(Path.GetDirectoryName(path));
            sb.Append('\\');
            sb.Append(Path.GetFileNameWithoutExtension(path));
            sb.Append("_");
            sb.Append(Program.TARGET_CONFIGURATION);
            sb.Append("_");
            sb.Append(Program.TARGET_PATFORM);
            sb.Append(".csv");
            return sb.ToString();
        }

        public static string GetclExportOutputPath()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(Path.GetDirectoryName(Program.VCXPROJ_FILE_PATH));
            sb.Append('\\');
            sb.Append(Program.DEFAULT_CL_SCAN_OUT_FILE_NAME);
            sb.Append("_");
            sb.Append(Program.TARGET_CONFIGURATION);
            sb.Append("_");
            sb.Append(Program.TARGET_PATFORM);
            sb.Append(".cppbin");
            return sb.ToString();
        }

        public enum eClreflectFileExtension
        { 
            EXE,
            DLL,
            NONE
        };

        public static eClreflectFileExtension GetClreflectFileExtension(in string filePath)
        {
            string extension = Path.GetExtension(filePath);
            if(extension == ".dll")
            {
                return DirectoryHelper.eClreflectFileExtension.DLL;
            }
            else if (extension == ".exe")
            {
                return DirectoryHelper.eClreflectFileExtension.EXE;
            }
            else
            {
                return DirectoryHelper.eClreflectFileExtension.NONE;
            }
        }


    }
}
