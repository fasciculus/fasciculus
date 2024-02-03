
declare global
{
    interface Map<K, V>
    {
        find(predicate: (value: V) => boolean): Set<K>;
        keep(keys: Set<K>): Map<K, V>
    }
}

if (Object.getOwnPropertyDescriptor(Map.prototype, "find") === undefined)
{
    Object.defineProperty(Map.prototype, "find",
        {
            enumerable: false,
            writable: false,
            configurable: true,
            value: function <K, V>(this: Map<K, V>, predicate: (value: V) => boolean): Set<K>
            {
                const result: Set<K> = new Set();

                this.forEach((v, k) => { if (predicate(v)) result.add(k); });

                return result;
            }
        });
}

if (Object.getOwnPropertyDescriptor(Map.prototype, "keep") === undefined)
{
    Object.defineProperty(Map.prototype, "keep",
        {
            enumerable: false,
            writable: false,
            configurable: true,
            value: function <K, V>(this: Map<K, V>, keys: Set<K>): Map<K, V>
            {
                const toDelete: Array<K> = new Array();

                this.forEach((v, k) => { if (!keys.has(k)) toDelete.push(k); });
                toDelete.forEach(k => this.delete(k));

                return this;
            }
        });
}

export { };
