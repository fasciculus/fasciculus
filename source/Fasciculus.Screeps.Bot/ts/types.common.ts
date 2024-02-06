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
        groupBy<K>(fnKey: (value: T) => K): Map<K, Array<T>>;

        clone(): Array<T>;
        toSet(): Set<T>;
    }

    interface ArrayConstructor
    {
        empty<T>(): Array<T>;

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

        if (count == 0) return new Array<T>();

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
        const result: Map<K, T> = new Map<K, T>();

        for (const value of this)
        {
            result.set(fnKey(value), value);
        }

        return result;
    }

    static groupBy<T, K>(this: Array<T>, fnKey: (value: T) => K): Map<K, Array<T>>
    {
        const result: Map<K, Array<T>> = new Map<K, Array<T>>();

        for (const value of this)
        {
            result.ensure(fnKey(value), () => new Array<T>()).push(value);
        }

        return result;
    }

    static clone<T>(this: Array<T>): Array<T>
    {
        return Array.from(this);
    }

    static toSet<T>(this: Array<T>): Set<T>
    {
        return new Set<T>(this);
    }

    static empty<T>(): Array<T>
    {
        return new Array<T>();
    }

    static defined<T>(values: Iterable<T | undefined>): Array<T>
    {
        var result: Array<T> = new Array<T>();

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
        const result: Array<T> = new Array<T>();

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
Objects.setFunction(Array.prototype, "groupBy", Arrays.groupBy);
Objects.setFunction(Array.prototype, "clone", Arrays.clone);
Objects.setFunction(Array.prototype, "toSet", Arrays.toSet);

Objects.setFunction(Array, "empty", Arrays.empty);
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
        empty<T>(): Set<T>;
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
        return new Set<T>(this);
    }

    static toArray<T>(this: Set<T>): Array<T>
    {
        return Array.from(this);
    }

    static empty<T>(): Set<T>
    {
        return new Set<T>();
    }

    static from<T>(iterable?: Iterable<T> | null): Set<T>
    {
        return iterable ? new Set(iterable) : new Set<T>();
    }

    static union<T>(a: Set<T>, b: Set<T>): Set<T>
    {
        return new Set([...a, ...b]);
    }

    static intersect<T>(a: Set<T>, b: Set<T>): Set<T>
    {
        if (a.size == 0) return new Set<T>(b);
        if (b.size == 0) return new Set<T>(a);

        return new Set(Array.from(a).filter(x => b.has(x)));
    }

    static difference<T>(a: Set<T>, b: Set<T>): Set<T>
    {
        if (a.size == 0) return new Set<T>();
        if (b.size == 0) return new Set<T>(a);

        return new Set(Array.from(a).filter(x => !b.has(x)));
    }

    static flatten<T>(sets: Iterable<Set<T>>): Set<T>
    {
        const array: Array<T> = new Array<T>();

        for (const set of sets)
        {
            array.append(set);
        }

        return new Set<T>(array);
    }
}

Objects.setFunction(Set.prototype, "clone", Sets.clone);
Objects.setFunction(Set.prototype, "toArray", Sets.toArray);

Objects.setFunction(Set, "empty", Sets.empty);
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

        map<U>(fn: (value: V) => U): Map<K, U>;

        update(keys: Set<K>, fnCreate: (key: K) => V): boolean;

        clone(): Map<K, V>;
    }

    interface MapConstructor
    {
        empty<K, V>(): Map<K, V>;
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

    static map<K, V, U>(this: Map<K, V>, fn: (value: V) => U): Map<K, U>
    {
        const result: Map<K, U> = new Map<K, U>();

        this.forEach((v, k) => result.set(k, fn(v)));

        return result;
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

    static empty<K, V>(): Map<K, V>
    {
        return new Map<K, V>();
    }
}

Objects.setFunction(Map.prototype, "ks", Maps.ks);
Objects.setFunction(Map.prototype, "vs", Maps.vs);
Objects.setFunction(Map.prototype, "ensure", Maps.ensure);
Objects.setFunction(Map.prototype, "find", Maps.find);
Objects.setFunction(Map.prototype, "keep", Maps.keep);
Objects.setFunction(Map.prototype, "map", Maps.map);
Objects.setFunction(Map.prototype, "update", Maps.update);

Objects.setFunction(Map, "empty", Maps.empty);

export { };
