using Fasciculus.CodeAnalysis.Frameworking;
using Microsoft.CodeAnalysis;
using System;
using System.IO;

namespace Fasciculus.CodeAnalysis.Models
{
    public class UnparsedProject
    {
        public required Project Project { get; init; }

        public required DirectoryInfo ProjectDirectory { get; init; }

        public required Uri Repository { get; init; }

        public required TargetFramework Framework { get; init; }

    }
}
