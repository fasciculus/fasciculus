using Fasciculus.IO;
using Fasciculus.Threading.Synchronization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Fasciculus.Plugins
{
    public abstract class PluginBase : IDisposable
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly FileInfo file;
        private readonly AssemblyName name;
        private PluginLoadContext? context = null;
        private Assembly? assembly = null;
        private DateTime version = DateTime.MinValue;

        public PluginBase(FileInfo file)
        {
            this.file = file;

            name = new(file.NameWithoutExtension());

            Load();
        }

        ~PluginBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool _)
        {
            Unload();
        }

        protected virtual void Load()
        {
            using Locker locker = Locker.Lock(mutex);

            context = new(file);
            assembly = context.LoadFromAssemblyName(name);
            version = file.Update().LastWriteTimeUtc;
        }

        protected virtual void Unload()
        {
            using Locker locker = Locker.Lock(mutex);

            version = DateTime.MinValue;
            assembly = null;

            if (context is not null)
            {
                //WeakReference contextRef = new(context);

                context.Unload();
                context = null;

                //GC.WaitForPendingFinalizers();

                //while (contextRef.IsAlive)
                //{
                //    Thread.Yield();
                //    GC.Collect();
                //    GC.WaitForPendingFinalizers();
                //}
            }
        }

        protected void Update()
        {
            using Locker locker = Locker.Lock(mutex);

            DateTime current = file.Update().LastWriteTimeUtc;

            if (current != version)
            {
                Unload();
                Load();
            }
        }

        protected IEnumerable<Type> GetTargetTypes(Type type)
        {
            using Locker locker = Locker.Lock(mutex);

            return assembly is null ? [] : assembly.GetTypes().Where(t => t.IsAssignableTo(type));
        }
    }

}
