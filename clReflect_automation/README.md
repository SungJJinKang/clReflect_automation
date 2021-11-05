# clReflect_VisualStudio


## Feature

This library support using [clReflect](https://github.com/Celtoys/clReflect) with Visual Studio            

## How Works

1. Analyze Visual Studio Project File ( *.vcxproj ) file ( AdditionalIncludeDirectories.... )              
2. Generate reflection data files ( *.csv ) using clReflect               
	2.1 Analyze modified data of source file and dependency files of it ( this require /sourceDependencies option in msvc )      
	2.2 Reflection Data is regenerated only when it require        
3. Merge it all to one File

## Setting

Nuget Console           

Get-Project clReflect_automation | Install-Package Conari          
Get-Project clReflect_automation | Install-Package DllExport        
Get-Project clReflect_automation | Install-Package Newtonsoft.Json     

## Roadmap

- Optimization ( clScan is too slow... Can apply multithread? )