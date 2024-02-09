
class Arrays
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

    static get descriptors(): PropertyDescriptorMap
    {
        const descriptors: PropertyDescriptorMap =
        {
            "defined": { configurable: true, enumerable: false, writable: false, value: Arrays.defined }
        };

        return descriptors;
    }
}

Object.defineProperties(Array, Arrays.descriptors);