
declare global
{
    interface Array<T>
    {
        indexBy<K>(toKey: (value: T) => K): Map<K, T>;
    }

    interface ArrayConstructor
    {
        defined<T>(array: Array<T | undefined>): Array<T>;
    }

    interface SetConstructor
    {
        from<T>(iterable?: Iterable<T> | null): Set<T>;
    }

    interface Map<K, V>
    {
        get ids(): Set<K>;
        get data(): Array<V>;

        ensure(key: K, create: (key: K) => V): V;
    }

    interface MapConstructor
    {
        from<V>(o: { [key: string]: V }): Map<string, V>;
    }
}

export { }
