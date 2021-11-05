# clReflect_VisualStudio


## Feature

This library support using [clReflect](https://github.com/Celtoys/clReflect) with Visual Studio            

1. Analyze Visual Studio Project File ( *.vcxproj ) file ( AdditionalIncludeDirectories.... )
2. Output Type Info Database File ( *.csv ) using clReflect
3. Merge it all to one File

## Setting

Nuget Console           

Get-Project clReflect_automation | Install-Package Conari          
Get-Project clReflect_automation | Install-Package DllExport        
Get-Project clReflect_automation | Install-Package Newtonsoft.Json      