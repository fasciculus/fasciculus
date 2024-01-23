
export class Vector<T> implements Iterable<T>
{
    private array: Array<T>;

    constructor(items?: T[])
    {
        this.array = items && items.length > 0 ? Array.from(items) : new Array();
    }

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
        let result: Vector<T> = new Vector(this.array);

        for (let value of vector.array)
        {
            result.append(value);
        }

        return result;
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

    map<U>(fn: (value: T) => U): Vector<U>
    {
        let result: Vector<U> = new Vector();

        for (let value of this.array)
        {
            result.append(fn(value));
        }

        return result;
    }

    forEach(fn: (value: T) => void)
    {
        if (this.length == 0) return;

        this.array.forEach(fn);
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
}

export class Vectors
{
    static from<T>(items?: T[]): Vector<T> { return new Vector(items); }

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
}