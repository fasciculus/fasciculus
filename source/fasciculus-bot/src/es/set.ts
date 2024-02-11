import { Objects } from "./object";

export class Sets
{
    private static keep<T>(this: Set<T>, values: Set<T>): Set<T>
    {
        const toDelete: Set<T> = this.filter(v => !values.has(v));

        toDelete.forEach(v => this.delete(v));

        return this;
    }

    private static filter<T>(this: Set<T>, predicate: (value: T) => boolean): Set<T>
    {
        const result: Set<T> = new Set<T>();

        this.forEach(value => { if (predicate(value)) result.add(value) });

        return result;
    }

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
        Objects.setFunction(Set.prototype, "keep", Sets.keep);
        Objects.setFunction(Set.prototype, "filter", Sets.filter);
        Objects.setFunction(Set.prototype, "map", Sets.map);

        Objects.setFunction(Set, "from", Sets.from);
    }
}