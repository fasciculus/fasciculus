---
Created: 2023-03-18
---
# Custom MSBuild Tasks

2023-03-18, Roger H. J&ouml;rg

## TargetFramework

When building from the command line ("```dotnet build```"), custom tasks run in the current dotnet version. That would be
```TargetFramework``` set to ```net7.0```.

When building from within the Visual Studio IDE, custom tasks run in the .Net Framework of the IDE. That would be
```TargetFramework``` set to ```net472```.

To use custom tasks when building from the command line ("```dotnet build```") as well as from within the Visual Studio IDE, the
best approach is to set ```TargetFramework``` to ```netstandard2.0```.
