
interface Objects
{
    keys(o: {} | object | undefined | null): Set<string>;
    values<T>(o: { [s: string]: T; } | undefined | null): Array<T>;

    setFunction<T>(target: T, key: PropertyKey, fn: Function): T;
    setGetter<T>(target: T, key: PropertyKey, fn: () => any): T;
    setValue<T>(target: T, key: PropertyKey, value: any | undefined): T
}

declare var Objects: Objects;
