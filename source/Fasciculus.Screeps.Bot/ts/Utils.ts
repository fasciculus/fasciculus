import * as _ from "lodash";

export class Utils
{
    static round(value: number, precision: number): number
    {
        let factor = Math.pow(10, precision);

        return Math.round(value * factor) / factor;
    }

    static isEmpty<T>(collection: _.List<T> | _.Dictionary<T>): boolean
    {
        return _.size(collection) == 0;
    }

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