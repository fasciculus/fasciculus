---
Title: "Source-Only Package"
Published: 2025-02-15
Author: "Roger H. Jörg"
Summary: "Creating a source-only NuGet package with Microsoft.Build.NoTargets"
---
# Source-Only Package with Microsoft.Build.NoTargets

!frontmatter{Author, Published}

## Introduction

It is a niche case, but some  NuGet packages do not contain an actual library or executable.
These packages just may contain build scripts (`.props` and/or `.targets` files in the `build`
directory) and/or content files (in the `contentFiles` directory). Packages of this type are
called "source-only" packages.

## How-To

### Adjust the Project SDK

Microsoft provides an SDK for this type of packages. It is called `Microsoft.Build.NoTargets`.
This works only for C# and F# projects. To use it in a C# project, you just change the SDK in your
`.csproj` file from

```xml
<Project Sdk="Microsoft.NET.Sdk">
```

to

```xml
<Project Sdk="Microsoft.Build.NoTargets">
```

Building this with `GeneratePackageOnBuild` set to `true` will get you a bunch of errors.

### Adjust `global.json`

The above doesn't work because MSBuild doesn't know which version of the SDK to use.

Usually you would just write

```xml
<Project Sdk="Microsoft.Build.NoTargets/3.7.56">
```

to use version `3.7.56` of the SDK. Updating this may get forgotten as time goes by. A more elegant
way is to modify `global.json` (you have one sitting at the root of your projects just next to
your solution file, right?). As of this writing, mine looks like this:

```json
{
  "sdk": {
    "version": "9.0.200",
    "rollForward": "latestFeature",
    "allowPrerelease": false
  },
  "msbuild-sdks": {
    "Microsoft.Build.NoTargets": "3.7.56"
  }
}
```

Building the project still gives errors.

### Add content

A package without content makes no sense. So let's add some in the `.csproj` file:

```xml
  <ItemGroup>
    <None Include="README.md" Pack="true" PackagePath="/" />
    <None Include="Properties/Aweful.Library.png" Pack="true" PackagePath="/" />
    <None Include="Build/Aweful.Library.props" Pack="true" PackagePath="/build" />
  </ItemGroup>
```

Building the project still gives errors.

### Add Essential Properties

Besides all the properties you usually set to create a package (`Deterministic`, `DebugType`,
`IncludeSymbols`, `ContinuousIntegrationBuild`, `EmbedUntrackedSources`, `PublishRepositoryUrl`,
`SymbolPackageFormat`, `Version`, `Authors`, `Company`, `Copyright`, `PackageLicenseExpression`,
`PackageReleaseNotes`, ...?), the following must be set:

```xml
<PropertyGroup>
  <IncludeSymbols>false</IncludeSymbols>
  <SuppressDependenciesWhenPacking>true</SuppressDependenciesWhenPacking>
</PropertyGroup>
```

And voilà, the project gets packed.
