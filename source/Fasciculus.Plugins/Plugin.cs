using System;
using System.IO;

namespace Fasciculus.Plugins
{
    public static class Plugin
    {
        public static void Apply<I>(FileInfo file, Action<I> action)
            where I : class
        {
            using GenericPlugin<I> plugin = new(file);

            action(plugin.Target);
        }

        public static void Apply<I, T1>(FileInfo file, Action<I, T1> action, T1 t1)
            where I : class
            => Apply<I>(file, i => action(i, t1));

        public static void Apply<I, T1, T2>(FileInfo file, Action<I, T1, T2> action, T1 t1, T2 t2)
            where I : class
            => Apply<I>(file, i => action(i, t1, t2));

        public static void Apply<I, T1, T2, T3>(FileInfo file, Action<I, T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
            where I : class
            => Apply<I>(file, i => action(i, t1, t2, t3));

        public static void Apply<I, T1, T2, T3, T4>(FileInfo file, Action<I, T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4)
            where I : class
            => Apply<I>(file, i => action(i, t1, t2, t3, t4));

        public static void Apply<I, T1, T2, T3, T4, T5>(FileInfo file, Action<I, T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
            where I : class
            => Apply<I>(file, i => action(i, t1, t2, t3, t4, t5));

        public static R Select<I, R>(FileInfo file, Func<I, R> func)
            where I : class
        {
            using GenericPlugin<I> plugin = new(file);

            return func(plugin.Target);
        }

        public static R Select<I, T1, R>(FileInfo file, Func<I, T1, R> func, T1 t1)
            where I : class
            => Select<I, R>(file, i => func(i, t1));

        public static R Select<I, T1, T2, R>(FileInfo file, Func<I, T1, T2, R> func, T1 t1, T2 t2)
            where I : class
            => Select<I, R>(file, i => func(i, t1, t2));

        public static R Select<I, T1, T2, T3, R>(FileInfo file, Func<I, T1, T2, T3, R> func, T1 t1, T2 t2, T3 t3)
            where I : class
            => Select<I, R>(file, i => func(i, t1, t2, t3));

        public static R Select<I, T1, T2, T3, T4, R>(FileInfo file, Func<I, T1, T2, T3, T4, R> func, T1 t1, T2 t2, T3 t3, T4 t4)
            where I : class
            => Select<I, R>(file, i => func(i, t1, t2, t3, t4));

        public static R Select<I, T1, T2, T3, T4, T5, R>(FileInfo file, Func<I, T1, T2, T3, T4, T5, R> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
            where I : class
            => Select<I, R>(file, i => func(i, t1, t2, t3, t4, t5));
    }
}
