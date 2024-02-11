import { Objects } from "./object";

export class Maps
{
    private static ids<K, V>(this: Map<K, V>): Set<K>
    {
        return Set.from(this.keys());
    }

    private static data<K, V>(this: Map<K, V>): Array<V>
    {
        return Array.from(this.values());
    }

    private static keep<K, V>(this: Map<K, V>, keys: Set<K>): Map<K, V>
    {
        const toDelete: Set<K> = this.ids.filter(id => !keys.has(id));

        toDelete.forEach(id => this.delete(id));

        return this;
    }

    private static filter<K, V>(this: Map<K, V>, predicate: (key: K, value: V) => boolean): Map<K, V>
    {
        const result: Map<K, V> = new Map();

        this.forEach((v, k) => { if (predicate(k, v)) result.set(k, v); })

        return result;
    }

    private static ensure<K, V, H>(this: Map<K, V>, key: K, create: (key: K, hint?: H) => V, hint?: H): V
    {
        var result: V | undefined = this.get(key);

        if (!result)
        {
            this.set(key, result = create(key, hint));
        }

        return result;
    }

    private static from<V>(o: { [key: string]: V }): Map<string, V>
    {
        const result: Map<string, V> = new Map();

        for (const key in o)
        {
            result.set(key, o[key]);
        }

        return result;
    }

    static setup()
    {
        Objects.setGetter(Map.prototype, "ids", Maps.ids);
        Objects.setGetter(Map.prototype, "data", Maps.data);
        Objects.setFunction(Map.prototype, "keep", Maps.keep);
        Objects.setFunction(Map.prototype, "filter", Maps.filter);
        Objects.setFunction(Map.prototype, "ensure", Maps.ensure);

        Objects.setFunction(Map, "from", Maps.from);
    }
}