# clReflect_VisualStudio

This library support using [clReflect](https://github.com/SungJJinKang/clReflect_automation) with Project Build File         

1. Analyze Project Build file ( CMakeLists.txt, *.vcxproj.... )
2. Parse All Project source files ( *.cpp ) to Type info database file ( *.csv ) using clReflect with multiple threads ( stored at Project Build Folder )
3. Export Type info database file to Binary, Memory-mapped database ( *.cppbin )  ( stored at Project Build Folder )              

[Example Video](https://youtu.be/KGihaYTzqG8)                    
 
## How To Use

Nuget Console           

Get-Project clReflect_automation | Install-Package Conari          
Get-Project clReflect_automation | Install-Package DllExport        
Get-Project clReflect_automation | Install-Package Newtonsoft.Json     

-----------------------------------------------       

To generate source file dependency list file            
Please enable /sourceDependencies option ( MSVC )          
Please add compiler option ( -M -MT ~~ )      

This is for preventing recompile unchanged files        

## Roadmap

- Fully support parsing *.vcxproj
- Support CMakeLists.txt
