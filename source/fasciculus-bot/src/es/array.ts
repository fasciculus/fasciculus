import { Objects } from "./object";

declare global
{
    interface ArrayConstructor
    {
        defined<T>(array: Array<T | undefined>): Array<T>;
    }
}

export class ArrayExt
{
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
        Objects.setFunction(Array, "defined", ArrayExt.defined)
    }
}