﻿using Microsoft.Win32;
using System;

namespace Fasciculus.Windows
{
    public static class RegistryHelper
    {
        public static RegistryKey GetBaseKey(RegistryHive hive)
        {
            return hive switch
            {
                RegistryHive.ClassesRoot => Registry.ClassesRoot,
                RegistryHive.CurrentUser => Registry.CurrentUser,
                RegistryHive.LocalMachine => Registry.LocalMachine,
                RegistryHive.Users => Registry.Users,
                RegistryHive.PerformanceData => Registry.PerformanceData,
                RegistryHive.CurrentConfig => Registry.CurrentConfig,
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
