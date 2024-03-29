import { Objects } from "./object";

export class Arrays
{
    private static append<T>(this: Array<T>, values: Array<T>): number
    {
        for (const value of values)
        {
            this.push(value);
        }

        return this.length;
    }

    private static indexBy<K, T>(this: Array<T>, toKey: (value: T) => K): Map<K, T>
    {
        const result: Map<K, T> = new Map();

        for (const value of this)
        {
            result.set(toKey(value), value);
        }

        return result;
    }

    private static groupBy<K, T>(this: Array<T>, toKey: (value: T) => K): Map<K, Array<T>>
    {
        const result: Map<K, Array<T>> = new Map();

        for (const value of this)
        {
            const key: K = toKey(value);

            result.ensure(key, () => new Array()).push(value);
        }

        return result;
    }

    private static sum<T>(this: Array<T>, toNumber: (value: T) => number): number
    {
        var result: number = 0;

        this.forEach(v => { result += toNumber(v); });

        return result;
    }

    private static flatten<T>(arrays: Array<Array<T>>): Array<T>
    {
        const result: Array<T> = new Array();

        arrays.forEach(a => result.append(a));

        return result;
    }

    private static defined<T>(values: Array<T | undefined>): Array<T>
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

    static setup()
    {
        Objects.setFunction(Array.prototype, "append", Arrays.append);
        Objects.setFunction(Array.prototype, "indexBy", Arrays.indexBy);
        Objects.setFunction(Array.prototype, "groupBy", Arrays.groupBy);
        Objects.setFunction(Array.prototype, "sum", Arrays.sum);

        Objects.setFunction(Array, "flatten", Arrays.flatten);
        Objects.setFunction(Array, "defined", Arrays.defined);
    }
}