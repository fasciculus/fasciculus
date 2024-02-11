
declare global
{
    interface Array<T>
    {
        append(values: Array<T>): number;

        indexBy<K>(toKey: (value: T) => K): Map<K, T>;
    }

    interface ArrayConstructor
    {
        defined<T>(array: Array<T | undefined>): Array<T>;
        flatten<T>(arrays: Array<Array<T>>): Array<T>;
    }

    interface Set<T>
    {
        map<U>(fn: (key: T) => U): Array<U>;
    }

    interface SetConstructor
    {
        from<T>(iterable?: Iterable<T> | null): Set<T>;
    }

    interface Map<K, V>
    {
        get ids(): Set<K>;
        get data(): Array<V>;

        filter(predicate: (key: K, value: V) => boolean): Map<K, V>;

        ensure(key: K, create: (key: K) => V): V;
    }

    interface MapConstructor
    {
        from<V>(o: { [key: string]: V }): Map<string, V>;
    }
}

export { }
