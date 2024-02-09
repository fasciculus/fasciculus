
export class Objects implements IObjects
{
    setFunction<T>(target: T, key: PropertyKey, fn: Function): T
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
}

//Object.defineProperty(Objects, "keys",
//    {
//        configurable: true,
//        enumerable: false,
//        writable: false,
//        value: function (o: {} | object | undefined | null): Set<string>
//        {
//            if (o === undefined || o === null) return new Set();

//            const keys = Object.keys(o);

//            if (!keys || !Array.isArray(keys) || keys.length == 0) return new Set();

//            return new Set<string>(keys);
//        }
//    });

//Object.defineProperty(Objects, "values",
//    {
//        configurable: true,
//        enumerable: false,
//        writable: false,
//        value: function <T>(o: { [s: string]: T; } | undefined | null): Array<T>
//        {
//            if (o === undefined || o === null) return new Array();

//            const keys = Objects.keys(o);
//            const values: Array<T> = new Array();

//            for (const key of keys)
//            {
//                values.push(o[key]);
//            }

//            return values;
//        }
//    });

//Object.defineProperty(Objects, "setFunction",
//    {
//        configurable: true,
//        enumerable: false,
//        writable: false,
//        value: function <T>(target: T, key: PropertyKey, fn: Function): T
//        {
//            const attributes: PropertyDescriptor & ThisType<any> =
//            {
//                configurable: true,
//                enumerable: false,
//                writable: false,
//                value: fn
//            };

//            return Object.defineProperty(target, key, attributes);
//        }
//    });

//Object.defineProperty(Objects, "setGetter",
//    {
//        configurable: true,
//        enumerable: false,
//        writable: false,
//        value: function <T>(target: T, key: PropertyKey, fn: () => any): T
//        {
//            const attributes: PropertyDescriptor & ThisType<any> =
//            {
//                configurable: true,
//                enumerable: true,
//                get: fn
//            };

//            return Object.defineProperty(target, key, attributes);
//        }
//    });

//Object.defineProperty(Objects, "setValue",
//    {
//        configurable: true,
//        enumerable: false,
//        writable: false,
//        value: function <T>(target: T, key: PropertyKey, value: any | undefined): T
//        {
//            const attributes: PropertyDescriptor & ThisType<any> =
//            {
//                configurable: true,
//                enumerable: true,
//                writable: true,
//                value: value
//            };

//            return Object.defineProperty(target, key, attributes);
//        }
//    });
