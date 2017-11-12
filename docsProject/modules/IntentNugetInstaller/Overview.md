# Intent Nuget Installer

## Overview

This module provides Nuget support, allowing you to create Nuget dependencies within template and have Nuget packages install these Nuget packages based on Template usage.

## Usage

Implement the 'IHasNugetDependencies' interface (from 'Intent.Modules.Common') on you templates which have Nuget dependencies. 
This interface allows you to describe this dependency.

```csharp

    public class MyTemplate : IHasNugetDependencies
    {
        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new NugetPackageInfo[] { new NugetPackageInfo("Intent.Modules.Common", "1.2.3", "net452") };
        }
    }

```
Intent Nuget Installer will install these dependant Nuget Packages into your solutions, as part of the Software Factory execution.