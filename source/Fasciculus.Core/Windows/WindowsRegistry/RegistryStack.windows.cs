using Fasciculus.Collections;
using Microsoft.Win32;
using System;

namespace Fasciculus.Windows.WindowsRegistry
{
    public class RegistryStack : IDisposable
    {
        private DisposableStack<RegistryKey> stack;

        public RegistryKey Top { get; private set; }

        public RegistryStack()
        {
            stack = new();
            Top = RegistryHives.GetRegistryKey(RegistryHive.ClassesRoot);
        }

        ~RegistryStack()
        {
            Close();
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            stack.Dispose();
            stack = new();
            Top = RegistryHives.GetRegistryKey(RegistryHive.ClassesRoot);
        }

        public bool Open(RegistryPath path)
        {
            bool success = true;
            Close();

            Top = RegistryHives.GetRegistryKey(path.Hive);

            foreach (string name in path.Names)
            {
                RegistryKey? key = Top.OpenSubKey(name, false);

                if (key is null)
                {
                    success = false;
                    break;
                }
                else
                {
                    stack.Push(key);
                    Top = key;
                }
            }

            return success;
        }
    }
}
