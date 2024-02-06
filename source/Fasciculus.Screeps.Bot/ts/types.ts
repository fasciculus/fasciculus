
export class Objects
{
    static keys(o: {} | object | undefined | null): Set<string>
    {
        if (o === undefined || o === null) return new Set<string>();

        const keys = Object.keys(o);

        if (!keys || !Array.isArray(keys) || keys.length == 0) return new Set<string>()

        return new Set<string>(keys);;
    }

    static values<T>(o: { [s: string]: T; } | ArrayLike<T> | undefined | null): Array<T>
    {
        if (o === undefined || o === null) return new Array<T>();

        const values = Object.values(o);

        if (!values || !Array.isArray(values) || values.length == 0) return new Array<T>();

        return values;
    }

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

    static setValue<T>(target: T, key: PropertyKey, value: any | undefined): T
    {
        const attributes: PropertyDescriptor & ThisType<any> =
        {
            configurable: true,
            enumerable: true,
            writable: true,
            value: value
        };

        return Object.defineProperty(target, key, attributes);
    }
}
