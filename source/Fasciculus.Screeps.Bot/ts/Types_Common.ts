
declare global
{
    interface Map<K, V>
    {
        find(predicate: (value: V) => boolean): Set<K>;
        keep(keys: Set<K>): Map<K, V>
    }
}

function Map_find<K, V>(this: Map<K, V>, predicate: (value: V) => boolean): Set<K>
{
    const result: Set<K> = new Set();

    this.forEach((v, k) => { if (predicate(v)) result.add(k); });

    return result;
}

function Map_keep<K, V>(this: Map<K, V>, keys: Set<K>): Map<K, V>
{
    const toDelete: Array<K> = new Array();

    this.forEach((v, k) => { if (!keys.has(k)) toDelete.push(k); });
    toDelete.forEach(k => this.delete(k));

    return this;
}

if (!Map.prototype.find)
{
    Object.defineProperty(Map.prototype, 'find',
        {
            enumerable: false,
            writable: false,
            configurable: true,
            value: Map_find
        });
}

if (!Map.prototype.keep)
{
    Object.defineProperty(Map.prototype, 'keep',
        {
            enumerable: false,
            writable: false,
            configurable: true,
            value: Map_keep
        });
}

export { };
