using Fasciculus.IO;
using Fasciculus.Support;
using Fasciculus.Threading.Synchronization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Fasciculus.Plugins
{
    public abstract class Plugin : IDisposable
    {
        protected readonly ReentrantTaskSafeMutex mutex = new();

        private FileInfo file;
        private AssemblyName name;
        private PluginLoadContext? context = null;
        private Assembly? assembly = null;
        private DateTime version = DateTime.MinValue;

        public Plugin(FileInfo file)
        {
            this.file = file;

            name = new(file.NameWithoutExtension());

            Load();
        }

        ~Plugin()
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

            if (context is not null)
            {
                context.Unload();
                context = null;
            }

            assembly = null;
            version = DateTime.MinValue;
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

    public class Plugin<T> : Plugin
        where T : class
    {
        private T? target;

        public T Target => GetTarget();

        public Plugin(FileInfo file)
            : base(file) { }

        private T GetTarget()
        {
            using Locker locker = Locker.Lock(mutex);

            Update();

            if (target is null)
            {
                throw Ex.InvalidOperation();
            }

            return target;
        }

        protected override void Load()
        {
            base.Load();

            Type type = GetTargetTypes(typeof(T)).First();

            target = Activator.CreateInstance(type) as T;
        }

        protected override void Unload()
        {
            if (target is IAsyncDisposable asyncDisposable)
            {
                asyncDisposable.DisposeAsync();
            }
            else if (target is IDisposable disposable)
            {
                disposable.Dispose();
            }

            target = null;

            base.Unload();
        }
    }
}
