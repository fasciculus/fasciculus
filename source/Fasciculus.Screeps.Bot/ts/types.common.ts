import { Objects } from "./types";

declare global
{
    interface Array<T>
    {
        append(values: Iterable<T>): number;

        take(count: number): Array<T>;

        min(fn: (value: T) => number): T | undefined;
        max(fn: (value: T) => number): T | undefined;

        sum(fn: (value: T) => number): number;
        avg(fn: (value: T) => number): number

        indexBy<K>(fnKey: (value: T) => K): Map<K, T>;

        clone(): Array<T>;
        toSet(): Set<T>;
    }

    interface ArrayConstructor
    {
        defined<T>(array: Iterable<T | undefined>): Array<T>;
        flatten<T>(arrays: Iterable<Iterable<T>>): Array<T>;
    }
}

class Arrays
{
    static append<T>(this: Array<T>, values: Iterable<T>): number
    {
        for (const value of values)
        {
            this.push(value);
        }

        return this.length;
    }

    static take<T>(this: Array<T>, count: number): Array<T>
    {
        count = Math.max(0, Math.min(this.length, Math.round(count)));

        if (count == 0) return new Array();

        return this.slice(0, count)
    }

    static min<T>(this: Array<T>, fn: (value: T) => number): T | undefined
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
    }

    static max<T>(this: Array<T>, fn: (value: T) => number): T | undefined
    {
        const length: number = this.length;

        if (length == 0) return undefined;

        var result: T = this[0];

        if (length == 1) return result;

        var maxValue: number = fn(result);

        for (let i = 1; i < length; ++i)
        {
            const candidate: T = this[i];
            const value: number = fn(candidate);

            if (value > maxValue)
            {
                result = candidate;
                maxValue = value;
            }
        }

        return result;
    }

    static sum<T>(this: Array<T>, fn: (value: T) => number): number
    {
        var result: number = 0;

        for (const value of this)
        {
            result += fn(value);
        }

        return result;
    }

    static avg<T>(this: Array<T>, fn: (value: T) => number): number
    {
        return this.sum(fn) / Math.max(1, this.length);
    }

    static indexBy<T, K>(this: Array<T>, fnKey: (value: T) => K): Map<K, T>
    {
        const result: Map<K, T> = new Map();

        for (const value of this)
        {
            result.set(fnKey(value), value);
        }

        return result;
    }

    static clone<T>(this: Array<T>): Array<T>
    {
        return Array.from(this);
    }

    static toSet<T>(this: Array<T>): Set<T>
    {
        return new Set(this);
    }

    static defined<T>(values: Iterable<T | undefined>): Array<T>
    {
        var result: Array<T> = new Array();

        for (const value of values)
        {
            if (value !== undefined)
            {
                result.push(value);
            }
        }

        return result;
    }

    static flatten<T>(arrays: Iterable<Iterable<T>>): Array<T>
    {
        const result: Array<T> = new Array();

        for (const array of arrays)
        {
            result.append(array);
        }

        return result;
    }
}

Objects.setFunction(Array.prototype, "append", Arrays.append);
Objects.setFunction(Array.prototype, "take", Arrays.take);
Objects.setFunction(Array.prototype, "min", Arrays.min);
Objects.setFunction(Array.prototype, "max", Arrays.max);
Objects.setFunction(Array.prototype, "sum", Arrays.sum);
Objects.setFunction(Array.prototype, "avg", Arrays.avg);
Objects.setFunction(Array.prototype, "indexBy", Arrays.indexBy);
Objects.setFunction(Array.prototype, "clone", Arrays.clone);
Objects.setFunction(Array.prototype, "toSet", Arrays.toSet);

Objects.setFunction(Array, "defined", Arrays.defined);
Objects.setFunction(Array, "flatten", Arrays.flatten);

declare global
{
    interface Set<T>
    {
        clone(): Set<T>;
        toArray(): Array<T>;
    }

    interface SetConstructor
    {
        from<T>(iterable?: Iterable<T> | null): Set<T>

        union<T>(a: Set<T>, b: Set<T>): Set<T>;
        intersect<T>(a: Set<T>, b: Set<T>): Set<T>;
        difference<T>(a: Set<T>, b: Set<T>): Set<T>;

        flatten<T>(sets: Iterable<Set<T>>): Set<T>;
    }
}

