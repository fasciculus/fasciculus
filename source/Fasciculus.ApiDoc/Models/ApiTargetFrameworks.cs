using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiTargetFrameworks
    {
        public SortedSet<ApiTargetFrameworkGroup> Groups { get; } = [];
        public SortedSet<ApiTargetFramework> Monikers { get; } = [];

        public void Add(string fullName)
        {
            ApiTargetFrameworkGroup group = AddGroup(fullName);

            AddMoniker(fullName, group);
        }

        public void Add(ApiTargetFrameworks other)
        {
            foreach (ApiTargetFrameworkGroup otherGroup in other.Groups)
            {
                ApiTargetFrameworkGroup? group = Groups.FirstOrDefault(g => g.Name == otherGroup.Name);

                if (group is null)
                {
                    Groups.Add(otherGroup);
                }
                else
                {
                    otherGroup.Monikers.Apply(m => { group.Monikers.Add(m); });
                }
            }

            other.Monikers.Apply(m => { Monikers.Add(m); });
        }

        private ApiTargetFrameworkGroup AddGroup(string fullName)
        {
            string name = ParseGroupName(fullName);
            ApiTargetFrameworkGroup? group = Groups.FirstOrDefault(g => g.Name == name);

            if (group is null)
            {
                group = new() { Name = name };
                Groups.Add(group);
            }

            return group;
        }

        private void AddMoniker(string fullName, ApiTargetFrameworkGroup group)
        {
            string version = ParseVersion(fullName);
            ApiTargetFramework? tfm = Monikers.FirstOrDefault(m => m.Moniker == fullName && m.Version == version);

            if (tfm is null)
            {
                tfm = new() { Moniker = fullName, Version = version };

                Monikers.Add(tfm);
                group.Monikers.Add(tfm);
            }
        }

        private static string ParseGroupName(string fullName)
        {
            if (fullName.StartsWith("netstandard"))
            {
                return ".NET Standard";
            }

            if (fullName.StartsWith("net"))
            {
                return ".NET";
            }

            return string.Empty;
        }

        private static string ParseVersion(string fullName)
        {
            if (fullName.StartsWith("netstandard"))
            {
                return fullName["netstandard".Length..];
            }

            if (fullName.StartsWith("net9.0-"))
            {
                return string.Concat("9-", fullName.AsSpan("net9.0-".Length));
            }

            if (fullName.StartsWith("net9.0"))
            {
                return "9";
            }

            return string.Empty;
        }
    }
}
