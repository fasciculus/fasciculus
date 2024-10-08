﻿using Fasciculus.Collections;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Windows
{
    public class RegistryInfo
    {
        private readonly RegistryPath[] children;

        public RegistryPath Path { get; }
        public bool Exists { get; }

        public IEnumerable<RegistryPath> Children => children;

        public RegistryInfo(RegistryPath path)
        {
            using DisposableStack<RegistryKey> keys = new();
            RegistryKey parent = RegistryHelper.GetBaseKey(path.Hive);
            bool exists = true;

            foreach (string name in path.Names)
            {
                RegistryKey? key = parent.OpenSubKey(name, false);

                if (key is null)
                {
                    exists = false;
                    break;
                }
                else
                {
                    keys.Push(key);
                    parent = key;
                }
            }

            Path = path;
            Exists = exists;

            if (exists)
            {
                children = parent.GetSubKeyNames().Select(name => RegistryPath.Combine(path, name)).ToArray();
            }
            else
            {
                children = [];
            }
        }
    }
}
