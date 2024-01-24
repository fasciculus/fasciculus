export class Utils
{
    static defined<T>(array: Array<T | undefined>): T[]
    {
        let result: T[] = [];

        for (let value of array)
        {
            if (value !== undefined)
            {
                result.push(value);
            }
        }

        return result;
    }
}