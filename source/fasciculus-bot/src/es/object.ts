
export class Objects
{
    static setFunction<T>(target: T, key: PropertyKey, fn: Function): T
    {
        const attributes: PropertyDescriptor & ThisType<any> =
        {
            configurable: true,
            enumerable: false,
            writable: false,
            value: fn
        };

        return Object.defineProperty(target, key, attributes);
    }

    static setGetter<T>(target: T, key: PropertyKey, fn: () => any): T
    {
        const attributes: PropertyDescriptor & ThisType<any> =
        {
            configurable: true,
            enumerable: true,
            get: fn
        };

        return Object.defineProperty(target, key, attributes);
    }
}