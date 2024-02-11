import { Objects } from "./object";

export class Sets
{
    private static map<T, U>(this: Set<T>, fn: (key: T) => U): Array<U>
    {
        const result: Array<U> = new Array();

        this.forEach(v => result.push(fn(v)));

        return result;
    }

    private static from<T>(iterable?: Iterable<T> | null): Set<T>
    {
        return iterable ? new Set(iterable) : new Set();
    }

    static setup()
    {
        Objects.setFunction(Set.prototype, "map", Sets.map);

        Objects.setFunction(Set, "from", Sets.from);
    }
}