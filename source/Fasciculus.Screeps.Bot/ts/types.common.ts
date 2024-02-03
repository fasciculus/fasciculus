
export class Objects
{
    static setFunction<T>(target: T, key: PropertyKey, fn: Function): T
    {
        const attributes: PropertyDescriptor & ThisType<any> =
        {
            configurable: true,
            enumerable: false,
            writable: true,
            value: fn,
        };

        return Object.defineProperty(target, key, attributes);
    }

    static setValue<T>(target: T, key: PropertyKey, value: any | undefined): T
    {
        const attributes: PropertyDescriptor & ThisType<any> =
        {
            configurable: true,
            enumerable: true,
            writable: true,
            value: value,
        };

        return Object.defineProperty(target, key, attributes);
    }
}

declare global
{
    interface Map<K, V>
    {
        find(predicate: (value: V) => boolean): Set<K>;
        keep(keys: Set<K>): Map<K, V>
    }
}

Objects.setFunction(Map.prototype, "find", function <K, V>(this: Map<K, V>, predicate: (value: V) => boolean): Set<K>
{
    const result: Set<K> = new Set();

    this.forEach((v, k) => { if (predicate(v)) result.add(k); });

    return result;
});

Objects.setFunction(Map.prototype, "keep", function <K, V>(this: Map<K, V>, keys: Set<K>): Map<K, V>
{
    const toDelete: Array<K> = new Array();

    this.forEach((v, k) => { if (!keys.has(k)) toDelete.push(k); });
    toDelete.forEach(k => this.delete(k));

    return this;
});

export { };
