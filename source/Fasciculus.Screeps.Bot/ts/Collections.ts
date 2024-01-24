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

    at(index: number): T | undefined
    {
        let length = this.length;
        index = Math.round(index);

        if (index < 0 || index >= length) return undefined;

        return this.array[index];
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

    clone(): Vector<T>
    {
        return new Vector(this.array);
    }

    sort(compare: (left: T, right: T) => number): Vector<T>
    {
        if (this.array.length > 1)
        {
            this.array.sort(compare);
        }

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

    forEach(fn: (value: T) => void)
    {
        if (this.length == 0) return;

        this.array.forEach(fn);
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
        let keys: Array<string> = new Array();

        for (let key in dictionary)
        {
            keys.push(key);
        }

        return new Set(keys);
    }

    static values<T>(dictionary: Dictionary<T>): Vector<T>
    {
        let result: Vector<T> = new Vector();

        for (let key in dictionary)
        {
            result.append(dictionary[key]);
        }

        return result;
    }
}