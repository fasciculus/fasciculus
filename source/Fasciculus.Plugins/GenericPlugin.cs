using Fasciculus.Collections;
using Fasciculus.Threading.Synchronization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Plugins
{
    public class GenericPlugin<T> : PluginBase
        where T : class
    {
        private readonly TaskSafeMutex mutex = new();
        private readonly List<T> targets = [];

        public T[] Targets => GetTargets();

        public T Target => GetTargets().First();

        public GenericPlugin(FileInfo file)
            : base(file) { }

        private T[] GetTargets()
        {
            using Locker locker = Locker.Lock(mutex);

            Update();

            return [.. targets];
        }

        protected override void Load()
        {
            using Locker locker = Locker.Lock(mutex);

            base.Load();

            GetTargetTypes(typeof(T))
                .Select(type => Activator.CreateInstance(type) as T)
                .NotNull()
                .Apply(targets.Add);
        }

        protected override void Unload()
        {
            using Locker locker = Locker.Lock(mutex);

            targets
                .Select(t => t as IDisposable)
                .NotNull()
                .Apply(d => d.Dispose());

            targets.Clear();

            base.Unload();
        }
    }
}
