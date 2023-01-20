# Stignore Combiner

## Intro

A utility to combine `.stignore` files under every folder together into one file and correct paths in them, since Syncthing does not support ignore file recursive lookup like Git.

## Example

./subfolder1/.stignore:

```
#include template.txt
!(?i)/build/CMakeCache.txt
(?i)/build
*.obj
```

./subfolder2/.stignore:

```
(?d).DS_Store
(?d).localized
(?d)._*
(?d).Icon*
(?d).fseventsd
(?d).Spotlight-V100
(?d).DocumentRevisions-V100
(?d).TemporaryItems
(?d).Trashes
(?d).Trash-1000
(?d).iCloud
```

./.stignore (output):

```
/// subfolder1/.stignore
#include subfolder1/template.txt
!(?i)/subfolder1/build/CMakeCache.txt
(?i)/subfolder1/build
*.obj

/// subfolder2/.stignore
(?d).DS_Store
(?d).localized
(?d)._*
(?d).Icon*
(?d).fseventsd
(?d).Spotlight-V100
(?d).DocumentRevisions-V100
(?d).TemporaryItems
(?d).Trashes
(?d).Trash-1000
(?d).iCloud
```

## How to run

1. Use dotnet-script: <br>`dotnet script .stignore.gen.csx`
   
2. If you have Visual Studio with C# workflow installed:

   1. Add `(VSInstallationPath)/MSBuild/Current/Bin/Roslyn` to `PATH`, <br>then run `csi .stignore.gen.csx`

   2. Copy-and-paste code to a C# project, build and run as an executable.

## Reference

https://docs.syncthing.net/users/ignoring.html