class Sets
{
    static clone<T>(this: Set<T>): Set<T>
    {
        return new Set(this);
    }

    static toArray<T>(this: Set<T>): Array<T>
    {
        return Array.from(this);
    }

    static from<T>(iterable?: Iterable<T> | null): Set<T>
    {
        return iterable ? new Set(iterable) : new Set();
    }

    static union<T>(a: Set<T>, b: Set<T>): Set<T>
    {
        return new Set([...a, ...b]);
    }

    static intersect<T>(a: Set<T>, b: Set<T>): Set<T>
    {
        if (a.size == 0) return new Set(b);
        if (b.size == 0) return new Set(a);

        return new Set(Array.from(a).filter(x => b.has(x)));
    }

    static difference<T>(a: Set<T>, b: Set<T>): Set<T>
    {
        if (a.size == 0) return new Set();
        if (b.size == 0) return new Set(a);

        return new Set(Array.from(a).filter(x => !b.has(x)));
    }

    static flatten<T>(sets: Iterable<Set<T>>): Set<T>
    {
        const array: Array<T> = new Array();

        for (const set of sets)
        {
            array.append(set);
        }

        return new Set(array);
    }
}

Objects.setFunction(Set.prototype, "clone", Sets.clone);
Objects.setFunction(Set.prototype, "toArray", Sets.toArray);

Objects.setFunction(Set, "from", Sets.from);
Objects.setFunction(Set, "union", Sets.union);
Objects.setFunction(Set, "intersect", Sets.intersect);
Objects.setFunction(Set, "difference", Sets.difference);
Objects.setFunction(Set, "flatten", Sets.flatten);

declare global
{
    interface Map<K, V>
    {
        ks(): Set<K>;
        vs(): Array<V>;

        ensure(key: K, create: (key: K) => V): V

        find(predicate: (value: V) => boolean): Set<K>;
        keep(keys: Set<K>): Map<K, V>

        update(keys: Set<K>, fnCreate: (key: K) => V): boolean;

        clone(): Map<K, V>;
    }
}

class Maps
{
    static ks<K, V>(this: Map<K, V>): Set<K>
    {
        return Set.from(this.keys());
    }

    static vs<K, V>(this: Map<K, V>): Array<V>
    {
        return Array.from(this.values());
    }

    static ensure<K, V>(this: Map<K, V>, key: K, create: (key: K) => V): V
    {
        var result: V | undefined = this.get(key);

        if (!result)
        {
            this.set(key, result = create(key));
        }

        return result;
    }

    static find<K, V>(this: Map<K, V>, predicate: (value: V) => boolean): Set<K>
    {
        const array: Array<K> = new Array();

        this.forEach((v, k) => { if (predicate(v)) array.push(k); });

        return new Set(array);
    }

    static keep<K, V>(this: Map<K, V>, keys: Set<K>): Map<K, V>
    {
        const toDelete: Array<K> = new Array();

        this.forEach((v, k) => { if (!keys.has(k)) toDelete.push(k); });
        toDelete.forEach(k => this.delete(k));

        return this;
    }

    static update<K, V>(this: Map<K, V>, keys: Set<K>, fnCreate: (key: K) => V): boolean
    {
        const existing: Set<K> = this.ks();
        const remove: Set<K> = Set.difference(existing, keys);
        const create: Set<K> = Set.difference(keys, existing);

        remove.forEach(k => this.delete(k));
        create.forEach(k => this.set(k, fnCreate(k)));

        return remove.size > 0 || create.size > 0;
    }

    static clone<K, V>(this: Map<K, V>): Map<K, V>
    {
        const result: Map<K, V> = new Map();

        this.forEach((v, k) => result.set(k, v));

        return result;
    }
}

Objects.setFunction(Map.prototype, "ks", Maps.ks);
Objects.setFunction(Map.prototype, "vs", Maps.vs);
Objects.setFunction(Map.prototype, "ensure", Maps.ensure);
Objects.setFunction(Map.prototype, "find", Maps.find);
Objects.setFunction(Map.prototype, "keep", Maps.keep);
Objects.setFunction(Map.prototype, "update", Maps.update);

export { };
