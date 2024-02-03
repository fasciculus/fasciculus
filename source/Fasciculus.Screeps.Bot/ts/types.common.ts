
export class Objects
{
    static setFunction<T>(target: T, key: PropertyKey, fn: Function): T
    {
        const attributes: PropertyDescriptor & ThisType<any> =
        {
            configurable: true,
            enumerable: false,
            writable: false,
            value: fn
        };

        return Object.defineProperty(target, key, attributes);
    }

    static setGetter<T>(target: T, key: PropertyKey, fn: () => any): T
    {
        const attributes: PropertyDescriptor & ThisType<any> =
        {
            configurable: true,
            enumerable: true,
            get: fn
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
            value: value
        };

        return Object.defineProperty(target, key, attributes);
    }
}

declare global
{
    interface Array<T>
    {
        toSet(): Set<T>;

        min(fn: (value: T) => number): T | undefined;
    }
}

Objects.setFunction(Array.prototype, "toSet", function<T>(this: Array<T>): Set<T>
{
    return new Set(this);
});

Objects.setFunction(Array.prototype, "min", function <T>(this: Array<T>, fn: (value: T) => number): T | undefined
{
    const length: number = this.length;

    if (length == 0) return undefined;

    var result: T = this[0];

    if (length == 1) return result;

    var minValue: number = fn(result);

    for (let i = 1; i < length; ++i)
    {
        const candidate: T = this[i];
        const value: number = fn(candidate);

        if (value < minValue)
        {
            result = candidate;
            minValue = value;
        }
    }

    return result;
});

declare global
{
    interface Set<T>
    {
        toArray(): Array<T>;
    }
}

Objects.setFunction(Set.prototype, "toArray", function <T>(this: Set<T>): Array<T>
{
    return Array.from(this);
});

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
