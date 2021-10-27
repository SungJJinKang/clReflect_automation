# clReflect_VisualStudio

This library support using [clReflect](https://github.com/Celtoys/clReflect) with Visual Studio            

1. Analyze Project Build file ( CMakeLists.txt, *.vcxproj.... ) ( to find compile options. ex ) AdditionalIncludeDirectories.... )
2. Parse All Project source files ( *.cpp ) to Type info database file ( *.csv ) using clReflect ( stored at Project Build Folder )
3. Export Type info database file to Binary, Memory-mapped database ( *.cppbin )  ( stored at Project Build Folder )

## Current Feature
- Support parsing Visual Studio Project File ( *.vcxproj )

## How To Use

clReflect_automation.exe [clscan.exe Path] [clexport.exe Path] [Project Folder ( including CMakeLists.txt, *.vcxproj... )]  [Compile Configuration] [Compile Platform] [Additional Compiler Command Lines ( optional )]                


ex) clReflect_automation.exe C:\Users\hour3\Downloads\clReflect-SDK-v0.5.11\bin\clscan.exe C:\Users\hour3\Downloads\clReflect-SDK-v0.5.11\bin\clexport.exe C:\Voxel_Doom3_From_Scratch\Doom3\Doom3.vcxproj Release x64 -std=c++17

## Roadmap

- Support CMakeLists.txt
