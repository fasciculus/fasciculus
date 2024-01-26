export interface Dictionary<T>
{
    [index: string]: T;
}

export class Vector<T> implements Iterable<T>
{
    private array: Array<T>;

    constructor(values?: T[])
    {
        this.array = values && values.length > 0 ? Array.from(values) : new Array();
    }

    static from<T>(items?: T[]): Vector<T> { return new Vector(items); }

    get length(): number { return this.array.length; }
    get values(): T[] { return Array.from(this.array); }

    private copyValues(): Array<T> { return Array.from(this.values); }

    toSet(): Set<T> { return new Set(this.array); }
    toArray(): Array<T> { return Array.from(this.values); }

    at(index: number): T | undefined
    {
        index = Math.round(index);

        if (index < 0 || index >= this.length) return undefined;

        return this.array[index];
    }

    take(count: number): Vector<T>
    {
        count = Math.max(0, Math.min(this.array.length, Math.round(count)));

        if (count == 0) return new Vector();

        return Vector.from(this.array.slice(0, count));
    }

    *[Symbol.iterator](): IterableIterator<T>
    {
        for (let i = 0; i < this.array.length; ++i)
        {
            yield this.array[i];
        }
    }

    append(value: T): Vector<T>
    {
        this.array.push(value);

        return this;
    }

    concat(vector: Vector<T>): Vector<T>
    {
        let a: Array<T> = this.array;
        let b: Array<T> = vector.array;

        if (a.length == 0) return new Vector(b);
        if (b.length == 0) return new Vector(a);

        return new Vector(a.concat(b));
    }

    clone(): Vector<T>
    {
        return new Vector(this.array);
    }

    call<R = void>(fn: (vakues: Array<T>) => R): R
    {
        return fn(this.copyValues());
    }

    find(fn: (vakues: Array<T>) => T | undefined): T | undefined
    {
        if (this.length == 0) return undefined;

        return fn(this.copyValues());
    }

    sort(compare: (left: T, right: T) => number): Vector<T>
    {
        if (this.array.length > 1)
        {
            this.array.sort(compare);
        }

        return this;
    }

    filter(predicate: (value: T) => boolean): Vector<T>
    {
        let result: Vector<T> = new Vector();

        for (let value of this.array)
        {
            if (predicate(value))
            {
                result.append(value);
            }
        }

        return result;
    }

    forEach(fn: (value: T) => void): Vector<T>
    {
        if (this.length == 0) return this;

        this.array.forEach(fn);

        return this;
    }

    map<U>(fn: (value: T) => U): Vector<U>
    {
        let result: Vector<U> = new Vector();

        for (let value of this.array)
        {
            result.append(fn(value));
        }

        return result;
    }

    sum(fn: (value: T) => number): number
    {
        var result: number = 0;

        for (let value of this.array)
        {
            result += fn(value);
        }

        return result;
    }

    indexBy(toIndex: (value: T) => string): Dictionary<T>
    {
        let result: Dictionary<T> = {};

        this.forEach(value => result[toIndex(value)] = value);

        return result;
    }

    groupBy(toKey: (value: T) => string): Dictionary<Vector<T>>
    {
        let result: Dictionary<Vector<T>> = {};

        for (let value of this.array)
        {
            let key: string = toKey(value);
            let entry: Vector<T> = result[key] || new Vector();

            entry.append(value);
            result[key] = entry;
        }

        return result
    }

    max(fn: (value: T) => number): T | undefined
    {
        let array: Array<T> = this.array;
        let length: number = array.length;

        if (length == 0) return undefined;

        let result: T = array[0];
        let resultValue: number = fn(result);

        for (let i = 1; i < length; ++i)
        {
            let candidate: T = array[i];
            let candidateValue: number = fn(candidate);

            if (candidateValue > resultValue)
            {
                result = candidate;
                resultValue = candidateValue;
            }
        }

        return result;
    }
}

export class Vectors
{
    static defined<T>(vector: Vector<T | undefined>): Vector<T>
    {
        var result: Vector<T> = new Vector();

        for (let value of vector)
        {
            if (value !== undefined)
            {
                result.append(value);
            }
        }

        return result;
    }

    static flatten<T>(arrays: Vector<Vector<T>>): Vector<T>
    {
        var result: Vector<T> = new Vector();

        for (let array of arrays)
        {
            result = result.concat(array);
        }

        return result;
    }
}

export class Dictionaries
{
    static isEmpty<T>(dictionary: Dictionary<T>): boolean
    {
        for (let key in dictionary)
        {
            return false;
        }

        return true;
    }

    static keys<T>(dictionary: Dictionary<T>): Set<string>
    {
        return new Set(Object.keys(dictionary));
    }

    static values<T>(dictionary: Dictionary<T>): Vector<T>
    {
        return new Vector(Object.values(dictionary)); 
    }

    static clone<T>(dictionary: Dictionary<T>): Dictionary<T>
    {
        return Object.assign({}, dictionary);
    }
}