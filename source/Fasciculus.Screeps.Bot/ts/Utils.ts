
export class Utils
{
    static defined<T>(array: Array<T | undefined>): T[]
    {
        return array.filter(x => x !== undefined) as T[];
    }
}