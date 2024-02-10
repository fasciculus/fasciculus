import { Objects } from "./object";

export class Arrays
{
    private static indexBy<K, T>(this: Array<T>, toKey: (value: T) => K): Map<K, T>
    {
        const result: Map<K, T> = new Map();

        for (const value of this)
        {
            result.set(toKey(value), value);
        }

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
        Objects.setFunction(Array.prototype, "indexBy", Arrays.indexBy);

        Objects.setFunction(Array, "defined", Arrays.defined);
    }
}