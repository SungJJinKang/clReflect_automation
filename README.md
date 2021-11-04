# clReflect_VisualStudio

This library support using [clReflect](https://github.com/SungJJinKang/clReflect_automation) with Project Build File         

1. Analyze Project Build file ( CMakeLists.txt, *.vcxproj.... ) ( to find compile options. ex ) AdditionalIncludeDirectories.... )
2. Parse All Project source files ( *.cpp ) to Type info database file ( *.csv ) using clReflect ( stored at Project Build Folder )
3. Export Type info database file to Binary, Memory-mapped database ( *.cppbin )  ( stored at Project Build Folder )


## Current Feature

- Support parsing Visual Studio Project File ( *.vcxproj ) - "Parse Additional Include Directories", "Add target compiler option"

## How To Use

To generate source file dependency list file            
Please enable /sourceDependencies option ( MSVC )          
Please add compiler option ( -M -MT ~~ )      

This is for preventing recompile unchanged files        

----------------------------


```
clReflect_automation.exe [clscan Path] [clmerge Path] [clexport Path] [Project Folder ( including CMakeLists.txt, *.vcxproj... )]  [Compile Configuration] [Compile Platform] [Additional Compiler Command Lines ( optional )]                
```

```
ex) clReflect_automation.exe C:\Users\hour3\Downloads\clReflect-SDK-v0.5.11\bin\clscan.exe C:\Users\hour3\Downloads\clReflect-SDK-v0.5.11\bin\clmerge.exe C:\Users\hour3\Downloads\clReflect-SDK-v0.5.11\bin\clexport.exe C:\Voxel_Doom3_From_Scratch\Doom3\Doom3.vcxproj Release x64 -std=c++17 -w
```

## Roadmap

- Optimize C# Codes
- Analyze SourceFiles metadata and If Source Files changed, Regenerating reflection data
- Fully support parsing *.vcxproj
- Support CMakeLists.txt

