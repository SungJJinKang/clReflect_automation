# clReflect_VisualStudio

With this library, reflection datas of project's source files is generated automatically.       

This library support using [clReflect](https://github.com/SungJJinKang/clReflect_automation) with Project Build File         

1. Analyze Project Build file ( *.vcxproj.... )                
2. Parse datas for generating reflection data ( addtional directories, compiler options... )              
3. Check if reflection data file of the source file is need to be regenerated ( Parse [json file](https://docs.microsoft.com/ko-kr/cpp/build/reference/sourcedependencies?view=msvc-170) containing dependency file list of source file, and compare its modified date with reflection file's )                 
4. If the source file require to be regenerated, pass it to [clReflect](https://github.com/SungJJinKang/clReflect) with multiple threads           

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
