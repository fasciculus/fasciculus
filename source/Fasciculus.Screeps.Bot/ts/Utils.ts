
export class Utils
{
    static round(value: number, precision: number): number
    {
        let factor = Math.pow(10, precision);

        return Math.round(value * factor) / factor;
    }

    static defined<T>(array: Array<T | undefined>): T[]
    {
        return array.filter(x => x !== undefined) as T[];
    }
}